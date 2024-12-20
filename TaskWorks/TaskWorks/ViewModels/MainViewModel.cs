using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TaskWorks.Data.Entities; // Certifique-se de adicionar este using

namespace TaskWorks.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Evento> Eventos { get; set; }
        public ObservableCollection<Data.Entities.Task> Tasks { get; set; }

        private Evento _selectedEvento;
        public Evento SelectedEvento
        {
            get { return _selectedEvento; }
            set
            {
                _selectedEvento = value;
                OnPropertyChanged(nameof(SelectedEvento));
            }
        }

        public ICommand AddTaskCommand { get; set; }

        public MainViewModel()
        {
            Eventos = new ObservableCollection<Evento>();
            Tasks = new ObservableCollection<Data.Entities.Task>();
            AddTaskCommand = new RelayCommand(AddTask);

            // Carregar dados do banco de dados, se necessário
        }

        private void AddTask()
        {
            // Lógica para adicionar tarefa
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Definição de RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
