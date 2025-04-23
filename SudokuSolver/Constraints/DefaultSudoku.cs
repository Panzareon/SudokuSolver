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
	public class DefaultSudoku(int boxWidth = 3, int boxHeight = 3) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => [];

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
			var xStart = nextStep.SetX / boxWidth * boxWidth;
			var yStart = nextStep.SetY / boxHeight * boxHeight;

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

				if (!UniqueConstraint.HandleSet(board, set))
				{
					return false;
				}
			}

			return true;
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

				if (!UniqueConstraint.HandleSet(board, set))
				{
					return false;
				}
			}

			return true;
		}

		private bool RemoveBoxGroups(Board board)
		{
			for (var boxX = 0; boxX < board.Width; boxX += boxWidth)
			{
				for (var boxY = 0; boxY < board.Height; boxY += boxHeight)
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
