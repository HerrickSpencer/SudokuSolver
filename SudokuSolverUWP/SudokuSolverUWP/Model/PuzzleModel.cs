using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuSolverLib;
using Windows.UI.Core;

namespace SudokuSolverUWP.Model
{
    public class PuzzleModel //: ObservableObject
    {
        SudokuCell cell;

        //public PuzzleModel(CoreDispatcher dispatcher) : base(dispatcher)
        public PuzzleModel()
        {
            Cell = new SudokuCell(2,3);
            Cell.SolvedValue = 5;
        }

        public SudokuCell Cell
        {
            get
            {
                return cell;
            }

            set
            {
                cell = value;
                //base.Set(ref cell, value);
            }
        }
    }
}
