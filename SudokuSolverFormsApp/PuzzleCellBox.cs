using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SudokuSolverLib;

namespace SudokuUI
{
    public partial class PuzzleCellBox : UserControl
    {
        internal SudokuCell cell;
        SynchronizationContext context;

        public PuzzleCellBox(SudokuCell cell)
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            this.textBox1.KeyUp += textBox1_KeyUp;
            this.textBox1.KeyDown += textBox1_KeyDown;
            SetCell(cell);
        }

        public void SetCell(SudokuCell cell)
        {
            this.cell = cell;
            this.label1.BackColor = Control.DefaultBackColor;
            this.cell.DuplicateEvent += cell_DuplicateEvent;
            this.cell.ValueChanged += cell_ValueChanged;
            this.cell_ValueChanged(null, null);
        }

        void cell_DuplicateEvent(object sender, EventArgs e)
        {
            label1.BackColor = Color.Red;
        }

        void cell_ValueChanged(object sender, EventArgs e)
        {
            if (context != SynchronizationContext.Current)
            {
                context.Post(new SendOrPostCallback(delegate(object state)
                {
                    EventHandler handler = cell_ValueChanged;

                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }), null);
                return;
            }

            this.textBox1.Lines = FormatValuesInLines(this.cell.PossibleValues);

            if (this.cell.IsSolved)
            {
                this.label1.Text = this.cell.SolvedValue.ToString();
                this.label1.Visible = true;
            }
            else
            {
                this.label1.Visible = false;
            }
        }

        private string[] FormatValuesInLines(int[] p)
        {
            string[] line = new string[3];
            int lineNbr = 0;
            foreach (int i in Enumerable.Range(1, 9))
            {
                line[lineNbr] += (p.Contains(i))? i.ToString() : " ";
                if (i % 3 == 0)
                {
                    lineNbr++;
                }
            }
            return line;
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt == false)
            {
                if (!(e.KeyValue >= 37 && e.KeyValue <= 40) ||
                    !(e.KeyValue >= 49 && e.KeyValue <= 57)
                    )
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int keyval = e.KeyValue;
            //49-57 or 97-105 = number 1-9
            if (keyval >= 97 && keyval <= 105)
            {
                keyval -= 48;
            }
            if (keyval >= 49 && keyval <= 57)
            {
                if (e.Shift)
                {
                    this.cell.RemoveValue(keyval - 48);
                }
                else
                {
                    this.cell.SolvedValue = keyval - 48;
                }
            }

            // ARROWS
            if (e.KeyValue >= 37 && e.KeyValue <= 40)
            {
                MoveFocus(e.KeyValue, this.cell.X, this.cell.Y);
            }
        }

        private void MoveFocus(int p, int X, int Y)
        {
            MoveDirection md;
            switch (p)
            {
                case 37: //left
                    md = MoveDirection.LEFT;
                    break;
                case 38: // up
                    md = MoveDirection.UP;
                    break;
                case 39: // right
                    md = MoveDirection.RIGHT;
                    break;
                case 40: // down
                    md = MoveDirection.DOWN;
                    break;
                default:
                    throw new ArgumentException(string.Format("Key supplied {0}, is not an arrow key", p));
            }
            MoveFocusEvent(this, md);
        }

        public event MoveFocusEventHandler MoveFocusEvent;
        public delegate void MoveFocusEventHandler(object sender, MoveDirection direction);
        public enum MoveDirection
        {
            LEFT = 57,
            UP = 38,
            RIGHT = 39,
            DOWN = 40
        }
    }
}
