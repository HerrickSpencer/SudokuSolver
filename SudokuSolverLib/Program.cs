using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SudokuSolver
{
    public class Program
    {
        static void Main(string[] args)
        {
            SudokuPuzzle sp = new SudokuPuzzle();
            //sp.Populate(getSolution(true));
            //sp.Populate(GetPuzzle(10));
            sp = new SudokuPuzzle(SudokuPuzzle.GetPuzzle(@".\Boards\GEOboard.csv"));
            
            Console.WriteLine(sp.ToString());
            //WritePuzzle(@"K:\MyStuff\Documents\Personal\Sudoku\Output.txt", sp, false);
            WritePuzzle(@".\Output.html", sp);
            Console.ReadLine();
        }

        private static void WritePuzzle(string filePath, SudokuPuzzle sp, bool writeHtml = true)
        {
            using (StreamWriter sr = new StreamWriter(filePath))
            {
                sr.Write((writeHtml)?sp.ToHTMLString():sp.ToString());
            }
        }

        private static void DoVerifyOldway()
        {
            if (isSudokuSolved(getSolution(true)))
            {
                Console.WriteLine("SOLVED");
            }
            else
            {
                Console.WriteLine("FAILED");
            }
        }

        

        /// <summary>
        /// Gets some puzzles based on difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static Int16[,] GetPuzzle(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return new Int16[9, 9] {
                    {5, 3, 0, 0, 7, 0, 0, 0, 0},
                    {6, 0, 0, 1, 9, 5, 0, 0, 0},
                    {0, 9, 8, 0, 0, 0, 0, 6, 0},
                    {8, 0, 0, 0, 6, 0, 0, 0, 3},
                    {4, 0, 0, 8, 0, 3, 0, 0, 1},
                    {7, 0, 0, 0, 2, 0, 0, 0, 6},
                    {0, 6, 0, 0, 0, 0, 2, 8, 0},
                    {0, 0, 0, 4, 1, 9, 0, 0, 5},
                    {0, 0, 0, 0, 8, 0, 0, 7, 9}
                };
                case 10:
                    return new Int16[9, 9] {
                    {4,0,0,0,0,9,0,0,0},
                    {7,0,0,4,5,0,0,8,0},
                    {0,0,0,0,0,0,0,0,0},
                    {5,0,0,0,0,3,0,7,0},
                    {0,8,0,0,1,0,0,3,0},
                    {0,6,0,2,0,0,0,0,4},
                    {0,0,0,0,0,0,0,0,0},
                    {0,3,0,0,8,4,0,0,9},
                    {0,0,0,6,0,0,0,0,2}                };
                default:
                    return getSolution(true);
            }
        }

        /// <summary>
        /// gets a sample solution, either solved or not
        /// </summary>
        /// <param name="solved"></param>
        /// <returns></returns>
        static Int16[,] getSolution(bool solved)
        {
            Int16[,] mySolution;
            if (solved)
            {
                mySolution = new Int16[9, 9] {
                    {5, 3, 4, 6, 7, 8, 9, 1, 2},
                    {6, 7, 2, 1, 9, 5, 3, 4, 8},
                    {1, 9, 8, 3, 4, 2, 5, 6, 7},
                    {8, 5, 9, 7, 6, 1, 4, 2, 3},
                    {4, 2, 6, 8, 5, 3, 7, 9, 1},
                    {7, 1, 3, 9, 2, 4, 8, 5, 6},
                    {9, 6, 1, 5, 3, 7, 2, 8, 4},
                    {2, 8, 7, 4, 1, 9, 6, 3, 5},
                    {3, 4, 5, 2, 8, 6, 1, 7, 9}
                };
            }
            else
            {
                Int16[] fullNumbSet = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                mySolution = new Int16[9, 9];
                for (Int16 x = 0; x < mySolution.GetUpperBound(0); x++)
                {
                    for (Int16 y = 0; y < mySolution.GetUpperBound(1); y++)
                    {
                        mySolution[x, y] = fullNumbSet[x];
                    }
                }
            }
            return mySolution;
        }

        /// <summary>
        /// the old way to check if this is a solution
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        static bool isSudokuSolved(Int16[,] solution)
        {
            if (!(solution.GetUpperBound(0) == 8 && solution.GetUpperBound(1) == 8)) { return false; }

            HashSet<short>[] rows = new HashSet<short>[9];

            for (int x = 0; x < 9; x++)
            {
                HashSet<Int16> cols = new HashSet<short>();
                for (int y = 0; y < 9; y++)
                {
                    //check bounds
                    if (!IsBetween(solution[x, y], 1, 9)) { return false; }

                    //Check and add new
                    if (
                        (!AddValueVerify(cols, solution[x, y]))
                        || (!AddValueVerify(rows[y], solution[x, y]))
                        )
                    {
                        return false; //dupe
                    }
                    if (x % 3 == 0 && y % 3 == 0)
                    {
                        //It's the top left corner of a box
                        if (!CheckBox(solution, x, y))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Part of old way to heck if a puzzle is solved
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private static bool CheckBox(short[,] solution, int X, int Y)
        {

            HashSet<Int16> box = new HashSet<short>();
            for (int x = X; x < X + 3; x++)
            {
                for (int y = Y; y < Y + 3; y++)
                {
                    if (!AddValueVerify(box, solution[x, y]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// for old way of checking solution - add value to hash 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool AddValueVerify(HashSet<short> hash, Int16 value)
        {
            if (null == hash) { hash = new HashSet<short>(); }
            return hash.Add(value);
        }
        
        static bool IsBetween<T>(T value, T minValue, T maxValue) where T : IComparable<T>
        {
            Debug.Assert(minValue.CompareTo(maxValue) <= 0);
            if (minValue.CompareTo(maxValue) > 0)
            {
                throw new ArgumentException(string.Format("minValue ({1}) must be less than or equal to maxValue ({2})", minValue.ToString(), maxValue.ToString()));
            }

            return (value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0);
        }
    }

}
