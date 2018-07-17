using System;
using System.Text;
using System.Windows.Input;
using TicTacToe.Models;
using MvvmFoundation.Wpf;

namespace TicTacToe.ViewModels
{
    class MainWindowViewModel : ObservableObject
    {
        #region Fields

        /* Tic-tac-toe model */
        private MainWindowModel _ticTacToeModel = new MainWindowModel();

        #endregion
    }
}
