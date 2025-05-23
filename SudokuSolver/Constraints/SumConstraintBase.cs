﻿using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class SumConstraintBase(Func<int, bool> sumPredicate, params Position[] positions) : IConstraint
	{
		public IReadOnlyList<Position> MostImpactedPositions => positions;

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

			return sumPredicate(currentSum + nextStep.NextValue);
		}


		public virtual bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
