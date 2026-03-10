using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HelpDeskAPI
{
    public class RepositorioRede
    {
        public string CaminhoRede { get; } = @"\\eni-tag-1899\TIC";

        public void Salvar(Notebook n)
        {
            try
            {
                string arq = Path.Combine(CaminhoRede, "inventario_geral.csv");
                string pastaDup = Path.Combine(CaminhoRede, "Duplicados");
                if (!Directory.Exists(pastaDup)) Directory.CreateDirectory(pastaDup);
                string arqDup = Path.Combine(pastaDup, "historico_duplicados.csv");

                List<string> l = File.Exists(arq) ? File.ReadAllLines(arq, Encoding.UTF8).ToList() : new List<string> { "DATA;USER;MODELO;NOME;CPU;RAM;DISCO;%USO;N/S;SO" };
                string nl = n.GerarLinhaCsv();
                bool ex = false;

                for (int i = 1; i < l.Count; i++)
                {
                    if (l[i].Contains(n.NumeroSerie))
                    {
                        using (var sw = new StreamWriter(arqDup, true, Encoding.UTF8)) { sw.WriteLine(l[i]); }
                        l[i] = nl;
                        ex = true;
                        break;
                    }
                }

                if (!ex) l.Add(nl);
                File.WriteAllLines(arq, l, Encoding.UTF8);
            }
            catch (Exception ex) { throw new Exception("Erro ao acessar o arquivo central: " + ex.Message); }
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
                        var n = Notebook.DeCsv(l); if (n != null) list.Add(n);
                    }
                }
            }
            catch { }
            return list;
        }

        public void RegistrarAcessoNegado(string login, string senha) => GravarLog("acesso_negado.csv", $"{DateTime.Now};{login};{Environment.UserName};{Environment.MachineName};{senha}");
        public void RegistrarLogAcesso(string login, string senha, string tempo) => GravarLog("log_acesso.csv", $"{DateTime.Now};{login};{Environment.UserName};{Environment.MachineName};{senha};{tempo}");

        private void GravarLog(string arq, string cont)
        {
            try
            {
                string path = Path.Combine(CaminhoRede, arq);
                using (var sw = new StreamWriter(path, true, Encoding.UTF8)) { sw.WriteLine(cont); }
            }
            catch { }
        }

        public void ExportarParaDesktop(List<Notebook> res)
        {
            string arq = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.csv");
            List<string> l = new List<string> { "DATA;USER;MODELO;NOME;CPU;RAM;DISCO;%USO;N/S;SO" };
            foreach (var n in res) l.Add(n.GerarLinhaCsv());
            File.WriteAllLines(arq, l, Encoding.UTF8);
        }
    }
}