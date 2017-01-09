using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    internal abstract class SudokuCellSet
    {
        List<SudokuCell> cellSet = new List<SudokuCell>();
        protected object setLocker = new object();
        protected static object reduceLocker = new object();

        internal List<SudokuCell> Cells
        {
            get { return cellSet; }
        }

        internal virtual void Add(SudokuCell item)
        {
            cellSet.Add(item);
            item.CellSolvedEvent += new UniqueNumberCell.CellSolvedHandler(item_CellSolvedEvent);
            item.CellPossibleRemovedEvent += new UniqueNumberCell.CellPossibleRemovedHandler(item_CellPossibleRemovedEvent);
        }

        protected virtual void item_CellPossibleRemovedEvent(UniqueNumberCell cell)
        {
            ReduceBySimilarPossibleSet();
        }

        /// <summary>
        /// Attempt to use Linq to solve for ReduceBySimilarPossibleSet (in development)
        /// </summary>
        private void ReduceBySimilarPossibleSet2()
        {
            lock (reduceLocker)
            {
                List<SudokuCell> checkSet = new List<SudokuCell>();
                foreach (SudokuCell thisCell in this.cellSet.Where(thisCell => !thisCell.IsSolved))
                {
                    checkSet.Clear();
                    foreach (int thisCellPossible in thisCell.PossibleValues)
                    {
                        checkSet.AddRange(from possCell in GetPossibleCellsByValue(thisCellPossible)
                                          let canAdd = possCell.PossibleValues.All(possCellPossible => thisCell.IsPossible(possCellPossible))
                                          let checkedVals = HasOnlyAllowedValues(thisCell.PossibleValues, possCell.PossibleValues)
                                          where canAdd && checkedVals
                                          select possCell);
                    }
                    if (checkSet.Count == thisCell.PossibleValues.Length && checkSet.Count > GetUnsolvedCells().Count)
                    {
                        // remove these values from other cells in the set
                        RemoveValuesFromSet(thisCell.PossibleValues, checkSet);
                    }
                }
            }
        }

        private void ReduceBySimilarPossibleSet()
        {
            lock (reduceLocker)
            {
                for (int i = 2; i < this.Cells.Where(n => !n.IsSolved).Count(); i++)
                {
                    List<SudokuCell> checkSet = this.Cells.FindAll(n => n.PossibleValues.Count() <= i && !n.IsSolved);
                    List<SudokuCell> checkSetEq = checkSet.FindAll(n => n.PossibleValues.Count() == i);
                    if (checkSet.Count() > 1 && checkSet.Count() >= i && this.Cells.Where(n => !n.IsSolved).Count() > checkSet.Count())
                    {
                        for (int x = 0; x < checkSetEq.Count; x++)
                        {
                            SudokuCell thisCell = checkSetEq[x];
                            List<SudokuCell> checkSet2 = checkSet;
                            foreach (int val in thisCell.PossibleValues)
                            {
                                checkSet2.RemoveAll(n => !n.IsPossible(val));
                            }
                            if (checkSet2.Count == thisCell.PossibleValues.Count())
                            {
                                RemoveValuesFromSet(thisCell.PossibleValues, checkSet2);
                            }
                        }
                    }
                }
            }
        }

        protected bool HasOnlyAllowedValues(int[] allowedValues, int[] actualValues)
        {
            if (actualValues.Length > allowedValues.Length || allowedValues.Length == 9) return false;
            List<int> badvals = new List<int>(actualValues.Where(test => !allowedValues.Contains(test)));
            return !badvals.Any();
        }

        protected List<SudokuCell> GetUnsolvedCells()
        {
            return this.cellSet.Where(setCell => !setCell.IsSolved).ToList();
        }

        protected List<SudokuCell> GetPossibleCellsByValue(int x)
        {
            List<SudokuCell> checkSet = this.cellSet.Where(setCell => setCell.IsPossible(x)).ToList();
            if (checkSet.Count == 1)
            {
                checkSet[0].SolvedValue = x;
            }
            return checkSet;
        }

        internal void RemoveValuesFromSet(int removeValue, List<SudokuCell> ignoreCells)
        {
            int[] removeValues = new int[] { removeValue };
            RemoveValuesFromSet(removeValues, ignoreCells);
        }

        internal void RemoveValuesFromSet(int[] removeValues, List<SudokuCell> ignoreCells)
        {
            lock (this.setLocker)
            {
                foreach (SudokuCell setCell in this.cellSet)
                {
                    if (setCell.IsSolved || ignoreCells.Contains(setCell))
                    {
                        continue;
                    }
                    setCell.RemoveValue(removeValues);
                }
            }
        }

        protected virtual void item_CellSolvedEvent(UniqueNumberCell cell)
        {
            RemoveValuesFromSet(new int[] { cell.SolvedValue }, new List<SudokuCell> { (SudokuCell)cell });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SudokuCell cell in cellSet)
            {
                sb.Append(String.Join(",", cell.PossibleValues));
                sb.Append("|");
            }
            return sb.ToString();
        }
    }
}