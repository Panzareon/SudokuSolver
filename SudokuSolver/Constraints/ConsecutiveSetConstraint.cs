using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class ConsecutiveSetConstraint(params Position[] positions) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => positions;

		public bool CanPlace(Board board, NextStep nextStep)
		{
			var index = positions.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			var mustContainHigherValue = nextStep.NextValue;
			var mustContainLowerValue = nextStep.NextValue;
			var values = new List<int>();
			for (var i = 0; i < positions.Length; i++)
			{
				if (i == index)
				{
					continue;
				}

				var position = positions[i];
				var tile = board.GetTile(position);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					return false;
				}

				var possibleValues = board.GetPossibleValues(position.X, position.Y);
				var node = possibleValues.Values.First;
				while (node != null)
				{
					if (!values.Contains(node.Value))
					{
						values.Add(node.Value);
					}
					node = node.Next;
				}

				if (possibleValues.Values.First?.Value > mustContainHigherValue)
				{
					mustContainHigherValue = possibleValues.Values.First.Value;
				}

				if (possibleValues.Values.Last?.Value < mustContainLowerValue)
				{
					mustContainLowerValue = possibleValues.Values.Last.Value;
				}
			}

			if ((mustContainHigherValue - mustContainLowerValue) + 1 > positions.Length)
			{
				return false;
			}

			var minValue = nextStep.NextValue;
			for (var i = nextStep.NextValue - 1; i >= 1 && i >= mustContainHigherValue - positions.Length + 1; i--)
			{
				if (values.Contains(i))
				{
					minValue = i;
				}
				else
				{
					break;
				}
			}

			var maxValue = nextStep.NextValue;
			for (var i = nextStep.NextValue + 1; i <= board.MaxNumber && i <= mustContainLowerValue + positions.Length - 1; i++)
			{
				if (values.Contains(i))
				{
					maxValue = i;
				}
				else
				{
					break;
				}
			}

			if (maxValue - minValue + 1 >= positions.Length)
			{
				return true;
			}

			return false;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return UniqueConstraint.HandleSet(board, positions);
		}
	}
}
