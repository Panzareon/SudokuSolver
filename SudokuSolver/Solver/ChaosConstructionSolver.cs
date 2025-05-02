using SudokuSolver.Constraints;
using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class ChaosConstructionSolver
	{
		private readonly Board board;
		private readonly IConstraint[] constraints;
		private readonly IChaosConstructionConstraint[] chaosConstructionConstraints;
		private readonly LogicSolver logicSolver;

		public ChaosConstructionSolver(Board board, params IConstraint[] constraints)
		{
			this.board = board;
			this.constraints = constraints;
			this.chaosConstructionConstraints = constraints.OfType<IChaosConstructionConstraint>().ToArray();
			this.logicSolver = new LogicSolver(constraints);
			for (var i = 0; i < chaosConstructionConstraints.Length; i++)
			{
				chaosConstructionConstraints[i].InitializeBoard(board);
			}
		}


		public IEnumerable<Board> Solve(int logicRepeats = 2)
		{
			var boardToCheck = new Stack<(Board Board, Next Step)>();
			for (var i = 0; i < logicRepeats; i++)
			{
				if (!this.logicSolver.Solve(this.board))
				{
					yield break;
				}
			}

			var initialStep = new Next(new Position(0, 0), 0);

			boardToCheck.Push((this.board, initialStep));
			while (boardToCheck.Count > 0)
			{
				var next = boardToCheck.Pop();
				var nextStep = next.Step.GetNextValue(next.Board);

				if (nextStep != null)
				{
					boardToCheck.Push((next.Board, nextStep));
				}

				if (next.Board.CanSet(next.Board.TileSets[next.Step.Index], next.Step.Position, this.chaosConstructionConstraints))
				{
					var board = next.Step.Apply(next.Board);
					var result = new BacktraceSolver(board, this.constraints).Solve(1);
					using var enumerator = result.GetEnumerator();
					if (!enumerator.MoveNext())
					{
						continue;
					}

					nextStep = next.Step.GetNextPosition(board);
					if (nextStep == null)
					{
						do
						{
							yield return enumerator.Current;
						}
						while (enumerator.MoveNext());
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

		private record Next(Position Position, int Index)
		{
			public Next? GetNextValue(Board board)
			{
				if (board.TileSets[Index].Positions.Count == 0
					|| board.TileSets.Count <= Index + 1)
				{
					return null;
				}

				return new Next(Position, Index + 1);
			}

			public Next? GetNextPosition(Board board)
			{
				if (Position.X < board.Width - 1)
				{
					return new Next(new Position(Position.X + 1, Position.Y), 0);
				}
				if (Position.Y < board.Height - 1)
				{
					return new Next(new Position(0, Position.Y + 1), 0);
				}

				return null;
			}

			public Board Apply(Board board)
			{
				var newBoard = board.Clone();
				newBoard.TileSets[Index].Positions.AddLast(this.Position);
				return newBoard;
			}
		}
	}
}
