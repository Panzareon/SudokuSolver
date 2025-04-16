using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class NextStep
	{
		private readonly Board board;

		public NextStep(Board board)
		{
			this.board = board;
		}

		public int SetX { get; set; }

		public int SetY { get; set; }

		public int NextValue { get; set; } = 1;

		public bool IsAt(Position position)
		{
			return position.X == this.SetX && position.Y == this.SetY;
		}

		public static NextStep? GetNext(Board board)
		{
			var nextStep = new NextStep(board);
			return nextStep.NextEmptyTile(nextStep);
		}

		public Board Apply()
		{
			var newBoard = this.board.Clone();
			if (this.SetX > 0 || this.SetY > 0)
			{
				Debug.Assert(newBoard.GetTile(this.SetX - 1, this.SetY).IsSet);
			}
			newBoard.SetTile(this.SetX, this.SetY, new Tile { Value = this.NextValue, IsSet = true });
			return newBoard;
		}

		public NextStep? GetNextValue()
		{
			if (this.NextValue < board.MaxNumber)
			{
				return new NextStep(this.board)
				{
					SetX = this.SetX,
					SetY = this.SetY,
					NextValue = this.NextValue + 1,
				};
			}

			return null;
		}

		private NextStep? NextEmptyTile(NextStep? nextTile)
		{
			while (nextTile != null && this.board.GetTile(nextTile.SetX, nextTile.SetY).IsSet)
			{
				nextTile = nextTile.NextTile();
			}

			return nextTile;
		}

		private NextStep? NextTile()
		{
			if (this.SetX + 1 < this.board.Width)
			{
				return new NextStep(this.board)
				{
					SetX = this.SetX + 1,
					SetY = this.SetY,
				};
			}

			if (this.SetY + 1 < this.board.Height)
			{
				return new NextStep(this.board)
				{
					SetY = this.SetY + 1,
				};
			}

			return null;
		}
	}
}
