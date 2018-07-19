using System;
using System.Collections.Generic;
using MvvmFoundation.Wpf;

namespace TicTacToe.Models
{
    /// <summary>
    /// Tile states.
    /// </summary>
    public enum TileState
    {
        /// <summary>
        /// Default (empty) tile.
        /// </summary>
        Default,

        /// <summary>
        /// X tile.
        /// </summary>
        X,

        /// <summary>
        /// O tile.
        /// </summary>
        O
    }

    /// <summary>
    /// Tic-tac-toe model.
    /// </summary>
    public class MainWindowModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// Game grid dimensions.
        /// </summary>
        private const int GridSize = 3;

        /* Move types */
        private const bool Blocking = true;
        private const bool NonBlocking = false;

        /// <summary>
        /// Game grid.
        /// </summary>
        private TileState[,] _grid = new TileState[GridSize, GridSize];

        /// <summary>
        /// Move count.
        /// </summary>
        private int _moveCount;

        /// <summary>
        /// Is the game over?
        /// </summary>
        private bool _isGameOver;

        #endregion

        #region Properties

        /* Current game mode */
        public string Mode { get; set; } = "Easy";
        
        /* Queue of winning tiles */
        public Queue<string> WinningTiles { get; set; } = new Queue<string>();

        /* Human player's letter (X or O?) */
        public TileState HumanMoveLetter
        {
            get
            {
                if (_moveCount % 2 == 0)
                {
                    return TileState.O;
                }
                else
                {
                    return TileState.X;
                }
            }
        }

        /* Computer player's letter (X or O?) */
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

        /* Is the game over? */
        public bool IsGameOver
        {
            get
            {
                return _isGameOver;
            }
        }

        /* Game grid dimensions */
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

        /// <summary>
        /// Reset the game grid.
        /// </summary>
        public void ResetGrid()
        {
            _moveCount = 0;
            Array.Clear(Grid, 0, Grid.Length);
            _isGameOver = false;
        }

        /// <summary>
        /// Make a human move at a specified position.
        /// If playing a computer generate its move in response.
        /// </summary>
        /// <param name="tilePosition">Specified move tile position.</param>
        /// <returns>Was the move made?</returns>
        public bool MakeHumanMove(string tilePosition)
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
            Grid[row, column] = HumanMoveLetter;

            if (IsWinner(row, column) || IsDraw())
            {
                _isGameOver = true;
            }
            else
            {
                GenerateComputerMove();
            }

