using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Labb3.Command;
using Labb3.Models;

namespace Labb3.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {

        string folderPath;
        string filePath;
        public ConfigurationViewModel ConfigVM { get; set; }
        public PlayerViewModel PlayerViewModel { get; set; }

        public PlayerViewModel PlayerVM => PlayerViewModel;

        private QuestionPackViewModel _activePack;
        public QuestionPackViewModel ActivePack
        {
            get { return _activePack; }
            set
            {
                _activePack = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanDeletePack));
                ConfigVM?.NotifyCurrentPackChanged();
            }
        }

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

        public ObservableCollection<QuestionPackViewModel> PackList => Packs;

        public bool CanDeletePack => ActivePack != null && Packs.Count > 0;

        public event EventHandler CloseWindowReq;
        public event EventHandler DeleteReq;
        public event EventHandler ExitReq;
        public event EventHandler OpenCreateDialogReq;
        public event EventHandler<bool> ToggleScreenReq;
        public event EventHandler OpenImportDialogReq;  


        public DelegateCommand FullScreenCmd { get; set; }
        public DelegateCommand SaveCmd { get; set; }
        public DelegateCommand ImportCmd { get; set; }
        public DelegateCommand OpenCreateCmd { get; set; }
        public DelegateCommand RemovePackCmd { get; set; }
        public DelegateCommand SelectPackCmd { get; set; }
        public DelegateCommand QuitCmd { get; set; }

        public DelegateCommand CancelCmd { get; set; }
        public DelegateCommand SavePackCmd { get; set; }

        private QuestionPackViewModel _thePack;
        public QuestionPackViewModel ThePack
        {
            get { return _thePack; }
            set
            {
                _thePack = value;
                RaisePropertyChanged();
            }
        }

        private bool _isFullScreen;

        public MainWindowViewModel()
        {
            Packs = new ObservableCollection<QuestionPackViewModel>();

            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Labb3Quiz");
            filePath = Path.Combine(folderPath, "data.json");

            FullScreenCmd = new DelegateCommand(DoToggleFullScreen);
            SaveCmd = new DelegateCommand(DoSave);
            ImportCmd = new DelegateCommand(DoImport);
            OpenCreateCmd = new DelegateCommand(DoOpenCreate);
            RemovePackCmd = new DelegateCommand(DoRemovePack, CanRemovePack);
            SelectPackCmd = new DelegateCommand(DoSelectPack);
            QuitCmd = new DelegateCommand(DoQuit);
            CancelCmd = new DelegateCommand(DoCancel);
            SavePackCmd = new DelegateCommand(DoSavePack);

            ConfigVM = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);

            LoadFromJson();
        }

        private void DoToggleFullScreen(object obj)
        {
            _isFullScreen = !_isFullScreen;
            ToggleScreenReq?.Invoke(this, _isFullScreen);
        }

        private void DoSave(object obj)
        {
            SaveToJsonAsync();
        }

        private void DoImport(object obj)
        {
            OpenImportDialogReq?.Invoke(this, EventArgs.Empty);
        }

        private void DoOpenCreate(object obj)
        {
            
            ThePack = new QuestionPackViewModel(new QuestionPack("New Pack"));
            OpenCreateDialogReq?.Invoke(this, EventArgs.Empty);
        }

        private void DoCancel(object obj)
        {
            
            ThePack = null;
            CloseWindowReq?.Invoke(this, EventArgs.Empty);
        }

        private void DoSavePack(object obj)
        {
            
            if (ThePack != null)
            {
                Packs.Add(ThePack);
                ActivePack = ThePack;
                ThePack = null;
                CloseWindowReq?.Invoke(this, EventArgs.Empty);
                SaveToJsonAsync();
            }
        }

        private void DoRemovePack(object obj)
        {
            DeleteReq?.Invoke(this, EventArgs.Empty);
        }

        private bool CanRemovePack(object obj)
        {
            return ActivePack != null && Packs.Count > 0;
        }

        private void DoSelectPack(object obj)
        {
            if (obj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
                ConfigVM.NotifyCurrentPackChanged();
            }
        }

        private void DoQuit(object obj)
        {
            ExitReq?.Invoke(this, EventArgs.Empty);
        }

        public void PerformDelete()
        {
            if (ActivePack != null && Packs.Contains(ActivePack))
            {
                Packs.Remove(ActivePack);

                if (Packs.Count > 0)
                {
                    ActivePack = Packs[0];
                }
                else
                {
                    ActivePack = null;
                }

                RemovePackCmd.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(CanDeletePack));
                SaveToJsonAsync();
            }
        }

        public void RaiseCloseWindowReq()
        {
            CloseWindowReq?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseDeleteReq()
        {
            DeleteReq?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseExitReq()
        {
            ExitReq?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseOpenCreateDialogReq()
        {
            OpenCreateDialogReq?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseToggleScreenReq(bool isFull)
        {
            ToggleScreenReq?.Invoke(this, isFull);
        }

        public async void SaveToJsonAsync()
        {

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var list = new List<QuestionPack>();
                foreach (var vm in Packs)
                {
                    var pack = new QuestionPack(vm.Name, vm.Difficulty, vm.TimeLimitInSeconds);
                    foreach (var q in vm.Questions)
                    {
                        pack.Questions.Add(q);
                    }
                    list.Add(pack);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(list, options);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch
            {
            }
        }
        private async void LoadFromJson()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    var loaded = JsonSerializer.Deserialize<List<QuestionPack>>(json);

                    if (loaded != null && loaded.Count > 0)
                    {
                        Packs.Clear();
                        foreach (var p in loaded)
                        {
                            Packs.Add(new QuestionPackViewModel(p));
                        }
                        ActivePack = Packs[0];
                    }
                }
            }
            catch
            {
            }

            if (Packs.Count == 0)
            {
                var def = new QuestionPack("My First Pack");
                Packs.Add(new QuestionPackViewModel(def));
                ActivePack = Packs[0];
            }
        }

    }

}
