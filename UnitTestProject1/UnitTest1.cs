using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolverLib;

namespace SudokuSolver
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_solvedValuesInColumn()
        {
            SudokuPuzzle puzzle = new SudokuPuzzle();
            puzzle.Cells[0, 1].SetPossibleValues(new[] { 2 });
            puzzle.Cells[0, 2].SetPossibleValues(new[] { 9 });
            foreach (SudokuCell cell in puzzle.GetsSudokuSet(0, SudokuSetType.Column))
            {
                if (cell.X == 0 && (cell.Y == 1 || cell.Y == 2))
                {
                    continue;
                }
                Assert.IsFalse(cell.IsPossible(2), "Cell {0},{1} is not correct: {2}", cell.X, cell.Y, string.Join(",", cell.PossibleValues));
                Assert.IsFalse(cell.IsPossible(9), "Cell {0},{1} is not correct: {2}", cell.X, cell.Y, string.Join(",", cell.PossibleValues));
            }
            string teststr = puzzle.ToString();
        }

        [TestMethod]
        public void Test_DoubleValueColumn()
        {
            SudokuPuzzle puzzle = new SudokuPuzzle();
            puzzle.Cells[0, 1].SetPossibleValues(new[] { 2, 9 });
            puzzle.Cells[0, 2].SetPossibleValues(new[] { 2, 9 });
            foreach (SudokuCell cell in puzzle.GetsSudokuSet(0, SudokuSetType.Column))
            {
                if (cell.X == 0 && (cell.Y == 1 || cell.Y == 2))
                {
                    continue;
                }
                Assert.IsFalse(cell.IsPossible(2), "Cell {0},{1} is not correct: {2}", cell.X, cell.Y, string.Join(",", cell.PossibleValues));
                Assert.IsFalse(cell.IsPossible(9), "Cell {0},{1} is not correct: {2}", cell.X, cell.Y, string.Join(",", cell.PossibleValues));
            }
            string teststr = puzzle.ToString();
        }
    }
}
