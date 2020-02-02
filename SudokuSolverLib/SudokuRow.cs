namespace SudokuSolverLib
{
    internal class SudokuRow : SudokuLineSet
    {
        internal override void Add(SudokuCell item)
        {
            base.Add(item);
            item.row = this;
        }
    }
}
