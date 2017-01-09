using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public class SudokuPuzzle
    {
        SudokuCellSet[] cellsets = new SudokuCellSet[9 + 9 + 9];
        SudokuCell[,] allCells = new SudokuCell[9, 9];

        public SudokuCell[,] Cells
        {
            get { return allCells; }
        }

        public SudokuPuzzle()
        {
            Initialize();
        }

        public SudokuPuzzle(short[,] puzzle)
        {
            Initialize();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    short value = puzzle[x, y];
                    if (value > 0)
                    {
                        allCells[x, y].SolvedValue = value;
                    }
                }
            }
        }

        private void Initialize()
        {
            this.Failed = false;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    int boxX = (int)Math.Floor(x / 3.0);
                    int boxY = (int)Math.Floor(y / 3.0);
                    int boxIndex = 18 + boxX + (boxY * 3);
                    if (x == 0) { cellsets[y] = new SudokuRow(); }
                    if (y == 0) { cellsets[9 + x] = new SudokuColumn(); }
                    if (null == cellsets[boxIndex])
                    {
                        cellsets[boxIndex] = new SudokuBox();
                    }
                    SudokuCell cell = new SudokuCell(x, y);
                    cell.FailedEvent += cell_FailedEvent;
                    cellsets[y].Add(cell);
                    cellsets[9 + x].Add(cell);
                    cellsets[boxIndex].Add(cell);
                    allCells[x, y] = cell;
                }
            }
        }

        void cell_FailedEvent(object sender, EventArgs e)
        {
            this.Failed = true;
            if (PuzzleFailedEvent != null)
            {
                PuzzleFailedEvent(this, new EventArgs());
            }
        }

        public event EventHandler PuzzleFailedEvent;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SudokuRow row in cellsets.Where(set => set is SudokuRow))
            {
                sb.AppendLine(row.ToString());
            }
            return sb.ToString();
        }

        public string ToHTMLString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><body><table border=1>");
            foreach (SudokuRow row in cellsets.Where(set => set is SudokuRow))
            {
                sb.AppendLine("<tr>");
                foreach (SudokuCell cell in row.Cells)
                {
                    sb.AppendLine("<td>");
                    sb.AppendLine(String.Join("|", cell.PossibleValues));
                    sb.AppendLine("</td>");
                }
                sb.AppendLine("<tr>");
            }
            sb.AppendLine("</table></body></html>");
            return sb.ToString();
        }

        private bool failed = false;
        public bool Failed
        {
            get
            {
                if (this.failed) return this.failed;
                foreach (SudokuCell cell in this.allCells)
                {
                    if (cell.DuplicateCheck())
                    {
                        this.failed = true;
                        return true;
                    }
                }
                return this.failed;
            }
            set
            {
                this.failed = value;
            }
        }

        public bool IsSolved
        {
            get
            {
                foreach (SudokuCell cell in this.allCells)
                {
                    if (!cell.IsSolved)
                    {
                        return false;
                    }
                    if (cell.DuplicateCheck())
                    {
                        this.Failed = true;
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a puzzle from a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Int16[,] GetPuzzle(string filePath)
        {
            Int16[,] puzzle = new Int16[9, 9];
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int lineNbr = 0;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] items = line.Split('|');
                        for (int x = 0; x < items.Length; x++)
                        {
                            if (items[x].Length == 1)
                            {
                                Int16.TryParse(items[x], out puzzle[x, lineNbr]);
                            }
                        }
                        lineNbr++;
                    }
                }
            }
            return puzzle;
        }

        internal short[,] ToIntArray()
        {
            short[,] puzVals = new short[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (allCells[x, y].IsSolved)
                    {
                        puzVals[x, y] = (short)allCells[x, y].SolvedValue;
                    }
                }
            }
            return puzVals;
        }

        public SudokuCell GetFirstUnsolvedCell()
        {
            foreach (SudokuCell cell in this.Cells)
            {
                if (cell.IsSolved) { continue; }
                return cell;
            }
            return null;
        }

        public List<SudokuCell> GetsSudokuSet(int index, SudokuSetType type)
        {
            if (index >= 9 || index < 0)
            {
                throw new IndexOutOfRangeException("Cell sets only range 0-8");
            }
            return this.cellsets[index + ((int)type * 9)].Cells;
        }
    }

    public enum SudokuSetType : int
    {
        Row = 0,
        Column = 1,
        Box = 3
    }
}
