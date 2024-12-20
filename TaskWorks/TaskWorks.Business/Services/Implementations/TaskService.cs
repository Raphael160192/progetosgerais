using System.Collections.Generic;
using System.Linq;
using TaskWorks.Data;
using TaskWorks.Data.Entities;
using TaskWorks.Business.Services.Interfaces;
using TaskWorks.Data.DataContext;

namespace TaskWorks.Business.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly TaskWorksContext _context;

        public TaskService(TaskWorksContext context)
        {
            _context = context;
        }

        public IEnumerable<Data.Entities.Task> GetAllTasks()
        {
            return _context.Tasks.ToList();
        }

        public Data.Entities.Task GetTaskById(int id)
        {
            return _context.Tasks.Find(id);
        }

        public void AddTask(Data.Entities.Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(Data.Entities.Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}
