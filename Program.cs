using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

class Program
{
    // CREDENCIAIS
    static string dominio = "[SEU_DOMINIO]";
    static string usuarioAdmin = "[SEU_USUARIO]";
    static string senhaAdmin = "[SUA_SENHA]";
    static string caminhoRede = @"\\[SEU_CAMINHO_DE_REDE]";

    // CRIAÇÃO DOS ARQUIVOS
    static string arquivoGeral = Path.Combine(caminhoRede, "inventario_geral.csv");
    static string arquivoDuplicidade = Path.Combine(caminhoRede, "historico_duplicidade.csv");
    static string cabecalho = "DATA;USER;MODELO DO NOTEBOOK;NOME DO NOTEBOOK;PROCESSADOR;RAM;ARMAZENAMENTO;%USO;N/S PLACA-MAE;S.O";

    // IMPORTAÇÃO DE DLL PARA AUTENTICAÇÃO
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

    static void Main(string[] args)
    {
        // MENU PRINCIPAL
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("        SISTEMA DE HELP DESK - TIC      ");
            Console.WriteLine("=======================================");
            Console.WriteLine("1. Coletar dados do notebook");
            Console.WriteLine("2. Sair");
            Console.WriteLine("=======================================");
            Console.Write("Escolha uma opcao: ");

            string opcao = Console.ReadLine();
            if (opcao == "1") ExecutarColeta();
            else if (opcao == "2") break;
        }
    }

    static void ExecutarColeta()
    {
        try
        {
            Console.WriteLine("\nIniciando coleta, aguarde...");

            // COLETA DE DADOS
            string dataColeta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string usuarioAtual = Environment.UserName;
            string maquinaAtual = Environment.MachineName;
            string cpuAtual = ColetarWMI("Win32_Processor", "Name");
            string ramAtual = ColetarRAM();
            string soAtual = ColetarWMI("Win32_OperatingSystem", "Caption");
            string serieAtual = ColetarWMI("Win32_BaseBoard", "SerialNumber");
            string fabricante = ColetarWMI("Win32_ComputerSystem", "Manufacturer");
            string modeloComercial = ColetarWMI("Win32_ComputerSystemProduct", "Version");

            // MODELO COMERCIAL DO DISPOSITIVO
            if (string.IsNullOrEmpty(modeloComercial) || modeloComercial.Contains("None"))
                modeloComercial = ColetarWMI("Win32_ComputerSystemProduct", "Name");

            if (modeloComercial.StartsWith(fabricante, StringComparison.OrdinalIgnoreCase))
                modeloComercial = modeloComercial.Substring(fabricante.Length).Trim();

            string fabricanteModeloFinal = $"{fabricante} {modeloComercial}";
            var (totalDisco, percentualUso) = ColetarArmazenamento();

            // FORMATAÇÃO DA LINHA PARA O CSV
            string novaLinha = $"{dataColeta};{usuarioAtual};{fabricanteModeloFinal};{maquinaAtual};{cpuAtual};{ramAtual};{totalDisco};{percentualUso};{serieAtual};{soAtual}";

            // AUTENTICAÇÃO DE CREDENCIAIS DE REDE
            Console.WriteLine("Autenticando credenciais de TI...");

            SafeAccessTokenHandle token;
            bool sucessoLogin = LogonUser(usuarioAdmin, dominio, senhaAdmin, 9, 0, out token);

            if (sucessoLogin)
            {
                // EXECUÇÃO
                WindowsIdentity.RunImpersonated(token, () =>
                {
                    if (!Directory.Exists(caminhoRede)) throw new Exception("Pasta de rede inacessível.");

                    // LEITURA E VALIDAÇÃO DO ARQUIVO EXISTENTE
                    List<string> linhas = File.Exists(arquivoGeral) ? File.ReadAllLines(arquivoGeral, Encoding.UTF8).ToList() : new List<string>();

                    if (linhas.Count == 0 || !linhas[0].Contains("MODELO DO NOTEBOOK"))
                    {
                        linhas.Clear();
                        linhas.Add(cabecalho);
                    }

                    // VERIFICAÇÃO DE MÁQUINA EXISTENTE E DUPLICIDADE
                    bool maquinaEncontrada = false;
                    for (int i = 1; i < linhas.Count; i++)
                    {
                        var colunas = linhas[i].Split(';');
                        if (colunas.Length < 9) continue;

                        if (colunas[8].Trim() == serieAtual)
                        {
                            maquinaEncontrada = true;

                            if (colunas[3].Trim() != maquinaAtual)
                            {
                                SalvarDuplicidadeInterno(arquivoDuplicidade, linhas[i], "NOME_ALTERADO_ANTIGO");
                                SalvarDuplicidadeInterno(arquivoDuplicidade, novaLinha, "NOME_ALTERADO_NOVO");
                            }
                            linhas[i] = novaLinha;
                            break;
                        }
                    }

                    // GRAVAÇÃO DOS DADOS ATUALIZADOS
                    if (!maquinaEncontrada) linhas.Add(novaLinha);
                    File.WriteAllLines(arquivoGeral, linhas, Encoding.UTF8);
                });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[SUCESSO] Dados gravados com sucesso!");
                Console.ResetColor();
            }
            else
            {
                // ERRO DE LOGIN
                int erro = Marshal.GetLastWin32Error();
                throw new Exception($"Erro de Autenticação {erro}. Verifique se as credenciais estão corretas.");
            }
        }
        catch (Exception ex)
        {
            // EXCEÇÕES GERAIS
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[ERRO] " + ex.Message);
            Console.ResetColor();
        }

        Console.WriteLine("\nPressione qualquer tecla...");
        Console.ReadKey();
    }

    // MÉTODOS AUXILIARES DE COLETA WMI E HARDWARE
    static string ColetarWMI(string classe, string prop) { try { using (var busca = new ManagementObjectSearcher($"SELECT {prop} FROM {classe}")) foreach (var obj in busca.Get()) return obj[prop]?.ToString().Trim() ?? "N/A"; } catch { } return "N/A"; }

    static string ColetarRAM() { try { long bytes = 0; using (var busca = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory")) foreach (var obj in busca.Get()) bytes += Convert.ToInt64(obj["Capacity"]); return Math.Round(bytes / Math.Pow(1024, 3), 2) + " GB"; } catch { return "0 GB"; } }

    static (string total, string uso) ColetarArmazenamento() { try { DriveInfo c = new DriveInfo("C"); double totalGB = Math.Round(c.TotalSize / Math.Pow(1024, 3), 2); double ocupado = totalGB - Math.Round(c.TotalFreeSpace / Math.Pow(1024, 3), 2); return ($"{totalGB} GB", $"{Math.Round((ocupado / totalGB) * 100, 1)}%"); } catch { return ("N/A", "N/A"); } }

    // MÉTODO PARA SALVAR HISTÓRICO DE DUPLICIDADE
    static void SalvarDuplicidadeInterno(string caminho, string dados, string motivo) { bool ex = File.Exists(caminho); using (var sw = new StreamWriter(caminho, true, Encoding.UTF8)) { if (!ex) sw.WriteLine("MOTIVO;" + cabecalho); sw.WriteLine($"{motivo};{dados}"); } }
}