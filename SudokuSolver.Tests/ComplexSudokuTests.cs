using NUnit.Framework;
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
				TimeSpan.FromSeconds(1.6),
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

		/// <summary>
		/// Test that solves the "Thermoregulators" sudoku by SSG.
		/// </summary>
		[Test]
		public void Thermoregulators()
		{
			var board = new Board(9, 9);
			this.Test(board,
				1,
				TimeSpan.FromMilliseconds(150),
				[Board.Parse(
					"""
					824679315
					196534782
					753812469
					671293854
					249758136
					538461927
					365987241
					487125693
					912346578
					""")],
				null,
				[new DefaultSudoku(),
				..CreateSpecialThermometer(new Position(3, 3), new Position(2, 2), new Position(1, 2), new Position(2, 1), new Position(1, 1)),
				..CreateSpecialThermometer(new Position(5, 3), new Position(6, 2), new Position(7, 2), new Position(6, 1), new Position(7, 1)),
				..CreateSpecialThermometer(new Position(3, 5), new Position(2, 6), new Position(1, 6), new Position(2, 7), new Position(1, 7)),
				..CreateSpecialThermometer(new Position(5, 5), new Position(6, 6), new Position(7, 6), new Position(6, 7), new Position(7, 7)),
				..CreateSpecialThermometer(new Position(0, 4), new Position(1, 5), new Position(1, 4), new Position(1, 3), new Position(2, 4)),
				..CreateSpecialThermometer(new Position(3, 8), new Position(4, 8), new Position(5, 7), new Position(5, 6), new Position(4, 6)),
				..CreateSpecialThermometer(new Position(6, 4), new Position(7, 3), new Position(8, 4))]);


			IEnumerable<IConstraint> CreateSpecialThermometer(params Position[] positions)
			{
				return [
					new IncreasingValues(positions),
					new CountAdjacentValuesConstraint(positions[0], IsValid, positions),
					];
			}
			bool IsValid(IReadOnlyList<PossibleValues> values, int sum)
			{
				var minResult = 0;
				var maxResult = 0;
				for (var i = 0; i < values.Count - 1; i++)
				{
					var first = values[i];
					var second = values[i + 1];
					if (first.Values.All(x => second.Values.All(s => s == x + 1)))
					{
						minResult++;
						maxResult++;
					}
					else if (first.Values.Any(x => second.Values.Any(s => s == x + 1)))
					{
						maxResult++;
					}
				}

				return sum >= minResult && sum <= maxResult;
			}
		}

		/// <summary>
		/// A test that solves the "Goldilocks and the Renbans" sudoku by Qodec.
		/// </summary>
		[Test]
		[Explicit("Currently taking too long")]
		public void GoldilocksAndTheRenbans()
		{
			this.Test(
				new Board(9, 9),
				1,
#if DEBUG
				TimeSpan.FromMinutes(6),
#else
				TimeSpan.FromSeconds(25),
#endif
				[Board.Parse(
					"""
					762543819
					531698427
					948127536
					217954368
					359286741
					486371952
					194765283
					675832194
					823419675
					""")],
				null,
				[new DefaultSudoku(),
				..ConstraintFactory.KillerCage(18, new Position(0, 0), new Position(1, 0), new Position(0, 1)),
				new ConsecutiveSetConstraint(new Position(3, 0), new Position(4, 0), new Position(5, 0)),
				..ConstraintFactory.KillerCage(18, new Position(6, 0), new Position(7, 0), new Position(8, 0)),
				..ConstraintFactory.KillerCage(7, new Position(3, 1), new Position(3, 2)),
				new ConsecutiveSetConstraint(new Position(8, 1), new Position(8, 2), new Position(8, 3)),
				new ConsecutiveSetConstraint(new Position(2, 2), new Position(3, 3)),
				new ConsecutiveSetConstraint(new Position(0, 3), new Position(0, 4), new Position(0, 5)),
				..ConstraintFactory.KillerCage(8, new Position(1, 3), new Position(2, 3)),
				..ConstraintFactory.KillerCage(12, new Position(4, 3), new Position(5, 3), new Position(6, 3)),
				..ConstraintFactory.KillerCage(12, new Position(3, 4), new Position(3, 5), new Position(3, 6)),
				new ConsecutiveSetConstraint(new Position(5, 4), new Position(4, 4), new Position(4, 5)),
				..ConstraintFactory.KillerCage(15, new Position(5, 5), new Position(5, 6), new Position(6, 5)),
				..ConstraintFactory.KillerCage(15, new Position(0, 6), new Position(0, 7), new Position(0, 8)),
				new ConsecutiveSetConstraint(new Position(8, 6), new Position(8, 7), new Position(8, 8), new Position(7, 8), new Position(6, 8)),
				..ConstraintFactory.KillerCage(10, new Position(6, 7), new Position(7, 7)),
				new ConsecutiveSetConstraint(new Position(1, 8), new Position(2, 8), new Position(3, 8)),
				]);
		}
	}
}
