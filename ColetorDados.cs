using System;
using System.IO;
using System.Management;
using System.Text;

namespace HelpDeskAPI
{
    public class ColetorDados
    {
        private string pathTXT = @"\\SERVIDOR\PASTA_LOGS";

        public Notebook ExecutarColeta()
        {
            string fab = ColetarWMI("Win32_ComputerSystem", "Manufacturer");
            string mod = ColetarWMI("Win32_ComputerSystemProduct", "Version");
            if (string.IsNullOrEmpty(mod) || mod.Contains("None")) mod = ColetarWMI("Win32_ComputerSystemProduct", "Name");
            var disco = ColetarArmazenamento();

            Notebook n = new Notebook
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Usuario = Environment.UserName,
                NomeMaquina = Environment.MachineName,
                Processador = ColetarWMI("Win32_Processor", "Name"),
                RAM = ColetarRAM(),
                SO = ColetarWMI("Win32_OperatingSystem", "Caption"),
                NumeroSerie = ColetarWMI("Win32_BaseBoard", "SerialNumber"),
                ModeloFinal = $"{fab} {mod}",
                TotalDisco = disco.total,
                PercentualUso = disco.uso
            };

            GerarLogAutomatico(n);
            return n;
        }

        private void GerarLogAutomatico(Notebook n)
        {
            try
            {
                if (!Directory.Exists(pathTXT)) Directory.CreateDirectory(pathTXT);
                string arqTxt = Path.Combine(pathTXT, $"{n.NomeMaquina}.txt");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("------------------------------------------------------------");
                sb.AppendLine($"DATA: {n.Data}");
                sb.AppendLine($"USUÁRIO: {n.Usuario}");
                sb.AppendLine($"CONFIGURAÇÃO: {n.Processador} / {n.TotalDisco} / {n.RAM}");
                sb.AppendLine("-------------------------------------------------------------");
                sb.AppendLine("");

                File.AppendAllText(arqTxt, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        private string ColetarWMI(string cl, string pr)
        {
            try { using (var b = new ManagementObjectSearcher($"SELECT {pr} FROM {cl}")) foreach (var o in b.Get()) return o[pr]?.ToString().Trim() ?? "N/A"; } catch { }
            return "N/A";
        }

        private string ColetarRAM()
        {
            try { long b = 0; using (var s = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory")) foreach (var o in s.Get()) b += Convert.ToInt64(o["Capacity"]); return Math.Round(b / Math.Pow(1024, 3), 2) + " GB"; } catch { return "0 GB"; }
        }

        private (string total, string uso) ColetarArmazenamento()
        {
            try { DriveInfo d = new DriveInfo("C"); double t = Math.Round(d.TotalSize / Math.Pow(1024, 3), 2); double u = t - Math.Round(d.TotalFreeSpace / Math.Pow(1024, 3), 2); return ($"{t} GB", $"{Math.Round((u / t) * 100, 1)}%"); } catch { return ("N/A", "N/A"); }
        }
    }
}