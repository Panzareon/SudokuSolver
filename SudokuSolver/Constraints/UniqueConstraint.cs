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
			var containsNextStep = false;
			var containsValue = false;
			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				if (nextStep.SetX == position.X && nextStep.SetY == position.Y)
				{
					containsNextStep = true;
					continue;
				}

				var tile = board.GetTile(position);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					if (containsNextStep)
					{
						return false;
					}

					containsValue = true;
				}
			}

			return !(containsNextStep && containsValue);
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

		private static bool IsValidSet(Board board, IReadOnlyList<PossibleValues> set)
		{
			var differentValues = set[0].Values.ToList();
			if (differentValues.Count >= set.Count)
			{
				return true;
			}
			for (var i = 1; i < set.Count; i++)
			{
				var node = set[i].Values.First;
				while (node != null)
				{
					if (!differentValues.Contains(node.Value))
					{
						differentValues.Add(node.Value);
						if (differentValues.Count >= set.Count)
						{
							return true;
						}
					}

					node = node.Next;
				}
			}

			return false;
		}

	}
}
