using System.IO;
using SudokuSolver;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuUI
{
    public partial class Form1 : Form
    {
        SudokuPuzzle puzzle = new SudokuPuzzle();
        PuzzleCellBox[,] cellBoxes = new PuzzleCellBox[9, 9];

        int cellWidth = 35;
        int cellHeight = 50;
        int cellGapX = 10;
        int cellGapY = 5;

        int xStart = 25;
        int xPos;
        int yPos = 25;
        int tabIndex = 0;

        int[] Xpoints = new int[4];
        int[] Ypoints = new int[4];

        private SynchronizationContext context;

        public Form1()
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            SudukuPuzzleBreaker.BreakerProgressEvent += SudukuPuzzleBreaker_BreakerProgressEvent;

            for (int y = 0; y < 9; y++)
            {
                if (y % 3 == 0) // draw horz line
                {
                    Ypoints[y / 3] = yPos - (cellGapY / 3);
                }
                xPos = xStart;
                for (int x = 0; x < 9; x++)
                {
                    if (y == 0 && x % 3 == 0) // draw vert line
                    {
                        Xpoints[x / 3] = xPos - (cellGapX / 2);
                    }
                    cellBoxes[x, y] = new PuzzleCellBox(puzzle.Cells[x, y]);
                    cellBoxes[x, y].Location = new Point(xPos, yPos);
                    cellBoxes[x, y].TabIndex = tabIndex++;
                    cellBoxes[x, y].MoveFocusEvent += Form1_MoveFocusEvent;
                    this.Controls.Add(cellBoxes[x, y]);
                    xPos += cellWidth + cellGapX;
                }
                yPos += cellHeight + cellGapY;
            }

            // set last two positions for lines
            Ypoints[3] = yPos - (cellGapY / 2);
            Xpoints[3] = xPos - (cellGapX / 2);

            textBox1.KeyUp += Form1_KeyUp;

            cellBoxes[0, 0].Select();
            InitializeEvents();
        }

        public void RunTestStuff()
        {
            string testfile = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName + "\\Boards\\Board.csv";
            SudokuPuzzle newPuz = new SudokuPuzzle(SudokuPuzzle.GetPuzzle(testfile));
            ChangePuzzle(newPuz);
            //btnMostDiff.PerformClick();
            //btnCellBreak.PerformClick();
        }

        bool showedPuzzleStatus = false;
        bool tryingBreak = false;
        void puzzle_PuzzleFailedEvent(object sender, EventArgs e)
        {
            if (context != SynchronizationContext.Current)
            {
                context.Post(new SendOrPostCallback(delegate(object state)
                {
                    EventHandler handler = puzzle_PuzzleFailedEvent;

                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }), null);
                return;
            }
            if (!showedPuzzleStatus && !tryingBreak)
            {
                showedPuzzleStatus = true;
                MessageBox.Show("Puzzle Failed");
            }
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox2.Text = e.KeyValue.ToString();
        }

        void Form1_MoveFocusEvent(object sender, PuzzleCellBox.MoveDirection direction)
        {
            int X = ((PuzzleCellBox)sender).cell.X;
            int Y = ((PuzzleCellBox)sender).cell.Y;
            textBox1.Text = String.Format(" x: {0}, y: {1} ", X, Y);
            switch (direction)
            {
                case PuzzleCellBox.MoveDirection.RIGHT:
                    if (X < 8)
                    {
                        this.cellBoxes[X + 1, Y].Focus();
                    }
                    break;
                case PuzzleCellBox.MoveDirection.LEFT:
                    if (X > 0)
                    {
                        this.cellBoxes[X - 1, Y].Focus();
                    }
                    break;
                case PuzzleCellBox.MoveDirection.UP:
                    if (Y > 0)
                    {
                        this.cellBoxes[X, Y - 1].Focus();
                    }
                    break;
                case PuzzleCellBox.MoveDirection.DOWN:
                    if (Y < 8)
                    {
                        this.cellBoxes[X, Y + 1].Focus();
                    }
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
            {
                using (System.Drawing.Graphics formGraphics = this.CreateGraphics())
                {
                    foreach (int x in Xpoints)
                    {
                        if (x != Xpoints[0] && x != Xpoints[Xpoints.GetUpperBound(0)])
                        {
                            formGraphics.DrawLine(myPen, x, Ypoints[0], x, Ypoints[Ypoints.GetUpperBound(0)]);
                        }
                    }
                    foreach (int y in Ypoints)
                    {
                        if (y != Ypoints[0] && y != Ypoints[Ypoints.GetUpperBound(0)])
                        {
                            formGraphics.DrawLine(myPen, Xpoints[0], y, Xpoints[Xpoints.GetUpperBound(0)], y);
                        }
                    }
                }
            }
        }

        private void ChangePuzzle(SudokuPuzzle newPuz)
        {
            foreach (PuzzleCellBox pz in this.cellBoxes)
            {
                pz.SetCell(newPuz.Cells[pz.cell.X, pz.cell.Y]);
            }
            this.puzzle = newPuz;
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            showedPuzzleStatus = false;
            puzzle.PuzzleFailedEvent += puzzle_PuzzleFailedEvent;

            if (!puzzle.Failed && puzzle.IsSolved) /// don't need to do anything for failed, event will handle this.
            {
                MessageBox.Show("WOOT");
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            SudokuPuzzle newPuz = new SudokuPuzzle();
            ChangePuzzle(newPuz);
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName + "\\Boards";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SudokuPuzzle newPuz = new SudokuPuzzle(SudokuPuzzle.GetPuzzle(openFileDialog1.FileName));
                    ChangePuzzle(newPuz);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string puzzleStr = puzzle.ToString().Replace(",","");
                    using(StreamWriter sr = new StreamWriter(saveFileDialog1.FileName))
                    {
                        sr.Write(puzzleStr);
                        sr.Flush();
                        sr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnMostDiff_Click(object sender, EventArgs e)
        {
            string filePath = @".\Boards\GEOboard.csv";
            filePath = @".\Boards\MostDifficult.csv";
            SudokuPuzzle newPuz = new SudokuPuzzle(SudokuPuzzle.GetPuzzle(filePath));
            //SudokuPuzzle newPuz = new SudokuPuzzle(SudokuSolver.Program.GetPuzzle(10));

            ChangePuzzle(newPuz);
        }

        private void btnCellBreak_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            TryBreak();
            this.Cursor = Cursors.Default;
        }

        int changedCtr = 0;
        void SudukuPuzzleBreaker_BreakerProgressEvent(BreakerProgressEventArgs e)
        {
            if (context != SynchronizationContext.Current)
            {
                context.Post(new SendOrPostCallback(delegate(object state)
                {
                    SudukuPuzzleBreaker.BreakerProgressEventHandler handler = SudukuPuzzleBreaker_BreakerProgressEvent;

                    if (handler != null)
                    {
                        handler(new BreakerProgressEventArgs(null));
                    }
                }), null);
                return;
            }
            textBox1.Visible = true;
            textBox1.Text = string.Format("Permutations tested : {0}", ++changedCtr);
            //ChangePuzzle(e.Puzzle);
        }

        private async void TryBreak()
        {
            this.tryingBreak = true;
            changedCtr = 0;
            Task<bool> t = SudukuPuzzleBreaker.TryBreakAsync(puzzle);

            if (await t)
            {
                this.tryingBreak = false;
                ChangePuzzle(puzzle);
            }
            this.tryingBreak = false;
            textBox1.Visible = true;
            textBox1.Text = string.Format(" Completed|Permutations : {0}", ++changedCtr);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            RunTestStuff();
#endif
        }
    }
}
