namespace HelpDeskAPI
{
    public class Notebook
    {
        public string Data { get; set; }
        public string Usuario { get; set; }
        public string ModeloFinal { get; set; }
        public string NomeMaquina { get; set; }
        public string Processador { get; set; }
        public string RAM { get; set; }
        public string TotalDisco { get; set; }
        public string PercentualUso { get; set; }
        public string NumeroSerie { get; set; }
        public string SO { get; set; }

        public string GerarLinhaCsv() => $"{Data};{Usuario};{ModeloFinal};{NomeMaquina};{Processador};{RAM};{TotalDisco};{PercentualUso};{NumeroSerie};{SO}";

        public static Notebook DeCsv(string linha)
        {
            var c = linha.Split(';');
            if (c.Length < 10) return null;
            return new Notebook
            {
                Data = c[0],
                Usuario = c[1],
                ModeloFinal = c[2],
                NomeMaquina = c[3],
                Processador = c[4],
                RAM = c[5],
                TotalDisco = c[6],
                PercentualUso = c[7],
                NumeroSerie = c[8],
                SO = c[9]
            };
        }
    }
}