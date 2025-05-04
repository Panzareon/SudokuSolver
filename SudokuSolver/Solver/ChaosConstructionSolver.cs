using SudokuSolver.Constraints;
using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
				this.RemoveNotPossibleTileSetPositions(board);
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

				if (next.Board.CanSet(next.Board.TileSets.Sets[next.Step.Index], next.Step.Position, this.chaosConstructionConstraints))
				{
					var board = next.Step.Apply(next.Board);
					if (!this.RemoveNotPossibleTileSetPositions(board))
					{
						continue;
					}

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

		private bool RemoveNotPossibleTileSetPositions(Board board)
		{
			for (var i = 0; i < board.TileSets.Sets.Count; i++)
			{
				var tileSet = board.TileSets.Sets[i];
				var position = tileSet.PossiblePositions.First;
				while (position != null)
				{
					if (!tileSet.Positions.Contains(position.Value)
						&& !board.CanSet(tileSet, position.Value, this.chaosConstructionConstraints))
					{
						var next = position.Next;
						tileSet.RemovePossiblePosition(position);
						if (tileSet.PossiblePositions.Count < tileSet.MaxPositions)
						{
							return false;
						}

						position = next;
					}
					else
					{
						position = position.Next;
					}
				}

				if (tileSet.PossiblePositions.Count < tileSet.MaxPositions)
				{
					return false;
				}
			}

			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					if (!board.TileSets.Sets.Any(tileSet => tileSet.PossiblePositions.Contains(new Position(x, y))))
					{
						return false;
					}
				}
			}

			for (var i = 0; i < this.chaosConstructionConstraints.Length; i++)
			{
				if (!this.chaosConstructionConstraints[i].RemoveNotPossibleTileSetPositions(board))
				{
					return false;
				}
			}

			return true;
		}
		private record Next(Position Position, int Index)
		{
			public Next? GetNextValue(Board board)
			{
				var newIndex = Index + 1;
				while (board.TileSets.Sets[Index].Positions.Count != 0
					&& board.TileSets.Sets.Count > newIndex)
				{
					if (board.TileSets.Sets[newIndex].PossiblePositions.Contains(Position))
					{
						return new Next(Position, newIndex);
					}

					newIndex++;
				}

				return null;
			}

			public Next? GetNextPosition(Board board)
			{
				var current = this;
				do
				{
					current = current.GetNextPositionInternal(board);
				}
				while (current != null && board.TileSets.GetTileSetFromPosition(current.Position) != null);
				return current;
			}

			private Next? GetNextPositionInternal(Board board)
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
				newBoard.TileSets.AddPosition(Index, this.Position);
				for (var i = 0; i < newBoard.TileSets.Sets.Count; i++)
				{
					if (i != Index)
					{
						newBoard.TileSets.Sets[i].RemovePossiblePosition(this.Position);
					}
				}
				return newBoard;
			}
		}
	}
}
