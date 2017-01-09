using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    internal class SudokuRow : SudokuLineSet
    {
        internal override void Add(SudokuCell item)
        {
            base.Add(item);
            item.row = this;
        }
    }
}
