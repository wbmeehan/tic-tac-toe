using System;
using System.Windows;
using TicTacToe.ViewModels;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel mainWindowViewModel =
                new MainWindowViewModel();
            DataContext = mainWindowViewModel;
            if (mainWindowViewModel.CloseAction == null)
            {
                mainWindowViewModel.CloseAction = new Action(Close);
            }
        }
    }
}
