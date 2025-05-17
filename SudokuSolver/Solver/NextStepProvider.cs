using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class NextStepProvider
	{
		private readonly Board initialBoard;
		private readonly IReadOnlyDictionary<Position, LinkedList<int>>? preferredValues;
		private Position[] nextStepOrder;

		public NextStepProvider(Board board, IConstraint[] constraints, IReadOnlyDictionary<Position, LinkedList<int>>? preferredValues)
		{
			this.initialBoard = board;
			this.preferredValues = preferredValues?.ToDictionary(x => x.Key, x => AddMissingValues(x.Value, board));
			var positions = new Dictionary<Position, int>();
			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					var tileValue = board.MaxNumber - board.GetPossibleValues(x, y).Values.Count;
					if (preferredValues != null && preferredValues[new Position(x, y)].Count == 0)
					{
						tileValue -= board.MaxNumber;
					}

					positions[new Position(x, y)] = tileValue;
				}
			}

			foreach (var constraint in constraints)
			{
				foreach (var position in constraint.MostImpactedPositions)
				{
					positions[position] += board.MaxNumber;
				}
			}

			this.nextStepOrder = positions.OrderByDescending(x => x.Value).Select(x => x.Key).ToArray();
		}

		private static LinkedList<int> AddMissingValues(LinkedList<int> preferredValues, Board board)
		{
			var orderedValues = new LinkedList<int>(preferredValues);
			for (var x = board.MinNumber; x <= board.MaxNumber; x++)
			{
				if (!preferredValues.Contains(x))
				{
					orderedValues.AddLast(x);
				}
			}

			return orderedValues;
		}

		public NextStep? NextEmptyTile(Board board, NextStep? currentTile)
		{
			while (currentTile != null && board.GetTile(currentTile.SetX, currentTile.SetY).IsSet)
			{
				currentTile = this.NextTile(board, currentTile);
			}

			return currentTile;
		}

		public NextStep? GetInitial()
		{
			var firstStep = this.GetStepAtIndex(this.initialBoard, 0);
			if (this.initialBoard.GetTile(firstStep.SetX, firstStep.SetY).IsSet)
			{
				return this.NextEmptyTile(this.initialBoard, firstStep);
			}

			return firstStep;
		}

		private NextStep? NextTile(Board board, NextStep currentStep)
		{
			if (currentStep.Index >= this.nextStepOrder.Length - 1)
			{
				return null;
			}
			return this.GetStepAtIndex(board, currentStep.Index + 1);
		}

		private NextStep GetStepAtIndex(Board board, int index)
		{
			var nextPosition = this.nextStepOrder[index];
			if (this.preferredValues != null)
			{
				return new OrderedNextStep(board, this.preferredValues[nextPosition])
				{
					SetX = nextPosition.X,
					SetY = nextPosition.Y,
					Index = index,
				};
			}
			return new NextStep(board)
			{
				SetX = nextPosition.X,
				SetY = nextPosition.Y,
				Index = index,
			};
		}
	}
}
