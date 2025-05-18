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
	/// This constraints specifies, that instead of having full boxes, each box overlaps in one column/row with each neighboring box.
	/// </summary>
	/// <param name="boxWidth">The full width of each box.</param>
	/// <param name="boxHeight">The full height of each box.</param>
	public class SquishdokuConstraint(int boxWidth = 3, int boxHeight = 3) : IConstraint
	{
		public IReadOnlyList<Position> MostImpactedPositions => [];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			if (this.NumberExistsInBox(board, nextStep))
			{
				return false;
			}

			return true;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return RemoveBoxGroups(board);
		}

		private bool NumberExistsInBox(Board board, NextStep nextStep)
		{
			foreach (var (xStart, yStart) in this.GetRelevantBoxes(board, nextStep.SetX, nextStep.SetY))
			{
				for (var x = 0; x < boxWidth; x++)
				{
					for (var y = 0; y < boxHeight; y++)
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
			}

			return false;
		}

		private IEnumerable<(int x, int y)> GetRelevantBoxes(Board board, int x, int y)
		{
			var possibleXValues = new List<int>();
			if (x < boxWidth)
			{
				possibleXValues.Add(0);
			}
			else
			{
				var leftBox = (x -1) / (boxWidth - 1) * (boxWidth - 1);
				possibleXValues.Add(leftBox);
				if (x % (boxWidth - 1) == 0 && x <= board.Width - boxWidth)
				{
					possibleXValues.Add(leftBox + 1);
				}
			}

			var possibleYValues = new List<int>();
			if (y < boxHeight)
			{
				possibleXValues.Add(0);
			}
			else
			{
				var leftBox = (y - 1) / (boxHeight - 1) * (boxHeight - 1);
				possibleXValues.Add(leftBox);
				if (y % (boxHeight - 1) == 0 && y <= board.Height - boxHeight)
				{
					possibleXValues.Add(leftBox + 1);
				}
			}

			return possibleXValues.SelectMany(xValue => possibleYValues.Select(yValue => (xValue, yValue)));
		}

		private bool RemoveBoxGroups(Board board)
		{
			for (var boxX = 0; boxX < board.Width - 1; boxX += (boxWidth - 1))
			{
				for (var boxY = 0; boxY < board.Height - 1; boxY += (boxHeight - 1))
				{
					var set = new List<PossibleValues>();
					for (var x = 0; x < boxWidth; x++)
					{
						for (var y = 0; y < boxHeight; y++)
						{
							set.Add(board.GetPossibleValues(x + boxX, y + boxY));
						}
					}

					if (!UniqueConstraint.HandleSet(board, set))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
