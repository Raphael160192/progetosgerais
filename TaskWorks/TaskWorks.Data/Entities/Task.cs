using System;

namespace TaskWorks.Data.Entities
{
    public class Task
    {
        public int TaskfaId { get; set; }
        public string Chamado { get; set; }  // Chamado associado à tarefa
        public TimeSpan TempoEstimado { get; set; }
        public string Tarefa { get; set; }
        public string DescricaoTarefa { get; set; }
    }
}