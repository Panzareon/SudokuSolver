using SudokuSolver.Constraints;
using SudokuSolver.Model;

namespace SudokuSolver;

class Program
{
	static void Main(string[] args)
	{
		var solver = new Solver.Solver(new Board(9, 9), [new DefaultSudoku()]);
		var board = solver.Solve();
		if (board == null)
		{
			Console.WriteLine("No solution found");
			return;
		}
		board.WriteToConsole();
		Console.WriteLine("Hello, World!");
	}
}
