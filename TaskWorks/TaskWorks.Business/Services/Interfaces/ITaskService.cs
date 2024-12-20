using System.Collections.Generic;
using TaskWorks.Data.Entities;

namespace TaskWorks.Business.Services.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<Data.Entities.Task> GetAllTasks();
        Data.Entities.Task GetTaskById(int id);
        void AddTask(Data.Entities.Task task);
        void UpdateTask(Data.Entities.Task task);
        void DeleteTask(int id);
    }
}
