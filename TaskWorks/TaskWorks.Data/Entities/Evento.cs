using System;

namespace TaskWorks.Data.Entities
{
    public class Evento
    {
        public int EventoId { get; set; }
        public string EventoNome { get; set; }  // Nome do evento
        public string Chamado { get; set; }  // Chamado associado ao evento
        public string Servico { get; set; }
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public string Descricao { get; set; }
    }
}
