using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace HelpDeskAPI
{
    public class GerenciadorSoftware
    {
        private string pathTIC = @"\\eni-tag-1899\Depto\24.TIC Pública\INSTALADORES";
        private string pathBit = @"\\eni-tag-1899\Publico\Arthur Magno\Downloads\BITDEFENDER\SEDE";

        public void ExecutarInstalacaoEmLote(List<int> ids)
        {
            List<Process> processosAtivos = new List<Process>();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [SISTEMA] Iniciando instalações...");
            Console.ResetColor();

            foreach (int id in ids)
            {
                Process p = null;
                if (id == 1) p = PrepararEIniciar(pathTIC, "Ninite AnyDesk Chrome Foxit Reader Google Earth Installer.exe", "Ninite", "", true);
                else if (id == 2) p = PrepararEIniciar(pathTIC, "OfficeSetup.exe", "Office 365", "", true);
                else if (id == 3) p = PrepararEIniciar(pathTIC, "Project.exe", "Project", "", true);
                else if (id == 4)
                {
                    string linkX = "BitDefender.lnk";
                    if (File.Exists(Path.Combine(pathTIC, linkX)))
                        p = PrepararEIniciar(pathTIC, linkX, "Bitdefender", "", false);
                    else
                    {
                        string nomeBD = "setupdownloader_[aHR0cHM6Ly9jbG91ZC1lY3MuZ3Jhdml0eXpvbmUuYml0ZGVmZW5kZXIuY29tL1BhY2thZ2VzL0JTVFdJTi8wLzhLUGhhRC9pbnN0YWxsZXIueG1sP2xhbmc9cHQtQlI=].exe";
                        p = PrepararEIniciar(pathBit, nomeBD, "Bitdefender", "/S", false);
                    }
                }
                if (p != null) processosAtivos.Add(p);
            }

            Console.WriteLine("\n [SISTEMA] Aguardando conclusão de todas instalações...");
            foreach (var p in processosAtivos) { try { p.WaitForExit(); } catch { } }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n [OK] Processos finalizados!");
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
                    Console.WriteLine($" [SISTEMA] Copiando e iniciando {nome}...");
                    File.Copy(orig, dest, true);
                    ProcessStartInfo psi = new ProcessStartInfo { FileName = dest, Arguments = arg, UseShellExecute = true, CreateNoWindow = invisivel };
                    return Process.Start(psi);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" [ERRO] {arq} não encontrado.");
                }
            }
            catch (Exception ex) { Console.WriteLine(" [ERRO]: " + ex.Message); }
            return null;
        }
    }
}