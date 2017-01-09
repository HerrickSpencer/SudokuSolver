using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            SudokuSolver.SudokuPuzzle puz = new SudokuSolver.SudokuPuzzle();
            puz.Cells[0, 1].SetPossibleValues(new int[] { 9 });
            puz.Cells[0, 2].SetPossibleValues(new int[] { 2 });
            Assert.IsFalse( puz.Cells[0, 3].PossibleValues.Where(n => n == 2 || n == 9).Any(), "found 2,9 in 0,3");
        }
    }
}
