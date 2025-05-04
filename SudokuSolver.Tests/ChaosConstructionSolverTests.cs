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
	public class ChaosConstructionSolverTests
	{
		[Test]
		public void TestSimpleChaosConstruction()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var board = Board.Parse(
				"""
				123456
				      
				      
				      
				      
				      
				""");

			var solver = new ChaosConstructionSolver(
				board,
				[new DefaultSudokuRowsColumns(),
				new ChaosConstructionConstraint(6),
				new NumberOfTilesInRegionInSpecifiedDirection(new Position(2, 0), new PositionDelta(-1, 1))
				]);
			var result = solver.Solve().First();
			foreach (var tileSet in result.TileSets)
			{
				Assert.That(tileSet.Positions, Has.Count.EqualTo(6));
			}
			Assert.That(result.TileSets.Select(x => x.Positions), Has.One.EquivalentTo(new[] { new Position(0, 0), new Position(1, 0), new Position(2, 0), new Position(0, 1), new Position(1, 1), new Position(0, 2) }));
			Assert.That(new[] { result.GetTile(0, 1).Value, result.GetTile(1, 1).Value, result.GetTile(0, 2).Value }, Is.EquivalentTo(new[] { 4, 5, 6 }));
			Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(20)));
		}
	}
}