            return true;
        }

        /// <summary>
        /// Generate a computer move.
        /// </summary>
        private void GenerateComputerMove()
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

        /// <summary>
        /// Make a computer move at the generated position.
        /// </summary>
        /// <param name="row">Move row.</param>
        /// <param name="column">Move column.</param>
        private void MakeComputerMove(int row, int column)
        {
            _moveCount++;
            Grid[row, column] = HumanMoveLetter;

            if (IsWinner(row, column) || IsDraw())
            {
                _isGameOver = true;
            }
        }

        /// <summary>
        /// Generate an easy mode computer move.
        /// </summary>
        private void EasyComputerMove()
        {
            /* Generate a random tile position */
            Random rnd = new Random();
            int positionID;
            if (_moveCount == 1)
            {
                positionID = rnd.Next(1, (GridSize * GridSize) - _moveCount);
            }
            else
            {
                positionID = rnd.Next(1,
                    (GridSize * GridSize) - _moveCount + 1);
            }

            /* Find the tile at the generated tile position */
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

            /* Make computer move at the random tile */
            MakeComputerMove(row, column);
        }

        /// <summary>
        /// Generate a medium level computer move.
        /// </summary>
        private void MediumComputerMove()
        {
            /* Make three in a row (if possible)*/
            if (ThreeInARow(NonBlocking) == true)
            {
                /* Made three in a row */
                return;
            }

            /* Block three in a row (if possible) */
            if (ThreeInARow(Blocking) == true)
            {
                /* Three in a row blocked */
                return;
            }

            /* Otherwise make random move */
            EasyComputerMove();
        }

        /// <summary>
        /// Generate a hard level computer move.
        /// </summary>
        private void HardComputerMove()
        {
            /* Make three in a row (if possible)*/
            if (ThreeInARow(NonBlocking) == true)
            {
                /* Three in a row accomplished */
                return;
             }

            /* Block three in a row (if possible)*/
            if (ThreeInARow(Blocking) == true)
            {
                /* Three in a row blocked */
                return;
            }

            /* Make forking move (if possible) */
            if (Fork(NonBlocking) == true)
            {
                /* Fork move made */
                return;
            }

            /* Block forking move (if possible) */
            if (Fork(Blocking) == true)
            {
                /* Fork blocked */
                return;
            }

            /* Is centre move possible? */
            if (Grid[1, 1] == TileState.Default)
            {
                /* Make centre move */
                MakeComputerMove(1, 1);
                return;
            }

            /* Make corner move (if possible) */
            if (AttemptCornerMove() == true)
            {
                /* Made corner move */
                return;
            }

            EasyComputerMove();
        }

        /// <summary>
        /// Attempt a fork or fork blocking move (if possible).
        /// </summary>
        /// <param name="isBlocking">Is it a blocking move?</param>
        /// <returns>Was a move made?</returns>
        private bool Fork(bool isBlocking)
        {
            /* Loop through each tile */
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    /* Check if the tile is empty (can fork/block) */
                    if (Grid[i, j] == TileState.Default)
                    {
                        /* Count how many possible three in a row
                         *  lines exist with this tile in them */
                        int nonBlockedLinesCount = 0;
                        if (IsCandidateRow(i, isBlocking) == true)
                        {
                            nonBlockedLinesCount++;
                        }
                        if (IsCandidateColumn(j, isBlocking) == true)
                        {
                            nonBlockedLinesCount++;
                        }
                        if (InMainDiagonal(i, j) == true &&
                            IsCandidateMainDiagonal(isBlocking))
                        {
                            nonBlockedLinesCount++;
                        }
                        if (InAntiDiagonal(i, j) == true &&
                            IsCandidateAntiDiagonal(isBlocking))
                        {
                            nonBlockedLinesCount++;
                        }

                        /* If there are at least 2 lines it is a potential
                         *  forking tile */
                        if (nonBlockedLinesCount >= 2)
                        {
                            MakeComputerMove(i, j);
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Does a specified tile lie on the main diagonal?
        /// </summary>
        /// <param name="row">Specified row.</param>
        /// <param name="column">Specified column.</param>
        /// <returns>Whether or not tile lies on main diagonal.</returns>
        private bool InMainDiagonal(int row, int column)
        {
            return row == column;
        }

        /// <summary>
        /// Determine whether or not a tile lies on the anti diagonal.
        /// </summary>
        /// <param name="row">Specified row.</param>
        /// <param name="column">Specified column.</param>
        /// <returns>Does tile lie on the anti diagonal?</returns>
        private bool InAntiDiagonal(int row, int column)
        {
            return row == GridSize - column - 1;
        }

        /// <summary>
        /// Determine if the specified row is a candidate for making/blocking
        /// a fork move.
        /// </summary>
        /// <param name="row">Row of game grid.</param>
        /// <param name="isBlockingMove">Is it a blocking move?</param>
        /// <returns>Is the specified row a candidate for making/blocking a
        /// fork move?</returns>
        private bool IsCandidateRow(int row, bool isBlockingMove)
        {
            TileState moveLetter;
            if (isBlockingMove)
            {
                moveLetter = ComputerMoveLetter;
            }
            else
            {
                moveLetter = HumanMoveLetter;
            }
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[row, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[row, i] == moveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        /// <summary>
        /// Determine if a specified column is a candidate for making/blocking
        /// a fork move.
        /// </summary>
        /// <param name="column">Column of game grid.</param>
        /// <param name="isBlockingMove">Is it a blocking move?</param>
        /// <returns>Is the specified column a candidate for making/blocking a
        /// fork move?</returns>
        private bool IsCandidateColumn(int column, bool isBlockingMove)
        {
            TileState moveLetter;
            if (isBlockingMove)
            {
                moveLetter = ComputerMoveLetter;
            }
            else
            {
                moveLetter = HumanMoveLetter;
            }
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, column] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, column] == moveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        /// <summary>
        /// Is the antidiagonal a candidate for making/blocking a fork move?
        /// </summary>
        /// <param name="isBlockingMove">Is it a blocking move?</param>
        /// <returns>Is the antidiagonal a candidate for
        /// making/blocking a fork?</returns>
        private bool IsCandidateAntiDiagonal(bool isBlockingMove)
        {
            TileState moveLetter;
            if (isBlockingMove)
            {
                moveLetter = ComputerMoveLetter;
            }
            else
            {
                moveLetter = HumanMoveLetter;
            }
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[GridSize - i - 1, i] == moveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        /// <summary>
        /// Determine if the main diagonal is a candidate for making/blocking
        /// a fork move.
        /// </summary>
        /// <param name="isBlockingMove">Is it a blocking move?</param>
        /// <returns>Is the main diagonal a candidate for making/blocking a
        /// fork move?</returns>
        private bool IsCandidateMainDiagonal(bool isBlockingMove)
        {
            TileState moveLetter;
            if (isBlockingMove)
            {
                moveLetter = ComputerMoveLetter;
            }
            else
            {
                moveLetter = HumanMoveLetter;
            }
            int blankCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == TileState.Default)
                {
                    blankCount++;
                    continue;
                }

                if (Grid[i, i] == moveLetter)
                {
                    return false;
                }

            }

            return blankCount == GridSize - 1;
        }

        /// <summary>
        /// Attempt a move either to block the opponent from getting
        /// three in a row or to get three in a row.
        /// </summary>
        /// <param name="isBlockingMove">Is it a blocking move?</param>
        /// <returns>Was a move made?</returns>
        private bool ThreeInARow(bool isBlockingMove)
        {
            TileState moveLetter;
            if (isBlockingMove)
            {
                moveLetter = HumanMoveLetter;
            }
            else
            {
                moveLetter = ComputerMoveLetter;
            }
            int computerMatchingTileCount = 0;
            int blankTileCount = 0;
            int blankTileRow = 0;
            int blankTileColumn = 0;
            
            /* Check rows */
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[i, j] == moveLetter)
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
                    MakeComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }

            /* Check columns */
            for (int i = 0; i < GridSize; i++)
            {
                computerMatchingTileCount = 0;
                blankTileCount = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if (Grid[j, i] == moveLetter)
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
                    MakeComputerMove(blankTileRow, blankTileColumn);
                    return true;
                }

            }

            /* Check diagonal */
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] == moveLetter)
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
                MakeComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
           
            /* Check anti diagonal */
            computerMatchingTileCount = 0;
            blankTileCount = 0;
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - i - 1, i] == moveLetter)
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
                MakeComputerMove(blankTileRow, blankTileColumn);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt a move in a corner. 
        /// </summary>
        /// <returns>Was move generated?</returns>
        private bool AttemptCornerMove()
        {
            // check opposite corners to opponent
            if (Grid[0, 0] == HumanMoveLetter &&
                Grid[GridSize - 1, GridSize - 1] == TileState.Default)
            {
                MakeComputerMove(GridSize - 1, GridSize - 1);
                return true;
            }
            if (Grid[GridSize - 1, 0] == HumanMoveLetter &&
                Grid[0, GridSize - 1] == TileState.Default)
            {
                MakeComputerMove(0, GridSize - 1);
                return true;
            }
            if (Grid[0, GridSize - 1] == HumanMoveLetter &&
                Grid[GridSize - 1, 0] == TileState.Default)
            {
                MakeComputerMove(GridSize - 1, 0);
                return true;
            }
            if (Grid[GridSize - 1, GridSize - 1] == HumanMoveLetter &&
                Grid[0, 0] == TileState.Default)
            {
                MakeComputerMove(0, 0);
                return true;
            }

            /* Check free corners */
            if (Grid[0, 0] == TileState.Default)
            {
                MakeComputerMove(0, 0);
                return true;
            }
            if (Grid[0, GridSize - 1] == TileState.Default)
            {
                MakeComputerMove(0, GridSize - 1);
                return true;
            }
            if (Grid[GridSize - 1, 0] == TileState.Default)
            {
                MakeComputerMove(GridSize - 1, 0);
                return true;
            }
            if (Grid[GridSize - 1, GridSize - 1] == TileState.Default)
            {
                MakeComputerMove(GridSize - 1, GridSize - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Does the specified column have three in a row?
        /// </summary>
        /// <param name="column">Column of game grid.</param>
        /// <returns>Is there a column with three in a row?</returns>
        private bool IsColumnWin(int column)
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (_grid[i, column] != HumanMoveLetter)
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

        /// <summary>
        /// Does the specified row have three in a row?
        /// </summary>
        /// <param name="row">Row of game grid.</param>
        /// <returns>Is there a row with three in a row?</returns>
        private bool IsRowWin(int row)
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (_grid[row, i] != HumanMoveLetter)
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

        /// <summary>
        /// Determine if there are three in a row on the main diagonal.
        /// </summary>
        /// <returns>Are there three in a row on the main diagonal?</returns>
        private bool IsMainDiagonalWin()
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[i, i] != HumanMoveLetter)
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

        /// <summary>
        /// Determines if there are three in a row on the anti diagonal.
        /// </summary>
        /// <returns>Are there three in a row on the anti diagonal?</returns>
        private bool IsAntiDiagonalWin()
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (Grid[GridSize - 1 - i, i] != HumanMoveLetter)
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

        /// <summary>
        /// Determines if there is a winner.
        /// If there is a winner, adds winning tiles to a queue.
        /// </summary>
        /// <param name="row">Game grid row.</param>
        /// <param name="column">Game grid column.</param>
        /// <returns>Is there a winner?</returns>
        private bool IsWinner(int row, int column)
        {
            if (IsColumnWin(column))
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(string.Format("{0}{1}", i, column));
                }
                return true;
            }

            if (IsRowWin(row))
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(string.Format("{0}{1}", row, i));
                }
                return true;
            }

            if (row == column && IsMainDiagonalWin())
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(string.Format("{0}{1}", i, i));
                }
                return true;
            }

            if (((row + column) == GridSize - 1) &&
                IsAntiDiagonalWin())
            {
                for (int i = 0; i < GridSize; i++)
                {
                    WinningTiles.Enqueue(string.Format("{0}{1}", 
                        i,
                        GridSize - 1 - i));
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if there is a draw.
        /// </summary>
        /// <returns>Is there a draw?</returns>
        private bool IsDraw()
        {
            return _moveCount == Math.Pow(GridSize, 2);
        }

    }
}
