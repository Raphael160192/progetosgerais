namespace TaskWorks.Business.Services.Interfaces
{
    public interface IDadosProdutividadeService
    {
        // Métodos para operações relacionadas a DadosProdutividade
        Task<DadosProdutividade> GetByIdAsync(int id);
        Task<List<DadosProdutividade>> GetAllAsync();
        Task AddAsync(DadosProdutividade dadosProdutividade);
        Task UpdateAsync(DadosProdutividade dadosProdutividade);
        Task DeleteAsync(int id);
    }
}
