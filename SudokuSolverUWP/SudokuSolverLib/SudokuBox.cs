using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib
{
    internal class SudokuBox : SudokuCellSet
    {
        internal override void Add(SudokuCell item)
        {
            base.Add(item);
            item.box = this;
        }

        protected override void item_CellPossibleRemovedEvent(UniqueNumberCell cell)
        {
            base.item_CellPossibleRemovedEvent(cell);

            ReduceBySoloSet();
        }

        private void ReduceBySoloSet()
        {
            lock (reduceLocker)
            {
                List<SudokuCell> checkSetRow = new List<SudokuCell>();
                List<SudokuCell> checkSetCol = new List<SudokuCell>();
                foreach (int i in Enumerable.Range(1, 9))
                {
                    checkSetRow.Clear();
                    checkSetCol.Clear();
                    SudokuRow row = null;
                    SudokuColumn col = null;

                    bool isUniqueRow = true;
                    bool isUniqueCol = true;
                    foreach (SudokuCell possCell in GetPossibleCellsByValue(i))
                    {
                        if (possCell.IsSolved)
                        {
                            isUniqueCol = false;
                            isUniqueRow = false;
                            break;
                        }
                        if (null == col)
                        {
                            col = possCell.col;
                            row = possCell.row;
                        }
                        if (isUniqueRow)
                        {
                            if (row != possCell.row)
                            {
                                isUniqueRow = false;
                            }
                            checkSetRow.Add(possCell);
                        }
                        if (isUniqueCol)
                        {
                            if (col != possCell.col)
                            {
                                isUniqueCol = false;
                            }
                            checkSetCol.Add(possCell);
                        }
                    }
                    if (isUniqueCol && checkSetCol.Count > 1)
                    {
                        lock (setLocker)
                        {
                            // remove these values from other cells in the set
                            checkSetCol[0].col.RemoveValuesFromSet(i, checkSetCol);
                        }
                    }
                    if (isUniqueRow && checkSetRow.Count > 1)
                    {
                        lock (setLocker)
                        {
                            // remove these values from other cells in the set
                            checkSetRow[0].row.RemoveValuesFromSet(i, checkSetRow);
                        }
                    }
                }
            }
        }
    }
}
