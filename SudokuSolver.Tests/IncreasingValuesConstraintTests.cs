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
	public class IncreasingValuesConstraintTests
	{
		[Test]
		public void SimpleConstraintTest()
		{
			var board = new Board(9, 9);
			var solver = new BacktraceSolver(board, [
				..ConstraintFactory.DefaultSudoku(),
				new IncreasingValues(
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
			Assert.That(result.GetTile(0, 0).Value, Is.EqualTo(1));
			Assert.That(result.GetTile(1, 1).Value, Is.EqualTo(2));
			Assert.That(result.GetTile(2, 2).Value, Is.EqualTo(3));
			Assert.That(result.GetTile(3, 3).Value, Is.EqualTo(4));
			Assert.That(result.GetTile(4, 4).Value, Is.EqualTo(5));
			Assert.That(result.GetTile(5, 5).Value, Is.EqualTo(6));
			Assert.That(result.GetTile(6, 6).Value, Is.EqualTo(7));
			Assert.That(result.GetTile(7, 7).Value, Is.EqualTo(8));
			Assert.That(result.GetTile(8, 8).Value, Is.EqualTo(9));
		}
		[Test]
		public void LessStrictConstraintTest()
		{
			var board = new Board(9, 9);
			var solver = new BacktraceSolver(board, [
				..ConstraintFactory.DefaultSudoku(),
				new IncreasingValues(
					new Position(0,0),
					new Position(1,1),
					new Position(2,2),
					new Position(3,3),
					new Position(4,4)
					)]);

			var result = solver.Solve().First();
			result.WriteToConsole();
			Assert.That(result.GetTile(0, 0).Value, Is.LessThan(result.GetTile(1, 1).Value));
			Assert.That(result.GetTile(1, 1).Value, Is.LessThan(result.GetTile(2, 2).Value));
			Assert.That(result.GetTile(2, 2).Value, Is.LessThan(result.GetTile(3, 3).Value));
			Assert.That(result.GetTile(3, 3).Value, Is.LessThan(result.GetTile(4, 4).Value));
		}
	}
}
