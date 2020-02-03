using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SudokuSolverLib
{
    public class SudukuPuzzleBreaker
    {
        public static List<SudokuPuzzle> SolvedPuzzles { get; set; } = new List<SudokuPuzzle>();

        public static async Task<bool> TryBreakAsync(SudokuPuzzle puzzle, bool CheckAllSolutions = false)
        {
            SolvedPuzzles.Clear();
            bool t = await Task.Run(() => TryBreak(puzzle, CheckAllSolutions));
            return t;
        }

        public static bool TryBreak(SudokuPuzzle puzzle, bool CheckAllSolutions)
        {
            if (puzzle.IsSolved)
            {
                return true;
            }
            if (puzzle.Failed)
            {
                return false;
            }
            SudokuCell cellToTry = puzzle.GetFirstUnsolvedCell();

            foreach (int possible in cellToTry.PossibleValues)
            {
                SudokuPuzzle puzzleTotry = new SudokuPuzzle(puzzle.ToIntArray());
                SudokuPuzzle puzzleTried = new SudokuPuzzle(puzzle.ToIntArray());
                SudokuCell cell = puzzleTotry.Cells[cellToTry.X, cellToTry.Y];

                cell.SolvedValue = possible;
                if (puzzleTotry.IsSolved)
                {
                    if (CheckAllSolutions)
                    {
                        SolvedPuzzles.Add(puzzleTotry);
                        // remove possible option and continue
                        puzzleTried.Cells[cellToTry.X, cellToTry.Y].RemoveValue(possible);
                        return TryBreak(puzzleTried, CheckAllSolutions);
                    }
                    else
                    { 
                        CopyPuzzle(puzzleTotry, puzzle);
                        return true; // no progress needed, it will return puzzle
                    }
                }
                else
                {
                    if (puzzleTotry.Failed)
                    {
                        puzzle.Cells[cell.X, cell.Y].RemoveValue(possible);
                        RaiseBreakerProgressEvent(puzzle);
                        if (cell.IsSolved)
                        {
                            return TryBreak(puzzle, CheckAllSolutions);
                        }
                    }
                    else
                    {
                        //try another cell
                        if (TryBreak(puzzleTotry, CheckAllSolutions))
                        {
                            CopyPuzzle(puzzleTotry, puzzle);
                            RaiseBreakerProgressEvent(puzzle);           
                            return true;
                        }
                        else
                        {
                            puzzle.Cells[cell.X, cell.Y].RemoveValue(possible);
                            CopyPuzzle(puzzle, puzzleTotry);
                            RaiseBreakerProgressEvent(puzzle);
                            if (cell.IsSolved)
                            {
                                return TryBreak(puzzle, CheckAllSolutions); 
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static void CopyPuzzle(SudokuPuzzle fromPuzzle, SudokuPuzzle toPuzzle)
        {
            foreach (SudokuCell cell in toPuzzle.Cells)
            {
                cell.SetPossibleValues(fromPuzzle.Cells[cell.X, cell.Y].PossibleValues);
            }
        }

        public static event BreakerProgressEventHandler BreakerProgressEvent;
        public delegate void BreakerProgressEventHandler(BreakerProgressEventArgs e);
        private static void RaiseBreakerProgressEvent(SudokuPuzzle puzzle)
        {
            if (BreakerProgressEvent != null)
            {
                BreakerProgressEvent(new BreakerProgressEventArgs(puzzle ));
            }
        }

    }

    public class BreakerProgressEventArgs : EventArgs
    {
        public BreakerProgressEventArgs(SudokuPuzzle puzzle)
        {
            this.Puzzle = puzzle;
        }

        public SudokuPuzzle Puzzle { get; set; }
    }
}
