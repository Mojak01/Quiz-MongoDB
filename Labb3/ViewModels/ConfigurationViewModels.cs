using System;
using System.Linq;
using Labb3.Command;
using Labb3.Models;

namespace Labb3.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        public MainWindowViewModel MainVM { get; set; }

        public QuestionPackViewModel CurrentPack
        {
            get
            {
                return MainVM.ActivePack;
            }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                _canDelete = value;
                RaisePropertyChanged();
            }
        }

        private bool _configVisible;
        public bool IsConfigView
        {
            get { return _configVisible; }
            set
            {
                _configVisible = value;
                RaisePropertyChanged();
            }
        }

        private Question _selectedQ;
        public Question SelectedQ
        {
            get { return _selectedQ; }
            set
            {
                _selectedQ = value;
                RemoveQCmd.RaiseCanExecuteChanged();
                RaisePropertyChanged();
                UpdateVis();
            }
        }

        private bool _detailsVisible;
        public bool ShowDetails
        {
            get { return _detailsVisible; }
            set
            {
                _detailsVisible = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler OpenOptionsEvent;

        public DelegateCommand NewQCmd { get; set; }
        public DelegateCommand RemoveQCmd { get; set; }
        public DelegateCommand OptionsCmd { get; set; }
        public DelegateCommand GoToConfigCmd { get; set; }

        public ConfigurationViewModel(MainWindowViewModel main)
        {
            this.MainVM = main;

            CanDelete = false;
            IsConfigView = true;

            NewQCmd = new DelegateCommand(DoAdd, CanAdd);
            RemoveQCmd = new DelegateCommand(DoRemove, CanRemove);
            OptionsCmd = new DelegateCommand(DoOptions, CanOptions);
            GoToConfigCmd = new DelegateCommand(DoEnterConfig, CanEnterConfig);

            if (CurrentPack != null && CurrentPack.Questions.Count > 0)
            {
                SelectedQ = CurrentPack.Questions.FirstOrDefault();
            }

            UpdateVis();
        }

        private void DoAdd(object obj)
        {
            if (CurrentPack != null)
            {
                var newQ = new Question("New Question", "", "", "", "");
                CurrentPack.Questions.Add(newQ);

                if (CurrentPack.Questions.Count > 0)
                {
                    SelectedQ = CurrentPack.Questions.Last();
                }

                RefreshAllCommands();
                UpdateVis();
                MainVM.SaveToJsonAsync();
            }
        }

        private bool CanAdd(object obj)
        {
            return IsConfigView;
        }

        private void DoRemove(object obj)
        {
            if (CurrentPack != null && SelectedQ != null)
            {
                CurrentPack.Questions.Remove(SelectedQ);
                RefreshAllCommands();
                UpdateVis();
                MainVM.SaveToJsonAsync();
            }
        }

        private bool CanRemove(object obj)
        {
            bool hasItems = false;
            if (CurrentPack != null && CurrentPack.Questions.Count > 0)
            {
                hasItems = true;
            }

            CanDelete = hasItems;

            if (IsConfigView && SelectedQ != null && hasItems)
            {
                return true;
            }
            return false;
        }

        private void DoOptions(object obj)
        {
            if (OpenOptionsEvent != null)
            {
                OpenOptionsEvent(this, EventArgs.Empty);
            }
            MainVM.SaveToJsonAsync();
        }

        private bool CanOptions(object obj)
        {
            return IsConfigView;
        }

        private void UpdateVis()
        {
            if (CurrentPack != null && CurrentPack.Questions.Count > 0 && SelectedQ != null)
            {
                ShowDetails = true;
            }
            else
            {
                ShowDetails = false;
            }
        }

        private void DoEnterConfig(object obj)
        {
            MainVM.PlayerViewModel._timer.Stop();

            IsConfigView = true;

            MainVM.PlayerViewModel.IsPlayerModeVisible = false;
            MainVM.PlayerViewModel.IsResultModeVisible = false;

            MainVM.PlayerViewModel.IsFinished = false;
            MainVM.PlayerViewModel.IsPlaying = false;

            RefreshAllCommands();
        }

        private bool CanEnterConfig(object obj)
        {
            if (IsConfigView == true)
            {
                return false;
            }
            return true;
        }

        private void RefreshAllCommands()
        {
            NewQCmd.RaiseCanExecuteChanged();
            RemoveQCmd.RaiseCanExecuteChanged();
            OptionsCmd.RaiseCanExecuteChanged();

            MainVM.PlayerViewModel.SwitchToPlayModeCommand.RaiseCanExecuteChanged();
        }

        public void NotifyCurrentPackChanged()
        {
            RaisePropertyChanged(nameof(CurrentPack));

            if (CurrentPack != null && CurrentPack.Questions.Count > 0)
            {
                SelectedQ = CurrentPack.Questions.FirstOrDefault();
            }
            else
            {
                SelectedQ = null;
            }

            UpdateVis();
        }
    }
}