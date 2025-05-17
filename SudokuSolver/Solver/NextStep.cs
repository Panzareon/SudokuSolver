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
		protected readonly Board board;

		public NextStep(Board board)
		{
			this.board = board;
			this.NextValue = board.MinNumber;
		}

		public int Index { get; init; }
		public int SetX { get; init; }

		public int SetY { get; init; }

		public int NextValue { get; set; }

		public bool IsAt(Position position)
		{
			return position.X == this.SetX && position.Y == this.SetY;
		}

		public Board Apply()
		{
			var newBoard = this.board.Clone();
			newBoard.SetTile(this.SetX, this.SetY, new Tile { Value = this.NextValue, IsSet = true });
			return newBoard;
		}

		public virtual NextStep? GetNextValue()
		{
			var possibleValues = board.GetPossibleValues(this.SetX, this.SetY);
			var nextValue = this.NextValue;
			while (nextValue < board.MaxNumber)
			{
				nextValue++;
				if (possibleValues.Values.Contains(nextValue))
				{
					return new NextStep(this.board)
					{
						SetX = this.SetX,
						SetY = this.SetY,
						Index = this.Index,
						NextValue = nextValue,
					};
				}
			}

			return null;
		}

		public NextStep CopyToPosition(int index, Position position)
		{
			return new NextStep(this.board)
			{
				Index = index,
				SetX = position.X,
				SetY = position.Y,
			};
		}
	}
}
