﻿using SudokuSolver.Model;
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
		private readonly IReadOnlyDictionary<Position, LinkedList<int>>? preferredValues;

		public BacktraceSolver(Board board, IConstraint[] constraints)
		{
			this.logicSolver = new LogicSolver(constraints);
			this.board = board;
			this.constraints = constraints;
		}

		private BacktraceSolver(Board board, IConstraint[] constraints, IReadOnlyDictionary<Position, LinkedList<int>> preferredValues)
			: this(board, constraints)
		{
			this.preferredValues = preferredValues;
		}

		public Board? SolveFixedValues()
		{
			var possibleBoards = this.Solve().Take(2).ToList();
			if (possibleBoards.Count == 1)
			{
				return possibleBoards[0];
			}

			if (possibleBoards.Count == 0)
			{
				return null;
			}

			var result = this.board.Clone();
			Dictionary<Position, LinkedList<int>> valuesToCheck = [];
			for (var x = 0; x < result.Width; x++)
			{
				for (var y = 0; y < result.Height; y++)
				{
					var possibleValues = result.GetPossibleValues(x, y).Values.Except(possibleBoards.Select(possibleBoard => possibleBoard.GetTile(x, y).Value));
					valuesToCheck[new Position(x, y)] = new LinkedList<int>(possibleValues);
				}
			}
			for (var x = 0; x < result.MaxNumber * 2; x++)
			{
				var possibleResult = new BacktraceSolver(result, this.constraints, valuesToCheck).Solve().First();
				if (!RemoveValuesToCheck(0, 0, valuesToCheck, possibleResult))
				{
					break;
				}
			}

			for (var x = 0; x < result.Width; x++)
			{
				for (var y = 0; y < result.Height; y++)
				{
					if (result.GetTile(x, y).IsSet)
					{
						continue;
					}
					this.CheckTile(result, x, y, valuesToCheck);
				}
			}

			return result;
		}

		private void CheckTile(Board result, int x, int y, Dictionary<Position, LinkedList<int>> valuesToCheck)
		{
			var possibleValues = valuesToCheck[new Position(x, y)];
			var hasUpdated = false;
			while (possibleValues.First != null)
			{
				var copy = result.Clone();
				var possibleValue = possibleValues.First;
				possibleValues.Remove(possibleValue);
				copy.SetTile(x, y, new Tile { Value = possibleValue.Value, IsSet = true });
				var possibleResult = new BacktraceSolver(copy, this.constraints).Solve().FirstOrDefault();
				if (possibleResult == null)
				{
					result.GetPossibleValues(x, y).RemoveValue(possibleValue.Value);
					hasUpdated = true;
				}
				else
				{
					RemoveValuesToCheck(x, y, valuesToCheck, possibleResult);
				}
			}

			if (hasUpdated)
			{
				var logicSolverResult = new LogicSolver(this.constraints).Solve(result);
				Debug.Assert(logicSolverResult, "This update should not have changed that there is a valid solution");
			}
		}

		/// <summary>
		/// Solves the board by trying out different values and tracing back when invalid sudokus are reached.
		/// </summary>
		/// <param name="logicRepeats">How often the initial logic step should be repeated to remove more possible values after other values are no longer available.</param>
		/// <returns>All finished boards which can be reached from the initial board while following the constraints.</returns>
		public IEnumerable<Board> Solve(int logicRepeats = 2)
		{
			board.EnsureIsInitialized(this.constraints);
			var checkedBoards = new HashSet<Board>();
			var boardToCheck = new Stack<(Board Board, NextStep Step)>();
			for (var i = 0; i < logicRepeats; i++)
			{
				if (!this.logicSolver.Solve(this.board))
				{
					yield break;
				}
			}

			var nextStepProvider = new NextStepProvider(this.board, this.constraints, this.preferredValues);
			var initialStep = nextStepProvider.GetInitial();
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
					nextStep = nextStepProvider.NextEmptyTile(board, next.Step);
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

		private static bool RemoveValuesToCheck(int x, int y, Dictionary<Position, LinkedList<int>> valuesToCheck, Board possibleResult)
		{
			var hasRemovedValue = false;
			for (var updateX = x; updateX < possibleResult.Width; updateX++)
			{
				for (var updateY = x == updateX ? y : 0; updateY < possibleResult.Height; updateY++)
				{
					if (valuesToCheck[new Position(updateX, updateY)].Remove(possibleResult.GetTile(updateX, updateY).Value))
					{
						hasRemovedValue = true;
					}
				}
			}

			return hasRemovedValue;
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
