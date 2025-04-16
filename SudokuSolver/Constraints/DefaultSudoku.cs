using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
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

		private bool NumberExistsInBox(Board board, NextStep nextStep)
		{
			var xStart = nextStep.SetX / board.BoxSize * board.BoxSize;
			var yStart = nextStep.SetY / board.BoxSize * board.BoxSize;

			for (var x = 0; x < board.BoxSize; x++)
			{
				for (var y = 0; y < board.BoxSize; y++)
				{
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
				var tile = board.GetTile(nextStep.SetX, y);
				if (tile.IsSet && tile.Value == nextStep.NextValue)
				{
					return true;
				}
			}

			return false;
		}
	}
}
