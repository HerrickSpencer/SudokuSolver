using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuSolverLib;
using Windows.UI.Core;

namespace SudokuSolverUWP.Model
{
    public class PuzzleModel //: ObservableObject
    {
        SudokuPuzzle puzzle;

        //public PuzzleModel(CoreDispatcher dispatcher) : base(dispatcher)
        public PuzzleModel(CoreDispatcher dispatcher, SudokuPuzzle sudokuPuzzle)
        {
            this.puzzle = sudokuPuzzle;
            puzzle.Cells[8, 8].SetPossibleValues(new int[] { 1 });

            puzzle.Cells[0, 0].SetPossibleValues(new int[] { 1 });
            puzzle.Cells[0, 1].SetPossibleValues(new int[] { 2 });
            puzzle.Cells[0, 2].SetPossibleValues(new int[] { 3 });
            puzzle.Cells[0, 3].SetPossibleValues(new int[] { 4 });
            puzzle.Cells[0, 4].SetPossibleValues(new int[] { 5 });
            puzzle.Cells[0, 5].SetPossibleValues(new int[] { 6 });
            puzzle.Cells[0, 6].SetPossibleValues(new int[] { 7 });
            puzzle.Cells[0, 7].SetPossibleValues(new int[] { 8 });
            puzzle.Cells[0, 8].SetPossibleValues(new int[] { 9 });
            puzzle.Cells[1, 0].SetPossibleValues(new int[] { 2 });
            puzzle.Cells[2, 0].SetPossibleValues(new int[] { 3 });
            puzzle.Cells[8, 0].SetPossibleValues(new int[] { 9 });
            puzzle.Cells[3, 0].SetPossibleValues(new int[] { 4 });
            puzzle.Cells[4, 0].SetPossibleValues(new int[] { 5 });
            puzzle.Cells[5, 0].SetPossibleValues(new int[] { 6 });
            puzzle.Cells[6, 0].SetPossibleValues(new int[] { 7 });
            puzzle.Cells[7, 0].SetPossibleValues(new int[] { 8 });
            puzzle.Cells[8, 0].SetPossibleValues(new int[] { 9 });
            foreach (SudokuCell cell in this.puzzle.Cells)
            {
                this.Cells.Add(new CellModel(dispatcher, cell));
            }
        }

        public ObservableCollection<CellModel> Cells { get; } = new ObservableCollection<CellModel>();
    }
}
