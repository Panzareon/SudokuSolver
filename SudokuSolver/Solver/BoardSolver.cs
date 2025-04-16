using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class BoardSolver
	{
		private readonly Board board;
		private readonly IConstraint[] constraints;

		public BoardSolver(Board board, IConstraint[] constraints)
		{
			this.board = board;
			this.constraints = constraints;
		}

		public IEnumerable<Board> Solve()
		{
			var checkedBoards = new HashSet<Board>();
			var boardToCheck = new Stack<(Board Board, NextStep Step)>();
			boardToCheck.Push((this.board, new NextStep(this.board)));
			while (boardToCheck.Count > 0)
			{
				var next = boardToCheck.Pop();
				var nextStep = next.Step.GetNextValue();

				if (nextStep != null)
				{
					boardToCheck.Push((next.Board, nextStep));
				}

				if (this.CanPlace(next.Board, next.Step))
				{
					var board = next.Step.Apply();
					if (!checkedBoards.Add(board))
					{
						var other = checkedBoards.FirstOrDefault(x => x.Equals(board));
						Debug.Assert(false, "The boards to check should be unique");
					}
					nextStep = NextStep.GetNext(board);
					if (nextStep == null)
					{
						Debug.Assert(this.IsFinished(board));
						yield return board;
					}
					else
					{
						boardToCheck.Push((board, nextStep));
					}
				}
			}
		}

		private bool IsFinished(Board board)
		{
			for (int y = 0; y < board.Height; y++)
			{
				for (int x = 0; x < board.Width; x++)
				{
					if (!board.GetTile(x, y).IsSet)
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < constraints.Length; i++)
			{
				if (!constraints[i].CanPlace(board, nextStep))
				{
					return false;
				}
			}

			return true;
		}
	}
}
