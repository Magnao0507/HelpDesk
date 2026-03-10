using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace HelpDeskAPI
{
    public class GerenciadorSoftware
    {
        private string pathArquivos = @"\\SERVIDOR\PASTA_INSTALADORES";
        private string pathAlternativo = @"\\SERVIDOR\PASTA_BACKUP";

        public void ExecutarInstalacaoEmLote(List<int> ids)
        {
            List<Process> processosAtivos = new List<Process>();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [SISTEMA] Iniciando processos...");
            Console.ResetColor();

            foreach (int id in ids)
            {
                Process p = null;
                if (id == 1) p = PrepararEIniciar(pathArquivos, "inst_1.exe", "Software 1", "", true);
                else if (id == 2) p = PrepararEIniciar(pathArquivos, "inst_2.exe", "Software 2", "", true);
                else if (id == 3) p = PrepararEIniciar(pathArquivos, "inst_3.exe", "Software 3", "", true);
                else if (id == 4) p = PrepararEIniciar(pathAlternativo, "inst_4.exe", "Software 4", "/S", false);

                if (p != null) processosAtivos.Add(p);
            }

            foreach (var p in processosAtivos) { try { p.WaitForExit(); } catch { } }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [OK] Tarefas concluídas!");
            Console.ResetColor();
        }

        private Process PrepararEIniciar(string pastaOrigem, string arq, string nome, string arg, bool invisivel)
        {
            string orig = Path.Combine(pastaOrigem, arq);
            string dest = Path.Combine(Path.GetTempPath(), arq);
            try
            {
                if (File.Exists(orig))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($" [SISTEMA] Processando {nome}...");
                    File.Copy(orig, dest, true);
                    ProcessStartInfo psi = new ProcessStartInfo { FileName = dest, Arguments = arg, UseShellExecute = true, CreateNoWindow = invisivel };
                    return Process.Start(psi);
                }
            }
            catch { }
            return null;
        }
    }
}