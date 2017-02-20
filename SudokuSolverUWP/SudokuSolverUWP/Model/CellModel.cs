using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SudokuSolverLib;
using System.Collections.ObjectModel;
using Windows.UI.Core;

namespace SudokuSolverUWP.Model
{
    public class CellModel : ObservableObject
    {
        SudokuCell cell;

        public CellModel(CoreDispatcher dispatcher, SudokuCell cell) : base(dispatcher)
        {
            this.cell = cell;
            foreach (int val in cell.PossibleValues)
            {
                this.PossibleValues.Add(val);
            }
        }

        public ObservableCollection<int> PossibleValues = new ObservableCollection<int>();
        public void SetPossible(int[] vals)
        {
            cell.SetPossibleValues(vals);
        }

        public int SolvedValue
        {
            get {
                return cell.PossibleValues.First();
            }
            set {
                cell.SetPossibleValues(new int[] { value});
                base.RaisepropertyChanged("SolvedValue");
            }
        }
    }
}
