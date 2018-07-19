using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using TicTacToe.Models;

namespace TicTacToe.ViewModels
{
    /// <summary>
    /// Tic-tac-toe view model.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        #region Fields

        /* Tic-tac-toe model */
        private MainWindowModel _ticTacToeModel = new MainWindowModel();

        /* Mode checkbox states */
        private bool _easyModeChecked = true;
        private bool _mediumModeChecked;
        private bool _hardModeChecked;
        private bool _friendModeChecked;

        /* Click commands */
        private ICommand _tileClickCommand;
        private ICommand _resetModeClickCommand;
        private ICommand _exitClickCommand;

        #endregion

        #region Public Properties/Commands

        /* Close window action */
        public Action CloseAction { get; set; }

        /* Title */
        public string Title
        {
            get
            {
                return "Tic-tac-toe: " + Mode;
            }
        }

        #region Tile labels
        public string Tile00Label { get; set; }

        public string Tile01Label { get; set; }

        public string Tile02Label { get; set; }

        public string Tile10Label { get; set; }

        public string Tile11Label { get; set; }

        public string Tile12Label { get; set; }

        public string Tile20Label { get; set; }

        public string Tile21Label { get; set; }

        public string Tile22Label { get; set; }
        #endregion

        #region Tile label colours

        public string Tile00Colour { get; set; } = "Yellow";

        public string Tile01Colour { get; set; } = "Yellow";

        public string Tile02Colour { get; set; } = "Yellow";

        public string Tile10Colour { get; set; } = "Yellow";

        public string Tile11Colour { get; set; } = "Yellow";

        public string Tile12Colour { get; set; } = "Yellow";

        public string Tile20Colour { get; set; } = "Yellow";

        public string Tile21Colour { get; set; } = "Yellow";

        public string Tile22Colour { get; set; } = "Yellow";

        #endregion

        #region Mode checkboxes
        public bool EasyModeChecked
        {
            get
            {
                return _easyModeChecked;
            }

            set
            {
                if (value == true)
                {
                    Mode = "Easy";
                    _easyModeChecked = true;
                    HardModeChecked = false;
                    MediumModeChecked = false;
                    FriendModeChecked = false;
                }
                else
                {
                    _easyModeChecked = false;
                }
            }
        }

        public bool MediumModeChecked
        {
            get
            {
                return _mediumModeChecked;
            }

            set
            {
                if (value == true)
                {
                    Mode = "Medium";
                    _mediumModeChecked = true;
                    EasyModeChecked = false;
                    HardModeChecked = false;
                    FriendModeChecked = false;
                }
                else
                {
                    _mediumModeChecked = false;
                }
            }
        }
  
        public bool HardModeChecked
        {
            get
            {
                return _hardModeChecked;
            }

            set
            {
                if (value == true)
                {
                    Mode = "Hard";
                    _hardModeChecked = true;
                    EasyModeChecked = false;
                    MediumModeChecked = false;
                    FriendModeChecked = false;
                }
                else
                {
                    _hardModeChecked = false;
                }
            }
        }

        public bool FriendModeChecked
        {
            get
            {
                return _friendModeChecked;
            }

            set
            {
                if (value == true)
                {
                    Mode = "Play against a friend";
                    _friendModeChecked = true;
                    EasyModeChecked = false;
                    MediumModeChecked = false;
                    HardModeChecked = false;
                }
                else
                {
                    _friendModeChecked = false;
                }
            }
        }

        #endregion

        #region Tic-tac-toe model variables

        public string Mode
        {
            get
            {
                return _ticTacToeModel.Mode;
            }

            set
            {
                _ticTacToeModel.Mode = value;
            }
        }

        public TileState[,] Grid
        {
            get
            {
                return _ticTacToeModel.Grid;
            }
        }

        public Queue<string> WinningTiles
        {
            get
            {
                return _ticTacToeModel.WinningTiles;
            }
        }

        public bool IsGameOver
        {
            get
            {
                return _ticTacToeModel.IsGameOver;
            }
        }

        #endregion

        #region Click commands

        public ICommand TileClickCommand
        {
            get
            {
                if (_tileClickCommand == null)
                {
                    _tileClickCommand = new RelayCommand<string>(TileClick);

                }
                return _tileClickCommand;
            }

        }

