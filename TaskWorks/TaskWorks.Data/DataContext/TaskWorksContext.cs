using Microsoft.EntityFrameworkCore;
using TaskWorks.Data.Entities;

namespace TaskWorks.Data.DataContext
{
    public class TaskWorksContext : DbContext
    {
        public TaskWorksContext(DbContextOptions<TaskWorksContext> options) : base(options) { }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Movimentacao> Movimentacoes { get; set; }
        public DbSet<Entities.Task> Tasks { get; set; }
        public DbSet<DadosProdutividade> DadosProdutividades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entities here
        }
    }
}
