using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class CountAdjacentValuesConstraint(Position countingPosition, Func<IReadOnlyList<PossibleValues>, int, bool> isValid, params Position[] positions)
		: RelativeConstraintBase([countingPosition], positions)
	{
		protected override bool CanPlace(Board board, NextStep nextStep, int setIndex, int indexInSet)
		{
			if (setIndex == 0)
			{
				var possibleValues = new List<PossibleValues>();
				for (var i = 0; i < positions.Length; i++)
				{
					var position = positions[i];
					possibleValues.Add(board.GetPossibleValues(position.X, position.Y));
				}

				return isValid(possibleValues, nextStep.NextValue);
			}
			else
			{
				var tile = board.GetTile(countingPosition);
				if (!tile.IsSet)
				{
					return true;
				}

				var possibleValues = new List<PossibleValues>();
				for (var i = 0; i < positions.Length; i++)
				{
					if (i == indexInSet)
					{
						possibleValues.Add(PossibleValues.Create(nextStep.NextValue));
						continue;
					}
					var position = positions[i];
					possibleValues.Add(board.GetPossibleValues(position.X, position.Y));
				}

				return isValid(possibleValues, tile.Value);
			}

		}
	}
}
