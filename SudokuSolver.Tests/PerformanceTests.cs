using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System.Diagnostics;

namespace SudokuSolver.Tests
{
	public class PerformanceTests
	{
		[Test]
		public void SolveSimpleSudokuPerformanceTests()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var board = Board.Parse(
				"""
				  9 85 63
				 7 96    
				5 1  4   
				  67 3  4
				 4 21 39 
				8   9  57
				9845  6  
				  7649 3 
				61  2  4  
				""");
			var solver = new BoardSolver(board, [new DefaultSudoku()]);

			var result = solver.Solve().ToList();

			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch}");
			Assert.That(result, Has.Count.EqualTo(1));
#if DEBUG
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(150));
#else
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5));
#endif
		}

		[Test]
		public void SolveSudokuWithMultipleSolutionsPerformanceTests()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var board = Board.Parse(
				"""
				  9  5 63
				 7 96    
				5 1  4   
				  67    4
				 4 2   9 
				8   9  57
				984   6  
				  7649   
				61  2  4  
				""");
			var solver = new BoardSolver(board, [new DefaultSudoku()]);

			var result = solver.Solve().ToList();

			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch}");
			Assert.That(result, Has.Count.EqualTo(3));
#if DEBUG
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(12000));
#else
			Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(25));
#endif
		}
	}
}