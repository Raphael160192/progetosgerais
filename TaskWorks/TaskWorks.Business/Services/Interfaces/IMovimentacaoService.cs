using TaskWorks.Data.Entities;

namespace TaskWorks.Business.Services.Interfaces
{
    public interface IMovimentacaoService
    {
        IEnumerable<Movimentacao> GetAllMovimentacoes();
        Movimentacao GetMovimentacaoById(int id);
        void AddMovimentacao(Movimentacao movimentacao);
        void UpdateMovimentacao(Movimentacao movimentacao);
        void DeleteMovimentacao(int id);
    }
}
