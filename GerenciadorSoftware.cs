using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace HelpDeskAPI
{
    public class GerenciadorSoftware
    {
        private readonly string hostTIC = @"[CAMINHO_REDE_TIC_HOSTNAME]";
        private readonly string hostBit = @"[CAMINHO_REDE_BITDEFENDER_HOSTNAME]";

        private readonly string ipTIC = @"[CAMINHO_REDE_TIC_IP]";
        private readonly string ipBit = @"[CAMINHO_REDE_BITDEFENDER_IP]";

        private string GetPathTIC() => Directory.Exists(hostTIC) ? hostTIC : ipTIC;
        private string GetPathBit() => Directory.Exists(hostBit) ? hostBit : ipBit;

        public void ExecutarInstalacaoEmLote(List<int> ids)
        {
            List<Process> processosAtivos = new List<Process>();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [SISTEMA] Iniciando instalações simultâneas...");
            Console.ResetColor();

            string currentTIC = GetPathTIC();
            string currentBit = GetPathBit();

            foreach (int id in ids)
            {
                Process p = null;

                switch (id)
                {
                    case 1:
                        p = PrepararEIniciar(currentTIC, "Ninite AnyDesk Chrome Foxit Reader Google Earth Installer.exe", "Ninite Bundle", "", true);
                        break;
                    case 2:
                        p = PrepararEIniciar(currentTIC, "OfficeSetup.exe", "Office 365", "", true);
                        break;
                    case 3:
                        p = PrepararEIniciar(currentTIC, "Project.exe", "Microsoft Project", "", true);
                        break;
                    case 4:
                        string linkShortcut = "BitDefender.lnk";
                        if (File.Exists(Path.Combine(currentTIC, linkShortcut)))
                        {
                            p = PrepararEIniciar(currentTIC, linkShortcut, "Bitdefender", "", false);
                        }
                        else
                        {
                            string nomeBD = "setupdownloader_[TOKEN_DE_INSTALACAO_BITDEFENDER].exe";
                            p = PrepararEIniciar(currentBit, nomeBD, "Bitdefender", "/S", false);
                        }
                        break;
                }

                if (p != null) processosAtivos.Add(p);
            }

            Console.WriteLine("\n [SISTEMA] Aguardando conclusão de todos os processos...");

            foreach (var p in processosAtivos)
            {
                try
                {
                    if (!p.HasExited) p.WaitForExit();
                }
                catch
                {
                }
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [OK] Processos finalizados!");
            Console.ResetColor();
        }

        private Process PrepararEIniciar(string pastaOrigem, string arquivoNome, string nomeExibicao, string argumentos, bool ocultarJanela)
        {
            string pathOrigemCompleto = Path.Combine(pastaOrigem, arquivoNome);
            string pathDestinoLocal = Path.Combine(Path.GetTempPath(), arquivoNome);

            try
            {
                if (File.Exists(pathOrigemCompleto))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($" [SISTEMA] Copiando e iniciando {nomeExibicao}...");
                    Console.ResetColor();

                    File.Copy(pathOrigemCompleto, pathDestinoLocal, true);

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = pathDestinoLocal,
                        Arguments = argumentos,
                        UseShellExecute = true,
                        CreateNoWindow = ocultarJanela
                    };

                    return Process.Start(psi);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" [ERRO] Arquivo {arquivoNome} não encontrado no repositório.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [ERRO]: {ex.Message}");
            }

            return null;
        }
    }
}