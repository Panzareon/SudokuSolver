using SudokuSolver.Constraints;
using SudokuSolver.Model;

namespace SudokuSolver;

class Program
{
	static void Main(string[] args)
	{
		var solver = new Solver.BoardSolver(new Board(9, 9), [new DefaultSudoku()]);
		var boards = solver.Solve().Take(2).ToList();
		if (boards.Count == 0)
		{
			Console.WriteLine("No solution found");
			return;
		}
		boards.ForEach(x =>
		{
			x.WriteToConsole();
			Console.WriteLine();
		});
	}
}
