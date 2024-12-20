using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskWorks.Business.Services.Interfaces;
using TaskWorks.Data;
using TaskWorks.Data.DataContext;
using TaskWorks.Data.Entities;

namespace TaskWorks.Business.Services.Implementations
{
    public class DadosProdutividadeService : IDadosProdutividadeService
    {
        private readonly TaskWorksContext _context;

        public DadosProdutividadeService(TaskWorksContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DadosProdutividade> GetByIdAsync(int id)
        {
            return await _context.DadosProdutividades.FindAsync(id);
        }

        public async Task<List<DadosProdutividade>> GetAllAsync()
        {
            return await _context.DadosProdutividades.ToListAsync();
        }

        public async Data.Entities.Task AddAsync(DadosProdutividade dadosProdutividade)
        {
            _context.DadosProdutividade.Add(dadosProdutividade);
            await _context.SaveChangesAsync();
        }

        public async Data.Entities.Task UpdateAsync(DadosProdutividade dadosProdutividade)
        {
            _context.DadosProdutividade.Update(dadosProdutividade);
            await _context.SaveChangesAsync();
        }

        public async Data.Entities.Task DeleteAsync(int id)
        {
            var dadosProdutividade = await _context.DadosProdutividade.FindAsync(id);
            if (dadosProdutividade != null)
            {
                _context.DadosProdutividade.Remove(dadosProdutividade);
                await _context.SaveChangesAsync();
            }
        }
    }
}
