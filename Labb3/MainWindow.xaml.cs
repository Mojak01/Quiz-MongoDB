using Labb3.Dialogs;
using Labb3.ViewModels;
using Labb3.Views;
using System;
using System.Windows;


namespace Labb3
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        private Window _activeWindow;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainWindowViewModel();
            this.DataContext = _vm;

            _vm.CloseWindowReq += OnCloseWindowReq;
            _vm.DeleteReq += OnDeleteReq;
            _vm.ExitReq += OnExitReq;
            _vm.OpenCreateDialogReq += OnOpenCreateDialogReq;
            _vm.ToggleScreenReq += OnToggleScreenReq;
            _vm.OpenImportDialogReq += OnOpenImportDialogReq;

            _vm.ConfigVM.OpenOptionsEvent += OnOpenOptionsEvent;
        }


        private void OnOpenImportDialogReq(object sender, EventArgs e)
        {
            var dialog = new TriviaImport(); 
            OpenDialog(dialog);
        }
        private void OnCloseWindowReq(object sender, EventArgs e)
        {
            if (_activeWindow != null)
            {
                _activeWindow.Close();
                _activeWindow = null;
            }
        }

        private void OnDeleteReq(object sender, EventArgs e)
        {
            string message = $"Are you sure you want to delete '{_vm.ActivePack?.Name}'?";
            var result = MessageBox.Show(message, "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _vm.PerformDelete();
            }
        }

        private void OnExitReq(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you really want to quit?", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void OnOpenCreateDialogReq(object sender, EventArgs e)
        {
            var dialog = new CreateNewPackDialog();
            OpenDialog(dialog);
        }

        private void OnOpenOptionsEvent(object sender, EventArgs e)
        {
            var dialog = new PackOptionsDialog();
            OpenDialog(dialog);
        }

        private void OnToggleScreenReq(object sender, bool isFullScreen)
        {
            if (isFullScreen)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void OpenDialog(Window dialog)
        {
            try
            {
                dialog.DataContext = _vm;
                dialog.Owner = this;
                _activeWindow = dialog;
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}