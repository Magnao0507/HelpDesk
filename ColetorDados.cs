using System;
using System.IO;
using System.Management;

namespace HelpDeskAPI
{
    public class ColetorDados
    {
        public Notebook ExecutarColeta()
        {
            string fab = ColetarWMI("Win32_ComputerSystem", "Manufacturer");
            string mod = ColetarWMI("Win32_ComputerSystemProduct", "Version");

            if (string.IsNullOrEmpty(mod) || mod.Contains("None"))
            {
                mod = ColetarWMI("Win32_ComputerSystemProduct", "Name");
            }

            var disco = ColetarArmazenamento();

            return new Notebook
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
        }

        private string ColetarWMI(string classe, string propriedade)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT {propriedade} FROM {classe}"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj[propriedade]?.ToString().Trim() ?? "N/A";
                    }
                }
            }
            catch
            {
            }
            return "N/A";
        }

        private string ColetarRAM()
        {
            try
            {
                long bytes = 0;
                using (var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        bytes += Convert.ToInt64(obj["Capacity"]);
                    }
                }
                return Math.Round(bytes / Math.Pow(1024, 3), 2) + " GB";
            }
            catch
            {
                return "0 GB";
            }
        }

        private (string total, string uso) ColetarArmazenamento()
        {
            try
            {
                DriveInfo drive = new DriveInfo("C");
                double totalGB = Math.Round(drive.TotalSize / Math.Pow(1024, 3), 2);
                double usadoGB = totalGB - Math.Round(drive.TotalFreeSpace / Math.Pow(1024, 3), 2);

                return ($"{totalGB} GB", $"{Math.Round((usadoGB / totalGB) * 100, 1)}%");
            }
            catch
            {
                return ("N/A", "N/A");
            }
        }
    }
}