using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib
{
    internal abstract class SudokuLineSet : SudokuCellSet
    {
        protected override void item_CellPossibleRemovedEvent(UniqueNumberCell cell)
        {
            base.item_CellPossibleRemovedEvent(cell);

            ReduceBySoloBox();
        }

        private void ReduceBySoloBox()
        {
            lock (reduceLocker)
            {
                List<SudokuCell> checkSet = new List<SudokuCell>();
                foreach (int i in Enumerable.Range(1, 9))
                {
                    checkSet.Clear();
                    SudokuBox box = null;

                    bool isUniqueBox = true;
                    foreach (SudokuCell possCell in GetPossibleCellsByValue(i))
                    {
                        if (possCell.IsSolved)
                        {
                            isUniqueBox = false;
                            break;
                        }
                        if (null == box)
                        {
                            box = possCell.box;
                        }
                        else
                        {
                            if (box != possCell.box)
                            {
                                isUniqueBox = false;
                                break;
                            }
                        }
                        checkSet.Add(possCell);
                    }
                    if (isUniqueBox && checkSet.Count > 1)
                    {
                        lock (setLocker)
                        {
                            // remove these values from other cells in the set
                            checkSet[0].box.RemoveValuesFromSet(i, checkSet);
                        }
                    }
                }
            }
        }
    }
}