        public ICommand ResetModeClickCommand
        {
            get
            {
                if (_resetModeClickCommand == null)
                {
                    _resetModeClickCommand = new RelayCommand(ResetModeClick);

                }
                return _resetModeClickCommand;
            }

        }

        public ICommand ExitClickCommand
        {
            get
            {
                if (_exitClickCommand == null)
                {
                    _exitClickCommand = new RelayCommand(CloseAction);

                }
                return _exitClickCommand;
            }

        }

        #endregion

        #endregion

        #region Private Helpers

        /// <summary>
        /// Reset game state
        /// </summary>
        private void ResetModeClick()
        {
            _ticTacToeModel.ResetGrid();
            UpdateTileText();
            ResetTileTextColour();
        }

        /// <summary>
        /// Register move on tile.
        /// </summary>
        /// <param name="tilePosition">Position of the move tile.</param>
        private void TileClick(string tilePosition)
        {
            if (IsGameOver)
            {
                ResetTileTextColour();
            }

            if (_ticTacToeModel.MakeHumanMove(tilePosition) == false)
            {
                /* Invalid move */
                return;
            }

            UpdateTileText();

            if (IsGameOver)
            {
                while (WinningTiles.Count != 0)
                {
                    SetTileTextColour(WinningTiles.Dequeue(), "LimeGreen");
                }
            } 
            
        }

        /// <summary>
        /// Refresh tile labels.
        /// </summary>
        private void UpdateTileText()
        {
            Tile00Label = (Grid[0, 0] != TileState.Default) ?
                Grid[0, 0].ToString() : string.Empty;
            Tile01Label = (Grid[0, 1] != TileState.Default) ?
                Grid[0, 1].ToString() : string.Empty;
            Tile02Label = (Grid[0, 2] != TileState.Default) ?
                Grid[0, 2].ToString() : string.Empty;
            Tile10Label = (Grid[1, 0] != TileState.Default) ?
                Grid[1, 0].ToString() : string.Empty;
            Tile11Label = (Grid[1, 1] != TileState.Default) ?
                Grid[1, 1].ToString() : string.Empty;
            Tile12Label = (Grid[1, 2] != TileState.Default) ?
                Grid[1, 2].ToString() : string.Empty;
            Tile20Label = (Grid[2, 0] != TileState.Default) ?
                Grid[2, 0].ToString() : string.Empty;
            Tile21Label = (Grid[2, 1] != TileState.Default) ?
                Grid[2, 1].ToString() : string.Empty;
            Tile22Label = (Grid[2, 2] != TileState.Default) ?
                Grid[2, 2].ToString() : string.Empty;
        }

        /// <summary>
        /// Refresh all tile label colours.
        /// </summary>
        private void ResetTileTextColour()
        {
            Tile00Colour = "Yellow";
            Tile01Colour = "Yellow";
            Tile02Colour = "Yellow";
            Tile10Colour = "Yellow";
            Tile11Colour = "Yellow";
            Tile12Colour = "Yellow";
            Tile20Colour = "Yellow";
            Tile21Colour = "Yellow";
            Tile22Colour = "Yellow";
        }

        /// <summary>
        /// Set the colour of a tile's text.
        /// </summary>
        /// <param name="tilePosition">Position of the specified tile.</param>
        /// <param name="tileColour">Colour to set the specified tile to.</param>
        private void SetTileTextColour(string tilePosition, string tileColour)
        {
            switch (tilePosition)
            {
                case "00":

                    Tile00Colour = tileColour;
                    break;
                case "01":

                    Tile01Colour = tileColour;
                    break;
                case "02":

                    Tile02Colour = tileColour;
                    break;
                case "10":

                    Tile10Colour = tileColour;
                    break;
                case "11":

                    Tile11Colour = tileColour;
                    break;
                case "12":

                    Tile12Colour = tileColour;
                    break;
                case "20":

                    Tile20Colour = tileColour;
                    break;
                case "21":

                    Tile21Colour = tileColour;
                    break;
                case "22":

                    Tile22Colour = tileColour;
                    break;
                default:

                    break;
            }
        }

        #endregion

    }
}
