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
	public class ConsecutiveConstraintTests
	{
		[Test]
		public void SimpleConstraintOn9Test()
		{

			var board = Board.Parse(
				"""
				  9  5 63
				   96    
				5 1  4   
				  67 3  4
				 4 21 39 
				    9  5 
				9 45  6  
				   649 3 
				61  2  4  
				""");
			var solver = new BacktraceSolver(board, [..ConstraintFactory.DefaultSudoku(), new ConsecutiveConstraint(new Position(1, 6), new Position(0, 6))]);

			var result = solver.Solve().ToList();
			result.ForEach(x =>
			{
				x.WriteToConsole();
				Console.WriteLine();
			});
			Assert.That(result, Is.Not.Empty);
			foreach (var resultBoard in result)
			{
				var adjacentTo9 = resultBoard.GetTile(1, 6);
				Assert.That(adjacentTo9.Value, Is.EqualTo(8));
			}
		}

		[Test]
		public void SimpleConstraintOn7Test()
		{

			var board = Board.Parse(
				"""
				249175863
				   96    
				5 1  4   
				   7     
				   21 39 
				    9  5 
				9 45  6  
				   649 3 
				613827549
				""");
			var solver = new BacktraceSolver(board, [..ConstraintFactory.DefaultSudoku(), new ConsecutiveConstraint(new Position(3, 3), new Position(2, 3))]);

			var result = solver.Solve().ToList();
			result.ForEach(x =>
			{
				x.WriteToConsole();
				Console.WriteLine();
			});
			Assert.That(result, Is.Not.Empty);
			foreach (var resultBoard in result)
			{
				var adjacentTo2 = resultBoard.GetTile(2, 3);
				Assert.That(adjacentTo2.Value, Is.EqualTo(6).Or.EqualTo(8));
			}
		}
	}
}
