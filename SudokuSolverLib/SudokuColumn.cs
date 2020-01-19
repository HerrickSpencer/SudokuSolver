namespace SudokuSolverLib
{
    internal class SudokuColumn : SudokuLineSet
    {
        internal override void Add(SudokuCell item)
        {
            base.Add(item);
            item.col = this;
        } 
    }
}
