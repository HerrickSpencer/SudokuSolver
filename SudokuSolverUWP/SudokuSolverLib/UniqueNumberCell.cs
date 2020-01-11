using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverLib
{
    public abstract class UniqueNumberCell
    {
        HashSet<int> possibleValues = new HashSet<int>();

        static object possibleValuesLock = new object();
        int x, y;

        public int Y
        {
            get { return y; }
        }

        public int X
        {
            get { return x; }
        }

        public UniqueNumberCell(int X, int Y, int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("min should be less than max");
            }
            this.x = X;
            this.y = Y;
            for (int i = min; i <= max; i++)
            {
                this.possibleValues.Add(i);
            }
        }
        public UniqueNumberCell(int X, int Y, int[] possibleValues)
        {
            this.x = X;
            this.y = Y;
            SetPossibleValues(possibleValues);
        }

        public void SetPossibleValues(int[] possibleValues)
        {
            HashSet<int> newValues = new HashSet<int>(possibleValues);
            if (this.possibleValues == newValues) // || this.IsSolved)
            {
                return;
            }
            lock (possibleValuesLock)
            {
                this.possibleValues = new HashSet<int>(possibleValues);
                if (this.IsSolved)
                {
                    CellSolvedEvent(this);
                }
                else
                {
                    CellPossibleRemovedEvent(this);
                }
                RaiseValueChanged();
            }
        }

        public bool IsSolved
        {
            get { return (possibleValues.Count == 1); }
        }

        public void RemoveValue(int value)
        {
            RemoveValue(new int[] { value });
        }

        public void RemoveValue(int[] values)
        {
            if (this.IsSolved)
            {
                return;
            }
            lock (possibleValuesLock)
            {
                bool valueChanged = false;
                foreach (int value in values)
                {
                    valueChanged |= possibleValues.Remove(value);
                }
                if (valueChanged)
                {
                    if (this.IsSolved)
                    {
                        CellSolvedEvent(this);
                    }
                    else
                    {
                        CellPossibleRemovedEvent(this);
                    }
                    RaiseValueChanged();
                }
            }
        }

        private void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                ValueChanged(this, null);
            }
        }

        public int SolvedValue
        {
            get
            {
                if (IsSolved)
                {
                    return possibleValues.Min();
                }
                return -1;
            }
            set
            {
                if (!IsSolved)
                {
                    lock (possibleValuesLock)
                    {
                        if (!IsSolved)
                        {
                            possibleValues = new HashSet<int>() { value };
                            Debug.WriteLine("Solved {0},{1} = {2}", this.X, this.Y, this.SolvedValue.ToString());
                            CellSolvedEvent(this);
                            RaiseValueChanged();
                        }
                    }
                }
            }
        }

        public bool IsPossible(int value)
        {
            return possibleValues.Contains(value);
        }

        public int[] PossibleValues
        {
            get { return possibleValues.ToArray(); }
        }

        public override string ToString()
        {
            return string.Join(",", possibleValues);
        }

        internal delegate void CellSolvedHandler(UniqueNumberCell cell);
        internal event CellSolvedHandler CellSolvedEvent;

        internal delegate void CellPossibleRemovedHandler(UniqueNumberCell cell);
        internal event CellPossibleRemovedHandler CellPossibleRemovedEvent;

        public event EventHandler ValueChanged;
    }
}
