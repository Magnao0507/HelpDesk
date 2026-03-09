using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HelpDeskAPI
{
    public class RepositorioRede
    {
        private readonly string pathHost = @"[CAMINHO_REDE_HOSTNAME]";
        private readonly string pathIP = @"[CAMINHO_REDE_IP]";

        public string CaminhoRede => Directory.Exists(pathHost) ? pathHost : pathIP;

        public void Salvar(Notebook n)
        {
            try
            {
                string arq = Path.Combine(CaminhoRede, "inventario_geral.csv");
                List<string> l = File.Exists(arq) ? File.ReadAllLines(arq, Encoding.UTF8).ToList() : new List<string> { "DATA;USER;MODELO;NOME;CPU;RAM;DISCO;%USO;N/S;SO" };

                bool ex = false;
                string nl = n.GerarLinhaCsv();

                for (int i = 1; i < l.Count; i++)
                {
                    if (l[i].Contains(n.NumeroSerie))
                    {
                        ex = true;
                        l[i] = nl;
                        break;
                    }
                }

                if (!ex) l.Add(nl);
                File.WriteAllLines(arq, l, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha de conexão com o repositório de rede: " + ex.Message);
            }
        }

        public void RegistrarAcessoNegado(string login, string senha) => GravarLog("acesso_negado.csv", $"{DateTime.Now};[LOGIN];{Environment.UserName};{Environment.MachineName};[SENHA]");

        public void RegistrarLogAcesso(string login, string senha, string tempo) => GravarLog("log_acesso.csv", $"{DateTime.Now};[LOGIN];{Environment.UserName};{Environment.MachineName};[SENHA];{tempo}");

        private void GravarLog(string arq, string cont)
        {
            try
            {
                string path = Path.Combine(CaminhoRede, arq);
                using (var sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(cont);
                }
            }
            catch
            {
            }
        }

        public List<Notebook> ObterTodos()
        {
            List<Notebook> list = new List<Notebook>();
            string arq = Path.Combine(CaminhoRede, "inventario_geral.csv");
            try
            {
                if (File.Exists(arq))
                {
                    foreach (var l in File.ReadAllLines(arq, Encoding.UTF8).Skip(1))
                    {
                        var n = Notebook.DeCsv(l);
                        if (n != null) list.Add(n);
                    }
                }
            }
            catch
            {
            }
            return list;
        }

        public void ExportarParaDesktop(List<Notebook> res)
        {
            string arq = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.csv");
            List<string> l = new List<string> { "DATA;USER;MODELO;NOME;CPU;RAM;DISCO;%USO;N/S;SO" };

            foreach (var n in res)
            {
                l.Add(n.GerarLinhaCsv());
            }

            File.WriteAllLines(arq, l, Encoding.UTF8);
        }
    }
}