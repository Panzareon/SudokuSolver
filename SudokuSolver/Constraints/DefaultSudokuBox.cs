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
	public class DefaultSudokuBox(int boxWidth = 3, int boxHeight = 3) : IBoxDefiningConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => [];

		public int BoxWidth => boxWidth;

		public int BoxHeight => boxHeight;

		public bool CanPlace(Board board, NextStep nextStep)
		{
			if (this.NumberExistsInBox(board, nextStep))
			{
				return false;
			}

			return true;
		}

		public void InitializeBoxPositions(Board board)
		{
			for (var boxX = 0; boxX < board.Width; boxX += boxWidth)
			{
				for (var boxY = 0; boxY < board.Height; boxY += boxHeight)
				{
					var positions = new LinkedList<Position>();
					for (var x = 0; x < boxWidth; x++)
					{
						for (var y = 0; y < boxHeight; y++)
						{
							positions.AddLast(new Position(x + boxX, y + boxY));
						}
					}

					board.TileSets.AddTileSet(TileSetType.SudokuRegion, positions);
				}
			}
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return RemoveBoxGroups(board);
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
