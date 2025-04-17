using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class DefaultSudoku : IConstraint
	{
		public bool CanPlace(Board board, NextStep nextStep)
		{
			if (this.NumberExistsInRow(board, nextStep))
			{
				return false;
			}
			if (this.NumberExistsInColumn(board, nextStep))
			{
				return false;
			}
			if (this.NumberExistsInBox(board, nextStep))
			{
				return false;
			}

			return true;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return RemoveColumnGroups(board)
				&& RemoveRowGroups(board)
				&& RemoveBoxGroups(board);
		}

		private bool NumberExistsInBox(Board board, NextStep nextStep)
		{
			var xStart = nextStep.SetX / board.BoxSize * board.BoxSize;
			var yStart = nextStep.SetY / board.BoxSize * board.BoxSize;

			for (var x = 0; x < board.BoxSize; x++)
			{
				for (var y = 0; y < board.BoxSize; y++)
				{
					if (xStart + x == nextStep.SetX && yStart + y == nextStep.SetY)
					{
						continue;
					}

					var tile = board.GetTile(xStart + x, yStart + y);
					if (tile.IsSet && tile.Value == nextStep.NextValue)
					{
						return true;
					}
				}
			}

			return false;
		}

		private bool NumberExistsInRow(Board board, NextStep nextStep)
		{
			for (var x = 0; x < board.Width; x++)
			{
				if (nextStep.SetX == x)
				{
					continue;
				}

				var tile = board.GetTile(x, nextStep.SetY);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					return true;
				}
			}

			return false;
		}
		private bool NumberExistsInColumn(Board board, NextStep nextStep)
		{
			for (var y = 0; y < board.Height; y++)
			{
				if (nextStep.SetY == y)
				{
					continue;
				}

				var tile = board.GetTile(nextStep.SetX, y);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					return true;
				}
			}

			return false;
		}

		private static bool RemoveRowGroups(Board board)
		{
			for (var y = 0; y < board.Height; y++)
			{
				var set = new List<PossibleValues>();
				for (var x = 0; x < board.Width; x++)
				{
					set.Add(board.GetPossibleValues(x, y));
				}

				RemoveFromSet(set);
				if (!IsValidSet(board, set))
				{
					return false;
				}
			}

			return true;
		}

		private static void RemoveFromSet(List<PossibleValues> set)
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

		private static bool RemoveColumnGroups(Board board)
		{
			for (var x = 0; x < board.Width; x++)
			{
				var set = new List<PossibleValues>();
				for (var y = 0; y < board.Height; y++)
				{
					set.Add(board.GetPossibleValues(x, y));
				}

				RemoveFromSet(set);
				if (!IsValidSet(board, set))
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsValidSet(Board board, List<PossibleValues> set)
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

		private static bool RemoveBoxGroups(Board board)
		{
			for (var boxX = 0; boxX < board.Width; boxX += board.BoxSize)
			{
				for (var boxY = 0; boxY < board.Height; boxY += board.BoxSize)
				{
					var set = new List<PossibleValues>();
					for (var x = 0; x < board.BoxSize; x++)
					{
						for (var y = 0; y < board.BoxSize; y++)
						{
							set.Add(board.GetPossibleValues(x + boxX, y + boxY));
						}
					}

					RemoveFromSet(set);
					if (!IsValidSet(board, set))
					{
						return false;
					}
				}
			}

			return true;
		}

	}
}
