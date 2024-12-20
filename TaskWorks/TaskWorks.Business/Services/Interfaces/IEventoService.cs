using TaskWorks.Data.Entities;

namespace TaskWorks.Business.Services.Interfaces
{
    public interface IEventoService
    {
        IEnumerable<Evento> GetAllEventos();
        Evento GetEventoById(int id);
        void AddEvento(Evento evento);
        void UpdateEvento(Evento evento);
        void DeleteEvento(int id);
    }
}
