using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Tests
{
	public class PerformanceTestsBase
	{
		[OneTimeSetUp]
		public void WarmUp()
		{
			_ = new BacktraceSolver(new Board(9, 9), [new DefaultSudoku(), new ConsecutiveConstraint(new Position(0, 1), new Position(0, 2))])
				.Solve().First();
		}

		protected void Test(Board board, int numberOfRuns, TimeSpan timePerRun, Board[] expectedResult, int? limitResult, params IConstraint[] constraints)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			for (var i = 0; i < numberOfRuns; i++)
			{
				var currentBoard = board.Clone();
				var solver = new BacktraceSolver(currentBoard, constraints);
				var result = solver.Solve();
				if (limitResult.HasValue)
				{
					result = result.Take(limitResult.Value);
				}

				Assert.That(result, Is.EquivalentTo(expectedResult));
			}

			stopwatch.Stop();
			Assert.That(stopwatch.Elapsed / numberOfRuns, Is.LessThan(timePerRun));
		}
	}
}
