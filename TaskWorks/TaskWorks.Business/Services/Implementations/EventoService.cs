using TaskWorks.Business.Services.Interfaces;
using TaskWorks.Data.DataContext;
using TaskWorks.Data.Entities;

namespace TaskWorks.Business.Services.Implementations
{
    public class EventoService : IEventoService
    {
        private readonly TaskWorksContext _context;

        public EventoService(TaskWorksContext context)
        {
            _context = context;
        }

        public IEnumerable<Evento> GetAllEventos()
        {
            return _context.Eventos.ToList();
        }

        public Evento GetEventoById(int id)
        {
            return _context.Eventos.Find(id);
        }

        public void AddEvento(Evento evento)
        {
            _context.Eventos.Add(evento);
            _context.SaveChanges();
        }

        public void UpdateEvento(Evento evento)
        {
            _context.Eventos.Update(evento);
            _context.SaveChanges();
        }

        public void DeleteEvento(int id)
        {
            var evento = _context.Eventos.Find(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                _context.SaveChanges();
            }
        }
    }
}
