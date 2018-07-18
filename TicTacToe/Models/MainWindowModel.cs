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

        public string Mode { get; set; } = "Play against a friend";
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

        public TileState ComputerMoveLetter
        {
            get
            {
                if (_moveCount % 2 == 0)
                {
                    return TileState.X;
                }
                else
                {
                    return TileState.O;
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
            else
            {
                MakeComputerMove();
            }

            return true;
        }

        private void MakeComputerMove()
        {
            switch (Mode)
            {
                case "Easy":

                    EasyComputerMove();
                    break;
                case "Medium":

                    MediumComputerMove();
                    break;
                case "Hard":

                    HardComputerMove();
                    break;
                case "Play against a friend":

                    break;
                default:

                    break;
            }
        }

        private void EasyComputerMove()
        {
            Random rnd = new Random();
            int positionID;
            if (_moveCount == 1)
            {
                positionID = rnd.Next(1, GridSize * GridSize - _moveCount);
            }
            else
            {
                positionID = rnd.Next(1, GridSize * GridSize - _moveCount + 1);
            }
            int freeTileCount = 0;
            int row = 0;
            int column = 0;
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] != TileState.Default ||
                        (_moveCount == 1 && i == 1 && j == 1))
                    {
                        continue;
                    }
                    freeTileCount++;
                    if (freeTileCount == positionID)
                    {
                        row = i;
                        column = j;
                    }

                }
            }

            ComputerMove(row, column);
        }

        private void ComputerMove(int row, int column)
        {
            _moveCount++;
            Grid[row, column] = MoveLetter;

            if (IsWinner(row, column) || IsDraw())
            {
                _isGameOver = true;
            }
        }

        private void MediumComputerMove()
        {
            if (AttemptThreeInARow() == true)
            {
                return;
            }


            if (BlockThreeInARow() == true)
            {
                //block
                return;
            }

            EasyComputerMove();
        }

        private void HardComputerMove()
        {
            if (AttemptThreeInARow() == true) {
                return;
             }


            if (BlockThreeInARow() == true) {
                //block
                return;
            }

            if (AttemptFork() == true)
            {
                //fork
                return;
            }

            if (BlockFork() == true)
            {
                //block fork
                return;
            }

            if (Grid[1, 1] == TileState.Default)
            {
                ComputerMove(1, 1);
                return;
            }
            if (AttemptCornerMove() == true)
            {
                return;
            }

            EasyComputerMove();
        }

        private bool AttemptFork()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] == TileState.Default)
                    {
                        int NonBlockedLinesCount = 0;
                        if (IsCandidateForkRow(i) == true)
                        {
                            NonBlockedLinesCount++;
                        }
                        if (IsCandidateForkColumn(j) == true)
                        {
                            NonBlockedLinesCount++;
                        }
                        if (InMainDiagonal(i, j) == true &&
                            IsCandidateForkMainDiagonal())
                        {
                            NonBlockedLinesCount++;
                        }
                        if (InAntiDiagonal(i, j) == true &&
                            IsCandidateForkAntiDiagonal())
                        {
                            NonBlockedLinesCount++;
                        }

                        if (NonBlockedLinesCount >= 2)
                        {
                            ComputerMove(i, j);
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        private bool BlockFork()
        {
            int row = 0;
            int column = 0;
            int candidateTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] == TileState.Default)
                    {
                        int NonBlockedLinesCount = 0;
                        if (IsCandidateBlockForkRow(i) == true)
                        {
                            NonBlockedLinesCount++;
                        }
                        if (IsCandidateBlockForkColumn(j) == true)
                        {
                            NonBlockedLinesCount++;
                        }
                        if (InMainDiagonal(i, j) == true &&
                            IsCandidateBlockForkMainDiagonal())
                        {
                            NonBlockedLinesCount++;
                        }
                        if (InAntiDiagonal(i, j) == true &&
                            IsCandidateBlockForkAntiDiagonal())
                        {
                            NonBlockedLinesCount++;
                        }

                        if (NonBlockedLinesCount >= 2)
                        {
                            row = i;
                            column = j;
                            candidateTileCount++;
                        }

                    }
                }
            }
            return false;
        }

        private bool InMainDiagonal(int row, int column)
        {
            return row == column;
        }

        private bool InAntiDiagonal(int row, int column)
        {
            return row == GridSize - column - 1;
        }

        private bool IsCandidateForkRow(int row)
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[row, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[row, i] == MoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateBlockForkRow(int row)
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[row, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[row, i] == ComputerMoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateBlockForkColumn(int column)
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, column] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, column] == ComputerMoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateForkColumn(int column)
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, column] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, column] == MoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateForkAntiDiagonal()
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[GridSize - i - 1, i] == MoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateBlockForkAntiDiagonal()
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[GridSize - i - 1, i] == ComputerMoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateForkMainDiagonal()
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, i] == MoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool IsCandidateBlockForkMainDiagonal()
        {
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, i] == ComputerMoveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        private bool AttemptCornerMove()
        {
            // check opposite corners to opponent
            if (Grid[0, 0] == MoveLetter &&
                Grid[GridSize - 1, GridSize - 1] == TileState.Default)
            {
                ComputerMove(GridSize - 1, GridSize - 1);
                return true;
            }
            if (Grid[GridSize - 1, 0] == MoveLetter &&
                Grid[0, GridSize - 1] == TileState.Default)
            {
                ComputerMove(0, GridSize - 1);
                return true;
            }
            if (Grid[0, GridSize - 1] == MoveLetter &&
                Grid[GridSize - 1, 0] == TileState.Default)
            {
                ComputerMove(GridSize - 1, 0);
                return true;
            }
            if (Grid[GridSize - 1, GridSize - 1] == MoveLetter &&
                Grid[0, 0] == TileState.Default)
            {
                ComputerMove(0, 0);
                return true;
            }

            //check free corners
            if (Grid[0, 0] == TileState.Default)
            {
                ComputerMove(0, 0);
                return true;
            }
            if (Grid[0, GridSize - 1] == TileState.Default)
            {
                ComputerMove(0, GridSize - 1);
                return true;
            }
            if (Grid[GridSize - 1, 0] == TileState.Default)
            {
                ComputerMove(GridSize - 1, 0);
                return true;
            }
            if (Grid[GridSize - 1, GridSize - 1] == TileState.Default)
            {
                ComputerMove(GridSize - 1, GridSize - 1);
                return true;
            }

            return false;
        }


        private bool AttemptThreeInARow()
        {
            int computerMatchingTileCount = 0;
            int blankTileCount = 0;
            int blankTileRow = 0;
            int blankTileColumn = 0;
            //check rows
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] == ComputerMoveLetter)
                    {
                        computerMatchingTileCount++;      
                    }
                    else if (Grid[i, j] == TileState.Default)
                    {
                        blankTileCount++;
                        blankTileRow = i;
                        blankTileColumn = j;
                    }
                }

                if (computerMatchingTileCount == 2 && blankTileCount == 1)
                {
                    ComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }

            //check columns
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[j, i] == ComputerMoveLetter)
                    {
                        computerMatchingTileCount++;
                    }
                    else if (Grid[j, i] == TileState.Default)
                    {
                        blankTileCount++;
                        blankTileRow = j;
                        blankTileColumn = i;
                    }
                }

                if (computerMatchingTileCount == 2 && blankTileCount == 1)
                {
                    ComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }


            //check diagonals
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == ComputerMoveLetter)
                {
                    computerMatchingTileCount++;
                }
                else if (Grid[i, i] == TileState.Default)
                {
                    blankTileCount++;
                    blankTileRow = i;
                    blankTileColumn = i;
                }
            }
            if (computerMatchingTileCount == 2 && blankTileCount == 1)
            {
                ComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
            //check anti diagonal
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == ComputerMoveLetter)
                {
                    computerMatchingTileCount++;
                }
                else if (Grid[GridSize - i - 1, i] == TileState.Default)
                {
                    blankTileCount++;
                    blankTileRow = GridSize - i - 1;
                    blankTileColumn = i;
                }
            }
            if (computerMatchingTileCount == 2 && blankTileCount == 1)
            {
                ComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
            return false;
        }

        private bool BlockThreeInARow()
        {
            int computerMatchingTileCount = 0;
            int blankTileCount = 0;
            int blankTileRow = 0;
            int blankTileColumn = 0;
            //check rows
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] == MoveLetter)
                    {
                        computerMatchingTileCount++;
                    }
                    else if (Grid[i, j] == TileState.Default)
                    {
                        blankTileCount++;
                        blankTileRow = i;
                        blankTileColumn = j;
                    }
                }

                if (computerMatchingTileCount == 2 && blankTileCount == 1)
                {
                    ComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }

            //check columns
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[j, i] == MoveLetter)
                    {
                        computerMatchingTileCount++;
                    }
                    else if (Grid[j, i] == TileState.Default)
                    {
                        blankTileCount++;
                        blankTileRow = j;
                        blankTileColumn = i;
                    }
                }

                if (computerMatchingTileCount == 2 && blankTileCount == 1)
                {
                    ComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }


            //check diagonals
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == MoveLetter)
                {
                    computerMatchingTileCount++;
                }
                else if (Grid[i, i] == TileState.Default)
                {
                    blankTileCount++;
                    blankTileRow = i;
                    blankTileColumn = i;
                }
            }
            if (computerMatchingTileCount == 2 && blankTileCount == 1)
            {
                ComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
            //check anti diagonal
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == MoveLetter)
                {
                    computerMatchingTileCount++;
                }
                else if (Grid[GridSize - i - 1, i] == TileState.Default)
                {
                    blankTileCount++;
                    blankTileRow = GridSize - i - 1;
                    blankTileColumn = i;
                }
            }
            if (computerMatchingTileCount == 2 && blankTileCount == 1)
            {
                ComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
            return false;
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
