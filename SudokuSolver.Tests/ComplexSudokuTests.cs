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
	public class ComplexSudokuTests : PerformanceTestsBase
	{
		/// <summary>
		/// Test that solves the "Tetrafolium" sudoku by Myxo.
		/// </summary>
		[Test]
		public void Tetrafolium()
		{
			var board = new Board(9, 9);
			this.Test(board,
				1,
#if DEBUG
				TimeSpan.FromSeconds(18)
#else
				TimeSpan.FromSeconds(3.5)
#endif
				,
				[Board.Parse(
				"""
				738526491
				465319728
				921478653
				513267984
				284195376
				697843215
				356982147
				849731562
				172654839
				""")],
				null,
				[new DefaultSudoku(),
				..ConstraintFactory.KillerCage(20, new Position(0, 0), new Position(0, 1), new Position(1, 0), new Position(1, 1)),
				ConstraintFactory.GermanWhisper(new Position(1, 0), new Position(2, 0), new Position(3, 1)),
				new SumConstraintBase(x => x % 2 == 1, new Position(3, 0), new Position(4, 0), new Position(5,0)),
				ConstraintFactory.GermanWhisper(new Position(5, 1), new Position(6, 0), new Position(7, 0)),
				..ConstraintFactory.KillerCage(20, new Position(7, 0), new Position(7, 1), new Position(8, 0), new Position(8, 1)),
				ConstraintFactory.GermanWhisper(new Position(0, 1), new Position(0, 2), new Position(1, 3)),
				new IncreasingValues(new Position(3, 1), new Position(3, 2), new Position(4, 3), new Position(5, 2), new Position(5, 1)),
				ConstraintFactory.GermanWhisper(new Position(8, 1), new Position(8, 2), new Position(7, 3)),
				new SumConstraintBase(x => x % 2 == 1, new Position(0, 3), new Position(0, 4), new Position(0, 5)),
				new IncreasingValues(new Position(1, 3), new Position(2, 3), new Position(2, 4), new Position(1, 4), new Position(1, 5)),
				new IncreasingValues(new Position(7, 5), new Position(6, 5), new Position(6, 4), new Position(7, 4), new Position(7, 3)),
				new SumConstraintBase(x => x % 2 == 1, new Position(8, 3), new Position(8, 4), new Position(8, 5)),
				ConstraintFactory.GermanWhisper(new Position(1, 5), new Position(0, 6), new Position(0, 7)),
				ConstraintFactory.GermanWhisper(new Position(7, 5), new Position(8, 6), new Position(8, 7)),
				..ConstraintFactory.KillerCage(20, new Position(0, 7), new Position(0, 8), new Position(1, 7), new Position(1, 8)),
				ConstraintFactory.GermanWhisper(new Position(1, 8), new Position(2, 8), new Position(3, 7)),
				new IncreasingValues(new Position(5, 7), new Position(5, 8), new Position(4, 8), new Position(3, 8), new Position(3, 7)),
				ConstraintFactory.GermanWhisper(new Position(5, 7), new Position(6, 8), new Position(7, 8)),
				..ConstraintFactory.KillerCage(20, new Position(7, 7), new Position(7, 8), new Position(8, 7), new Position(8, 8)),
				]);
		}
	}
}
