using Labb3.Converters;
using Labb3.Models;
using System.Collections.ObjectModel;

namespace Labb3.ViewModels
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private QuestionPack _pack;

        public QuestionPackViewModel(QuestionPack pack)
        {
            _pack = pack;
            Questions = new ObservableCollection<Question>(_pack.Questions);
        }

        public string Name
        {
            get
            {
                return _pack.Name;
            }
            set
            {
                _pack.Name = value;
                RaisePropertyChanged();
            }
        }

        public Difficulty Difficulty
        {
            get
            {
                return _pack.Difficulty;
            }
            set
            {
                _pack.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        public int TimeLimitInSeconds
        {
            get
            {
                return _pack.TimeLimitInSeconds;
            }
            set
            {
                _pack.TimeLimitInSeconds = value;
                RaisePropertyChanged();

                RaisePropertyChanged("TimeLimit");
            }
        }
        public int TimeLimit
        {
            get { return TimeLimitInSeconds; }
            set { TimeLimitInSeconds = value; }
        }

        public ObservableCollection<Question> Questions { get; set; }
    }
}