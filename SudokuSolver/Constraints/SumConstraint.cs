using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class SumConstraint : SumConstraintBase
	{
		private readonly int sum;
		private readonly Position[] positions;

		public SumConstraint(int sum, params Position[] positions)
			: base(x => x == sum, positions)
		{
			this.sum = sum;
			this.positions = positions;
		}

		public override bool RemoveNotPossibleValues(Board board)
		{
			for (var i = 0; i < positions.Length; i++)
			{
				var positionToCheck = positions[i];
				var possibleValues = board.GetPossibleValues(positionToCheck.X, positionToCheck.Y);
				var minSum = 0;
				var maxSum = 0;
				for (var j = 0; j < positions.Length; j++)
				{
					if (i == j)
					{
						continue;
					}

					var otherPosition = positions[j];
					var possibleValuesToAdd = board.GetPossibleValues(otherPosition.X, otherPosition.Y);
					minSum += possibleValuesToAdd.Values.First?.Value ?? 0;
					maxSum += possibleValuesToAdd.Values.Last?.Value ?? 0;
				}

				var nodeToCheck = possibleValues.Values.First;
				while (nodeToCheck != null)
				{
					if (minSum + nodeToCheck.Value > this.sum || maxSum + nodeToCheck.Value < this.sum)
					{
						var next = nodeToCheck.Next;
						possibleValues.Remove(nodeToCheck);
						nodeToCheck = next;
					}
					else
					{
						nodeToCheck = nodeToCheck.Next;
					}
				}

				if (possibleValues.Values.Count == 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}
