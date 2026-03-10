using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HelpDeskAPI
{
    class Program
    {
        private static string saltSistema = "CHAVE_SISTEMA_AQUI";
        private static string hashAcesso = "HASH_GERADO_AQUI";

        static void Main(string[] args)
        {
            ColetorDados coletor = new ColetorDados();
            RepositorioRede repo = new RepositorioRede();
            GerenciadorSoftware software = new GerenciadorSoftware();

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║              SISTEMA TIC             ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine(" 1. Coletar e enviar dados\n 2. Admin Center\n 3. Sair");
                Console.WriteLine("────────────────────────────────────────");
                Console.Write(" Selecione: ");
                string op = Console.ReadLine();

                if (op == "1")
                {
                    try { repo.Salvar(coletor.ExecutarColeta()); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\n [OK] Inventário Atualizado!"); }
                    catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("\n [ERRO] " + ex.Message); }
                    Console.ResetColor(); Console.ReadKey();
                }
                else if (op == "2") MenuAdmin(repo, software);
                else if (op == "3") break;
            }
        }

        static void MenuAdmin(RepositorioRede repo, GerenciadorSoftware software)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║           ACESSO RESTRITO            ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write(" Senha de Administrador: ");
            string sDigitada = LerSenha();

            if (Calcular(sDigitada) == hashAcesso)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(" 1. Administrador 1\n 2. Administrador 2");
                Console.ResetColor();
                Console.Write("\n Admin: ");
                string loginOp = Console.ReadLine();
                string loginNome = loginOp == "1" ? "Admin_1" : "Admin_2";
                DateTime entrada = DateTime.Now;
                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("╔══════════════════════════════════════╗");
                    Console.WriteLine($"║ ADMIN: {loginNome.ToUpper().PadRight(30)}║");
                    Console.WriteLine("╚══════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine(" 1. Instaladores\n 2. Buscar máquinas\n 3. Voltar");
                    Console.WriteLine("────────────────────────────────────────");
                    Console.Write(" Selecione: ");
                    string op = Console.ReadLine();
                    if (op == "1") MenuInst(software);
                    else if (op == "2") ExecutarBusca(repo);
                    else if (op == "3") { repo.RegistrarLogAcesso(loginNome, sDigitada, (DateTime.Now - entrada).ToString(@"hh\:mm\:ss")); break; }
                }
            }
            else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("\n ACESSO NEGADO."); Console.ResetColor(); System.Threading.Thread.Sleep(1500); }
        }

        static void MenuInst(GerenciadorSoftware sw)
        {
            List<int> selecionados = new List<int>();
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║        SELEÇÃO DE INSTALADORES       ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.ResetColor();

                string tag1 = selecionados.Contains(1) ? " - [ESCOLHIDA]" : "";
                string tag2 = selecionados.Contains(2) ? " - [ESCOLHIDA]" : "";
                string tag3 = selecionados.Contains(3) ? " - [ESCOLHIDA]" : "";
                string tag4 = selecionados.Contains(4) ? " - [ESCOLHIDA]" : "";

                Console.WriteLine($" 1. Opção 1{tag1}");
                Console.WriteLine($" 2. Opção 2{tag2}");
                Console.WriteLine($" 3. Opção 3{tag3}");
                Console.WriteLine($" 4. Opção 4{tag4}");
                Console.WriteLine(" 5. CONFIRMAR INSTALAÇÃO");
                Console.WriteLine(" 6. Voltar");
                Console.WriteLine("────────────────────────────────────────");
                Console.Write(" Selecione: ");
                string input = Console.ReadLine();

                if (input == "1" || input == "2" || input == "3" || input == "4")
                {
                    int id = int.Parse(input);
                    if (selecionados.Contains(id)) selecionados.Remove(id); else selecionados.Add(id);
                }
                else if (input == "5")
                {
                    if (selecionados.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n [AVISO] Escolha uma ou mais opções!");
                        Console.ResetColor(); System.Threading.Thread.Sleep(1500);
                    }
                    else
                    {
                        if (ConfirmarLote(selecionados))
                        {
                            sw.ExecutarInstalacaoEmLote(selecionados);
                            Console.WriteLine("\n Tecla para continuar..."); Console.ReadKey();
                            selecionados.Clear();
                        }
                    }
                }
                else if (input == "6") break;
            }
        }

        static bool ConfirmarLote(List<int> selecionados)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       CONFIRMAÇÃO DE INSTALAÇÃO      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine(" Verifique os itens marcados antes de prosseguir.");
            Console.WriteLine("────────────────────────────────────────");
            Console.WriteLine(" 1. Confirmar\n 2. Voltar");
            Console.Write("\n Selecione: ");
            return Console.ReadLine() == "1";
        }

        static void ExecutarBusca(RepositorioRede repo)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║            SISTEMA DE BUSCA          ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine(" 1. Usuário\n 2. Ativo\n 3. Modelo\n 4. Voltar");
            Console.WriteLine("────────────────────────────────────────");
            Console.Write("\n Selecione: ");
            string tipo = Console.ReadLine();

            if (tipo == "4") return;

            Console.Write(" Digite o termo: ");
            string termo = Console.ReadLine().ToLower();
            var res = repo.ObterTodos().Where(x =>
                (tipo == "1" && x.Usuario.ToLower().Contains(termo)) ||
                (tipo == "2" && x.NomeMaquina.ToLower().Contains(termo)) ||
                (tipo == "3" && x.ModeloFinal.ToLower().Contains(termo))).ToList();

            Console.WriteLine($"\n Encontrados: {res.Count}");
            foreach (var r in res) Console.WriteLine($" - {r.NomeMaquina} | {r.Usuario}");

            if (res.Count > 0)
            {
                Console.WriteLine("────────────────────────────────────────");
                Console.WriteLine(" 1. Exportar para Excel\n 2. Voltar");
                Console.Write("\n Selecione: ");
                if (Console.ReadLine() == "1")
                {
                    repo.ExportarParaDesktop(res);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n [OK] Arquivo exportado!");
                    Console.ResetColor(); System.Threading.Thread.Sleep(2000);
                }
            }
            else
            {
                Console.WriteLine("\n Nenhum registro encontrado."); Console.ReadKey();
            }
        }

        static string Calcular(string i) { using (var s = SHA256.Create()) { var b = s.ComputeHash(Encoding.UTF8.GetBytes(i + saltSistema)); var sb = new StringBuilder(); foreach (var x in b) sb.Append(x.ToString("x2")); return sb.ToString(); } }
        static string LerSenha() { string s = ""; ConsoleKeyInfo k; do { k = Console.ReadKey(true); if (k.Key != ConsoleKey.Backspace && k.Key != ConsoleKey.Enter) { s += k.KeyChar; Console.Write("*"); } else if (k.Key == ConsoleKey.Backspace && s.Length > 0) { s = s.Substring(0, s.Length - 1); Console.Write("\b \b"); } } while (k.Key != ConsoleKey.Enter); return s; }
    }
}