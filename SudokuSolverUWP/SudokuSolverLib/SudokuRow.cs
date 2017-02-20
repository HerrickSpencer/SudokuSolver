﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib
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