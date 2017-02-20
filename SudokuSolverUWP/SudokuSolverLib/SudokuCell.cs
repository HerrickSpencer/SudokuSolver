using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib
{
    public class SudokuCell : UniqueNumberCell
    {
        internal SudokuBox box { get; set; } = new SudokuBox();
        internal SudokuColumn col { get; set; } = new SudokuColumn();
        internal SudokuRow row { get; set; } = new SudokuRow();

        public SudokuCell(int x, int y)
            : base(x, y, 1, 9)
        {
            this.CellSolvedEvent += SudokuCell_CellSolvedEvent;
            this.CellPossibleRemovedEvent += SudokuCell_CellPossibleRemovedEvent;
        }

        void SudokuCell_CellPossibleRemovedEvent(UniqueNumberCell cell)
        {
            if (this.PossibleValues.Length == 0)
            {
                FailedEvent(this, new EventArgs());
            }
        }

        void SudokuCell_CellSolvedEvent(UniqueNumberCell cell)
        {
            DuplicateCheck();
        }

        public event EventHandler DuplicateEvent;
        public event EventHandler FailedEvent;

        internal void RaiseDuplicateEvent()
        {
            if (DuplicateEvent != null)
            {
                DuplicateEvent(this, new EventArgs());
                FailedEvent(this, new EventArgs());
            }
        }

        internal bool DuplicateCheck()
        {
            if (!this.IsSolved) { return false; }
            foreach (SudokuCellSet cellSet in new SudokuCellSet[] { box, col, row })
            {
                foreach (SudokuCell cell in cellSet.Cells)
                {
                    if (cell.IsSolved)
                    {
                        if (cell != this && cell.SolvedValue == this.SolvedValue)
                        {
                            cell.RaiseDuplicateEvent();
                            this.RaiseDuplicateEvent();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
