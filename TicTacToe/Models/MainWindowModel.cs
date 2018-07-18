using MvvmFoundation.Wpf;
using System;
using System.Collections.Generic;

namespace TicTacToe.Models
{
    /// <summary>
    /// Tile states
    /// </summary>
    public enum TileState { Default, X, O };
    class MainWindowModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// Game grid dimensions
        /// </summary>
        private const int GridSize = 3;

        /// <summary>
        /// Game grid
        /// </summary>
        private TileState[,] _grid = new TileState[GridSize, GridSize];

        /// <summary>
        /// Move count
        /// </summary>
        private int _moveCount;

        private bool _isGameOver;


        #endregion

        #region Properties

        public Queue<string> WinningTiles { get; set; } = new Queue<string>();

        public TileState MoveLetter
        {
            get
            {
                if (_moveCount % 2 == 0)
                {
                    return TileState.O;
                } else
                {
                    return TileState.X;
                }
            }
        }

        public bool IsGameOver {
            get
            {
                return _isGameOver;
            }
        }

        /// <summary>
        /// Game grid dimensions
        /// </summary>
        public TileState[,] Grid
        {
            get
            {
                return _grid;
            }

            set
            {
                Grid = value;
            }
        }

        #endregion

        public bool MakeMove(string tilePosition)
        {

            int row = tilePosition[0] - '0';
            int column = tilePosition[1] - '0';

            if (IsGameOver)
            {
                ResetGrid();
            }

            if (Grid[row, column] != TileState.Default)
            {
                /* Invalid move */
                return false;
            }

            _moveCount++;
            Grid[row, column] = MoveLetter;

            if (IsWinner(row, column) || IsDraw())
            {
                _isGameOver = true;
            }

            return true;
        }

        public void ResetGrid()
        {
            _moveCount = 0;
            Array.Clear(Grid, 0, Grid.Length);
            _isGameOver = false;
        }

        private bool IsWinner(int row, int column)
        {
            
            if (IsColumnWin(column))
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(String.Format("{0}{1}", i, column));
                }
                return true;
            }

            if (IsRowWin(row))
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(String.Format("{0}{1}", row, i));
                }
                return true;
            }

            if (row == column && IsMainDiagonalWin())
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(String.Format("{0}{1}", i, i));
                }
                return true;
            }

            if (((row + column) == GridSize - 1) &&
                IsAntiDiagonalWin())
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(String.Format("{0}{1}", i,
                        GridSize - 1 - i));
                }
                return true;
            }

            return false;
        }

        private bool IsColumnWin(int column)
        {
            for (int i = 0; i < GridSize; i++) {
                if (_grid[i, column] != MoveLetter)
                {
                    break;
                }
                if (i == GridSize - 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRowWin(int row)
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (_grid[row, i] != MoveLetter)
                {
                    break;
                }
                if (i == GridSize - 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMainDiagonalWin()
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] != MoveLetter)
                {
                    break;
                }
                if (i == GridSize - 1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAntiDiagonalWin()
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - 1 - i, i] != MoveLetter)
                {
                    break;
                }
                if (i == GridSize - 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsDraw()
        {
            return _moveCount == Math.Pow(GridSize, 2);
        }

    }
}
