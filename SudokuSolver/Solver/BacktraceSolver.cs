using SudokuSolver.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class BacktraceSolver
	{
		private readonly Board board;
		private readonly IConstraint[] constraints;
		private readonly LogicSolver logicSolver;

		public BacktraceSolver(Board board, IConstraint[] constraints)
		{
			this.logicSolver = new LogicSolver(constraints);
			this.board = board;
			this.constraints = constraints;
		}

		public IEnumerable<Board> Solve()
		{
			var checkedBoards = new HashSet<Board>();
			var boardToCheck = new Stack<(Board Board, NextStep Step)>();
			if (!this.logicSolver.Solve(this.board))
			{
				yield break;
			}

			var initialStep = NextStep.GetNext(board);
			if (initialStep == null)
			{
				if (this.IsFinished(board))
				{
					yield return board;
				}

				yield break;
			}

			boardToCheck.Push((this.board, initialStep));
			while (boardToCheck.Count > 0)
			{
				var next = boardToCheck.Pop();
				var nextStep = next.Step.GetNextValue();

				if (nextStep != null)
				{
					boardToCheck.Push((next.Board, nextStep));
				}

				if (next.Board.CanPlace(next.Step, this.constraints))
				{
					var board = next.Step.Apply();
					if (!this.logicSolver.Solve(board))
					{
						continue;
					}
					Debug.Assert(checkedBoards.Add(board), "The boards to check should be unique");
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
	}
}
