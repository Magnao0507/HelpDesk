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
            if (string.IsNullOrWhiteSpace(linha)) return null;

            var colunas = linha.Split(';');

            if (colunas.Length < 10) return null;

            return new Notebook
            {
                Data = colunas[0],
                Usuario = colunas[1],
                ModeloFinal = colunas[2],
                NomeMaquina = colunas[3],
                Processador = colunas[4],
                RAM = colunas[5],
                TotalDisco = colunas[6],
                PercentualUso = colunas[7],
                NumeroSerie = colunas[8],
                SO = colunas[9]
            };
        }
    }
}