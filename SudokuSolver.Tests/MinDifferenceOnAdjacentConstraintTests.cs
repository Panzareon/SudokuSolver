using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Tests
{
	public class MinDifferenceOnAdjacentConstraintTests
	{
		[Test]
		public void SimpleConstraintTest()
		{
			var board = new Board(9, 9);
			var solver = new BacktraceSolver(board, [
				new DefaultSudoku(),
				new MinDifferenceOnAdjacentConstraint(
					8,
					new Position(1,1),
					new Position(2,2),
					new Position(3,3)
					)]);

			var result = solver.Solve().First();
			result.WriteToConsole();
			Assert.That(Math.Abs(result.GetTile(1, 1).Value - result.GetTile(2, 2).Value), Is.AtLeast(8));
			Assert.That(Math.Abs(result.GetTile(2, 2).Value - result.GetTile(3, 3).Value), Is.AtLeast(8));
		}
		[Test]
		public void LessStrictConstraintTest()
		{
			var board = new Board(9, 9);
			var solver = new BacktraceSolver(board, [
				new DefaultSudoku(),
				new MinDifferenceOnAdjacentConstraint(
					4,
					new Position(0,0),
					new Position(1,1),
					new Position(2,2),
					new Position(3,3),
					new Position(4,4),
					new Position(5,5),
					new Position(6,6),
					new Position(7,7),
					new Position(8,8)
					)]);

			var result = solver.Solve().First();
			result.WriteToConsole();
			Assert.That(Math.Abs(result.GetTile(0, 0).Value - result.GetTile(1, 1).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(1, 1).Value - result.GetTile(2, 2).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(2, 2).Value - result.GetTile(3, 3).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(3, 3).Value - result.GetTile(4, 4).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(4, 4).Value - result.GetTile(5, 5).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(5, 5).Value - result.GetTile(6, 6).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(6, 6).Value - result.GetTile(7, 7).Value), Is.AtLeast(4));
			Assert.That(Math.Abs(result.GetTile(7, 7).Value - result.GetTile(8, 8).Value), Is.AtLeast(4));
		}
	}
}
