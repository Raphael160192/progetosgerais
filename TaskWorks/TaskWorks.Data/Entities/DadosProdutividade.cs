public class DadosProdutividade
{
    public int Mes { get; set; }
    public int DiasUteis { get; set; }
    public int DiasNaotrabalhado { get; set; }
    public int MetaQuantidadeHoraMes { get; set; }
    public decimal ValorHoraTrabalhada { get; set; }
    public int HorasTrabalhadas { get; set; } // Adicionado para rastrear as horas trabalhadas no mês
    public decimal ValorAcumulado { get; set; } // Adicionado para rastrear o valor acumulado no mês
}
