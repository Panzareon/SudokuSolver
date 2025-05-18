using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	/// <summary>
	/// Constraint, that the sum of values at specific positions equal the value at another position.
	/// </summary>
	public class ArrowConstraint(Position sumPosition, params Position[] positionsToSum) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions { get; } = [.. positionsToSum, sumPosition];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			if (sumPosition.X == nextStep.SetX && sumPosition.Y == nextStep.SetY)
			{
				var possibleValuesToCheck = board.GetPossibleSumsOfPositions(positionsToSum);
				return possibleValuesToCheck.Contains(nextStep.NextValue);
			}

			var index = positionsToSum.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			var possibleValues = board.GetPossibleSumsOfPositions(positionsToSum, index, nextStep.NextValue);
			return board.GetPossibleValues(sumPosition.X, sumPosition.Y)
				.Values.Any(possibleValues.Contains);
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
