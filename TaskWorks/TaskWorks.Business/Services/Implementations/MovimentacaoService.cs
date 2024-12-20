using System.Collections.Generic;
using System.Linq;
using TaskWorks.Data;
using TaskWorks.Data.Entities;
using TaskWorks.Business.Services.Interfaces;
using TaskWorks.Data.DataContext;

namespace TaskWorks.Business.Services.Implementations
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly TaskWorksContext _context;

        public MovimentacaoService(TaskWorksContext context)
        {
            _context = context;
        }

        public IEnumerable<Movimentacao> GetAllMovimentacoes()
        {
            return _context.Movimentacoes.ToList();
        }

        public Movimentacao GetMovimentacaoById(int id)
        {
            return _context.Movimentacoes.Find(id);
        }

        public void AddMovimentacao(Movimentacao movimentacao)
        {
            _context.Movimentacoes.Add(movimentacao);
            _context.SaveChanges();
        }

        public void UpdateMovimentacao(Movimentacao movimentacao)
        {
            _context.Movimentacoes.Update(movimentacao);
            _context.SaveChanges();
        }

        public void DeleteMovimentacao(int id)
        {
            var movimentacao = _context.Movimentacoes.Find(id);
            if (movimentacao != null)
            {
                _context.Movimentacoes.Remove(movimentacao);
                _context.SaveChanges();
            }
        }
    }
}
