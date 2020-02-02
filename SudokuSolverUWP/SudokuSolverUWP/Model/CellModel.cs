using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SudokuSolverLib;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace SudokuSolverUWP.Model
{
    public class CellModel : ObservableObject
    {
        SudokuCell cell;

        public CellModel(CoreDispatcher dispatcher, SudokuCell cell) : base(dispatcher)
        {
            this.cell = cell;
            cell.ValueChanged += Cell_ValueChanged;
            foreach (int x in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                PossibleValues.Add(cell.IsPossible(x) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        private void Cell_ValueChanged(object sender, EventArgs e)
        {
            foreach (int x in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                if (PossibleValues[x - 1] != (cell.IsPossible(x) ? Visibility.Visible : Visibility.Collapsed))
                {
                    PossibleValues[x - 1] = (cell.IsPossible(x) ? Visibility.Visible : Visibility.Collapsed);
                }
            }

            if (cell.IsSolved)
            {
                PossibleValues[cell.SolvedValue - 1] = Visibility.Collapsed;
            }
            base.RaisepropertyChanged("SolvedValue");
        }

        public ObservableCollection<Visibility> PossibleValues = new ObservableCollection<Visibility>();

        public string SolvedValue
        {
            get
            {
                return cell.SolvedValue > 0?cell.SolvedValue.ToString():"";
            }
        }

        /// <summary>
        /// for testing only
        /// </summary>
        internal void AddPossible()
        {
            foreach (int x in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                if (!cell.IsPossible(x))
                {
                    int[] added = cell.PossibleValues.Concat(new[] { x }).ToArray();
                    cell.SetPossibleValues(added);
                    return;
                }
            }
            base.RaisepropertyChanged("PossibleValues");
        }

        /// <summary>
        /// for testing only
        /// </summary>
        public void RemovePossible()
        {
            foreach (int x in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })
            {
                if (cell.PossibleValues.Contains(x))
                {
                    cell.RemoveValue(x);
                    return;
                }
            }
            base.RaisepropertyChanged("PossibleValues");
        }

        /// <summary>
        /// for testing only
        /// </summary>
        internal void SetPossible(int[] v)
        {
            cell.SetPossibleValues(v);
        }
    }
}
