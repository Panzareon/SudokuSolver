using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class ConsecutiveConstraint(Position first, Position second) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => [first, second];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			Tile adjacent;
			if (nextStep.IsAt(first))
			{
				adjacent = board.GetTile(second);
			}
			else if (nextStep.IsAt(second))
			{
				adjacent = board.GetTile(first);
			}
			else
			{
				return true;
			}

			if (!adjacent.IsSet)
			{
				return true;
			}

			return adjacent.Value == nextStep.NextValue + 1 || adjacent.Value == nextStep.NextValue - 1;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
