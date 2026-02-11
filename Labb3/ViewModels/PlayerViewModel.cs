using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Labb3.Command;
using Labb3.Models;

namespace Labb3.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        public MainWindowViewModel ParentVM { get; set; }

        public DispatcherTimer GameTimer;

        private int _seconds;
        public int SecondsLeft
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                RaisePropertyChanged();
            }
        }

        private string _currQ;
        public string QText
        {
            get { return _currQ; }
            set { _currQ = value; RaisePropertyChanged(); }
        }

        private string _info;
        public string TopInfo
        {
            get { return _info; }
            set { _info = value; RaisePropertyChanged(); }
        }

        private string _finalRes;
        public string EndText
        {
            get { return _finalRes; }
            set { _finalRes = value; RaisePropertyChanged(); }
        }

        private List<string> _mixAnswers;
        public List<string> CurrentAnswers
        {
            get { return _mixAnswers; }
            set { _mixAnswers = value; RaisePropertyChanged(); }
        }

        private bool[] _green;
        public bool[] ShowGreen
        {
            get { return _green; }
            set { _green = value; RaisePropertyChanged(); }
        }

        private bool[] _red;
        public bool[] ShowRed
        {
            get { return _red; }
            set { _red = value; RaisePropertyChanged(); }
        }

        private int _rightIndex;
        private int _currIndex;
        private int _score;
        private bool _buttonsActive;
        private List<Question> _gameQuestions;

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged();
            }
        }

        private bool _isFinished;
        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                RaisePropertyChanged();
            }
        }

        private bool _canStart;
        public bool CanStart
        {
            get { return _canStart; }
            set
            {
                _canStart = value;
                RaisePropertyChanged();
            }
        }

        private bool _isPlayerMode;
        public bool IsPlayerModeVisible
        {
            get { return _isPlayerMode; }
            set
            {
                _isPlayerMode = value;
                RaisePropertyChanged();
            }
        }

        private bool _isResultMode;
        public bool IsResultModeVisible
        {
            get { return _isResultMode; }
            set
            {
                _isResultMode = value;
                RaisePropertyChanged();
            }
        }

        public DispatcherTimer _timer;

        public DelegateCommand StartGameCmd { get; set; }
        public DelegateCommand PickAnswerCmd { get; set; }
        public DelegateCommand SwitchToPlayModeCommand { get; set; }

        public PlayerViewModel(MainWindowViewModel main)
        {
            ParentVM = main;
            _buttonsActive = true;

            IsPlaying = false;
            IsFinished = false;
            CanStart = false;

            StartGameCmd = new DelegateCommand(DoStart, CanStartGame);
            PickAnswerCmd = new DelegateCommand(DoPick, CanPick);
            SwitchToPlayModeCommand = new DelegateCommand(DoSwitchToPlay, CanSwitchToPlay);

            ShowGreen = new bool[] { false, false, false, false };
            ShowRed = new bool[] { false, false, false, false };

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            GameTimer = _timer;
        }

        private void DoStart(object obj)
        {
            CanStart = false;
            IsPlaying = true;
            IsFinished = false;

            ParentVM.ConfigVM.IsConfigView = false;

            var rawList = ParentVM.ActivePack.Questions.ToList();
            var rnd = new Random();
            _gameQuestions = rawList.OrderBy(x => rnd.Next()).ToList();

            _currIndex = 0;
            _score = 0;

            LoadLevel();
        }

        private bool CanStartGame(object obj)
        {
            if (ParentVM.ActivePack == null) return false;

            bool hasQs = ParentVM.ActivePack.Questions.Count > 0;
            bool notPlaying = !IsPlaying;

            CanStart = notPlaying && hasQs;
            return CanStart;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (SecondsLeft > 0)
            {
                SecondsLeft--;
            }
            else
            {
                GameTimer.Stop();
                HandleAnswer(null);
            }
        }

        private void LoadLevel()
        {
            SecondsLeft = ParentVM.ActivePack.TimeLimitInSeconds;
            GameTimer.Start();

            for (int i = 0; i < 4; i++)
            {
                ShowGreen[i] = false;
                ShowRed[i] = false;
            }
            RefreshUI();

            if (_currIndex < _gameQuestions.Count)
            {
                TopInfo = "Question " + (_currIndex + 1) + " / " + _gameQuestions.Count;

                var q = _gameQuestions[_currIndex];
                QText = q.Query;

                var tempList = new List<string>();
                tempList.Add(q.CorrectAnswer);
                tempList.AddRange(q.IncorrectAnswers);

                var rnd = new Random();
                CurrentAnswers = tempList.OrderBy(x => rnd.Next()).ToList();

                _rightIndex = CurrentAnswers.IndexOf(q.CorrectAnswer);

                _currIndex++;
            }
            else
            {
                GoToResults();
            }
        }

        private async void DoPick(object obj)
        {
            if (obj == null) return;
            string indexStr = obj.ToString();
            int index = int.Parse(indexStr);

            HandleAnswer(index);
        }

        private async void HandleAnswer(int? playerChoice)
        {
            GameTimer.Stop();
            _buttonsActive = false;
            PickAnswerCmd.RaiseCanExecuteChanged();

            if (playerChoice.HasValue)
            {
                if (playerChoice.Value == _rightIndex)
                {
                    _score++;
                    ShowGreen[playerChoice.Value] = true;
                }
                else
                {
                    ShowRed[playerChoice.Value] = true;
                }
            }

            ShowGreen[_rightIndex] = true;
            RefreshUI();

            await Task.Delay(2000);

            _buttonsActive = true;
            PickAnswerCmd.RaiseCanExecuteChanged();

            LoadLevel();
        }

        private bool CanPick(object obj)
        {
            return _buttonsActive;
        }

        private void GoToResults()
        {
            GameTimer.Stop();
            IsFinished = true;
            IsPlaying = false;

            EndText = "You got " + _score + " correct out of " + _gameQuestions.Count;

            RefreshUI();
        }

        private void RefreshUI()
        {
            RaisePropertyChanged("ShowGreen");
            RaisePropertyChanged("ShowRed");

            StartGameCmd.RaiseCanExecuteChanged();
            ParentVM.ConfigVM.NewQCmd.RaiseCanExecuteChanged();
            ParentVM.ConfigVM.OptionsCmd.RaiseCanExecuteChanged();
            ParentVM.ConfigVM.RemoveQCmd.RaiseCanExecuteChanged();
            ParentVM.ConfigVM.GoToConfigCmd.RaiseCanExecuteChanged();
        }

        private void DoSwitchToPlay(object obj)
        {
            IsPlayerModeVisible = true;
            IsResultModeVisible = false;
        }

        private bool CanSwitchToPlay(object obj)
        {
            return !IsPlaying;
        }
    }
}