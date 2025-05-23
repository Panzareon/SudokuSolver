using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System.Diagnostics;

namespace SudokuSolver.Tests
{
	public class PerformanceTests : PerformanceTestsBase
	{
		[Test]
		public void SolveSimpleSudokuPerformanceTests()
		{
			const int NumberOfRepeats = 20;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (var i = 0; i < NumberOfRepeats; i++)
			{
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
				var solver = new BacktraceSolver(board, [..ConstraintFactory.DefaultSudoku()]);

				var result = solver.Solve().ToList();
				Assert.That(result, Has.Count.EqualTo(1));
			}

			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch.Elapsed / NumberOfRepeats} per step");
			Assert.That(stopwatch.Elapsed / NumberOfRepeats, Is.LessThan(TimeSpan.FromMilliseconds(1.2)));
		}

		[Test]
		public void SolveSudokuWithMultipleSolutionsPerformanceTests()
		{
			const int NumberOfRepeats = 10;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (var i = 0; i < NumberOfRepeats; i++)
			{
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
				var solver = new BacktraceSolver(board, [..ConstraintFactory.DefaultSudoku()]);

				var result = solver.Solve().ToList();
				Assert.That(result, Has.Count.EqualTo(3));
			}

			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch.Elapsed / NumberOfRepeats} per step");
			Assert.That(stopwatch.Elapsed / NumberOfRepeats, Is.LessThan(TimeSpan.FromMilliseconds(2.5)));
		}
		[Test]
		public void InvalidSudokuTest()
		{
			const int NumberOfRepeats = 100;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (var i = 0; i < NumberOfRepeats; i++)
			{
				var board = Board.Parse(
				"""
				  9  5 63
				    6    
				5    4   
				  6     4
				   2   9 
				   1   5 
				  4   6  
				   6 9 3 
				61  2  4  
				""");
				var solver = new BacktraceSolver(board, [..ConstraintFactory.DefaultSudoku()]);

				var result = solver.Solve().ToList();
				Assert.That(result, Is.Empty);
			}

			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch.Elapsed / NumberOfRepeats} per step");
			Assert.That(stopwatch.Elapsed / NumberOfRepeats, Is.LessThan(TimeSpan.FromMilliseconds(0.4)));
		}

		[Test]
		public void Sudoku4x4Test()
		{
			var board = Board.Parse(
				"""
				1234
				    
				 3  
				  2 
				""");
			var expectedResult = Board.Parse(
				"""
				1234
				3412
				2341
				4123
				""");
			this.Test(
				board,
				100,
				TimeSpan.FromMilliseconds(0.1),
				[expectedResult],
				null,
				[..ConstraintFactory.DefaultSudoku(2, 2)]);
		}

		[Test]
		public void BacktraceFixedTileSolveTest()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var board = Board.Parse("""
				         
				         
				         
				123      
				   4     
				         
				         
				         
				         
				""");
			var result = new BacktraceSolver(board, [.. ConstraintFactory.DefaultSudoku()])
				.SolveFixedValues();
			Assert.That(result, Is.Not.Null);
			Assert.That(result.GetPossibleValues(6, 5).Values, Is.EquivalentTo(new[] { 1, 2, 3, 5, 6, 7, 8, 9 }));
			Assert.That(result.GetPossibleValues(7, 5).Values, Is.EquivalentTo(new[] { 1, 2, 3, 5, 6, 7, 8, 9 }));
			Assert.That(result.GetPossibleValues(8, 5).Values, Is.EquivalentTo(new[] { 1, 2, 3, 5, 6, 7, 8, 9 }));
			stopwatch.Stop();
			TestContext.Out.WriteLine($"Took {stopwatch.Elapsed} per run");
			Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(0.6)));
		}
	}
}