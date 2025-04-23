using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public abstract class RelativeConstraintBase : IConstraint
	{
		private readonly Position[][] positions;

		public RelativeConstraintBase(params Position[][] positions)
		{
			this.positions = positions;
		}

		public bool CanPlace(Board board, NextStep nextStep)
		{
			for (var set = 0; set < positions.Length; set++)
			{
				var currentSet = positions[set];
				for (var i = 0; i < currentSet.Length; i++)
				{
					var current = currentSet[i];
					if (nextStep.SetX == current.X && nextStep.SetY == current.Y && !this.CanPlace(board, nextStep, set, i))
					{
						return false;
					}
				}
			}

			return true;
		}

		protected abstract bool CanPlace(Board board, NextStep nextStep, int setIndex, int indexInSet);

		public virtual bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
	}
}
