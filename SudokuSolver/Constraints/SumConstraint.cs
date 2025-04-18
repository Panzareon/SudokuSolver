using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class SumConstraint(int sum, params Position[] positions) : IConstraint
	{
		public bool CanPlace(Board board, NextStep nextStep)
		{
			var index = positions.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			var currentSum = 0;
			for (var i = 0; i < positions.Length; i++)
			{
				if (i == index)
				{
					continue;
				}

				var tile = board.GetTile(positions[i]);
				if (!tile.IsSet)
				{
					return true;
				}

				currentSum += tile.Value;
			}

			return currentSum == sum;
		}


		public bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
