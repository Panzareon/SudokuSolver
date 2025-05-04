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
	/// Constraint, that the value X in indexed position Y (1 indexed) specifies, that in index X there is the value Y.
	/// </summary>
	/// <param name="indexedPositions"></param>
	public class IndexLine(params Position[] indexedPositions) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => indexedPositions;

		public bool CanPlace(Board board, NextStep nextStep)
		{
			var index = indexedPositions.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			if (nextStep.NextValue > indexedPositions.Length)
			{
				return false;
			}

			if (nextStep.NextValue - 1 == index)
			{
				// Indexing itself
				return true;
			}

			var indexedPosition = indexedPositions[nextStep.NextValue - 1];
			return board.GetPossibleValues(indexedPosition.X, indexedPosition.Y).Values.Contains(index + 1);
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
