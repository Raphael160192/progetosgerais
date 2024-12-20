using System;

namespace TaskWorks.Data.Entities
{
    public class Movimentacao
    {
        public int MovimentacaoId { get; set; }
        public string Chamado { get; set; }  // Chamado associado à movimentação
        public string Status { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraAlteracao { get; set; }
    }
}