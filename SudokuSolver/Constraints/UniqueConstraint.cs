using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class UniqueConstraint(params Position[] positions) : IConstraint
	{
		public bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				if (nextStep.SetX == position.X && nextStep.SetY == position.Y)
				{
					continue;
				}

				var tile = board.GetTile(position);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					return false;
				}
			}

			return true;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			var set = new List<PossibleValues>();
			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				set.Add(board.GetPossibleValues(position.X, position.Y));
			}

			return HandleSet(board, set);
		}

		public static bool HandleSet(Board board, IReadOnlyList<PossibleValues> set)
		{
			RemoveFromSet(set);
			return IsValidSet(board, set);
		}

		private static void RemoveFromSet(IReadOnlyList<PossibleValues> set)
		{
			for (var i = 0; i < set.Count; i++)
			{
				var values = set[i].Values.ToList();
				var checkedValues = new List<int> { i };
				for (var checkIndex = 0; checkIndex < set.Count && checkedValues.Count < values.Count; checkIndex++)
				{
					if (checkIndex == i)
					{
						continue;
					}

					if (set[checkIndex].Values.All(x => values.Contains(x)))
					{
						checkedValues.Add(checkIndex);
					}
				}

				if (checkedValues.Count == values.Count)
				{
					for (var removeIndex = 0; removeIndex < set.Count; removeIndex++)
					{
						if (!checkedValues.Contains(removeIndex))
						{
							set[removeIndex].RemoveValues(values);
						}
					}
				}
			}
		}

		private static bool IsValidSet(Board board, IReadOnlyCollection<PossibleValues> set)
		{
			for (var i = 1; i <= board.MaxNumber; i++)
			{
				if (!set.Any(x => x.Values.Contains(i)))
				{
					return false;
				}
			}

			return true;
		}

	}
}
