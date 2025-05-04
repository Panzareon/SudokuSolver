using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class NumberOfTilesInRegionInSpecifiedDirection(Position Start, params PositionDelta[] Directions) : IChaosConstructionConstraint
	{
		public IEnumerable<Position> MostImpactedPositions { get; } = [Start];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			if (nextStep.SetX != Start.X || nextStep.SetY != Start.Y)
			{
				return true;
			}

			var(tileSet, current, _, _) = GetCurrentState(board);
			if (tileSet == null)
			{
				return true;
			}

			return nextStep.NextValue >= current && nextStep.NextValue <= current + (tileSet.MaxPositions - tileSet.Positions.Count);
		}

		public bool CanSet(Board board, TileSet tileSet, Position position)
		{
			var (currentTileSet, current, max, _) = GetCurrentState(board);
			if (tileSet != currentTileSet)
			{
				return true;
			}

			var possibleValues = board.GetPossibleValues(Start.X, Start.Y);
			var isInDirection = this.IsInDirection(position);
			if (isInDirection)
			{
				current++;
			}
			return CanStillFulfillCondition(tileSet, current, max, possibleValues);
		}

		private static bool CanStillFulfillCondition(TileSet tileSet, int current, int max, PossibleValues possibleValues)
		{
			if (possibleValues.Values.Last?.Value == current)
			{
				return true;
			}

			if (possibleValues.Values.Last?.Value >= current && possibleValues.Values.First.Value <= max)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool IsInDirection(Position position)
		{
			var deltaX = position.X - Start.X;
			var deltaY = position.Y - Start.Y;
			for (var i = 0; i < Directions.Length; i++)
			{
				var direction = Directions[i];
				if (direction.deltaX == 0)
				{
					if (deltaX == 0 && deltaY / direction.deltaY > 0 && deltaY % direction.deltaY == 0)
					{
						return true;
					}
				}
				else
				{
					if (direction.deltaY == 0)
					{
						if (deltaY == 0 && deltaX / direction.deltaX > 0 && deltaX % direction.deltaX == 0)
						{
							return true;
						}
					}
					else
					{
						var steps = deltaX / direction.deltaX;
						if (steps > 0
							&& deltaY == steps * direction.deltaY
							&& deltaX % direction.deltaX == 0)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public void InitializeBoard(Board board)
		{
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			var (currentTileSet, current, max, possiblePositions) = GetCurrentState(board);
			if (currentTileSet == null)
			{
				return true;
			}

			if (possiblePositions == null)
			{
				throw new InvalidOperationException("possiblePositions should only be null if the currentTileSet is null");
			}

			var possibleValues = board.GetPossibleValues(Start.X, Start.Y);
			var currentValue = possibleValues.Values.First;
			while (currentValue != null)
			{
				if (currentValue.Value < current ||  currentValue.Value > max)
				{
					var next = currentValue.Next;
					possibleValues.Remove(currentValue);
					currentValue = next;
				}
				else
				{
					currentValue = currentValue.Next;
				}
			}

			if (possibleValues.Values.First == null)
			{
				return false;
			}

			if (possiblePositions.Count == 0)
			{
				return true;
			}

			if (max == possibleValues.Values.First.Value)
			{
				// All possible values need to be set
				foreach (var pair in possiblePositions.GroupBy(x => x.FromDirection))
				{
					foreach (var position in pair)
					{
						board.TileSets.AddPosition(currentTileSet, position.Position);
					}

					currentTileSet.RemovePossiblePosition(pair.Last().Position + Directions[pair.Key]);
				}
			}
			else
			{
				// Some of the possiblePositions need to be set
				var minToPlace = possibleValues.Values.First.Value - current;
				if (minToPlace == 0 || Directions.Length == 1 || possiblePositions.Select(x => x.FromDirection).Distinct().Count() == 1)
				{
					return true;
				}

				// We need to place additional, and there is only one direction where to place it
				for (var i = 0; i < minToPlace; i++)
				{
					board.TileSets.AddPosition(currentTileSet, possiblePositions[i].Position);
				}
			}

			return true;
		}

		public bool RemoveNotPossibleTileSetPositions(Board board)
		{
			return true;
		}
		private (TileSet? tileSet, int current, int max, List<(Position Position, int FromDirection)>? possiblePositions) GetCurrentState(Board board)
		{
			var tileSet = board.TileSets.GetTileSetFromPosition(Start);
			if (tileSet == null)
			{
				return (null, 0, 0, null);
			}

			var possiblePositions = new List<(Position Position, int DirectionIndex)>();
			var otherPositions = board.TileSets.Sets.Except([tileSet]).SelectMany(x => x.Positions).ToList();
			var current = 1;
			var possibleAdditional = 0;
			for (var i = 0; i < Directions.Length; i++)
			{
				var direction = Directions[i];
				var currentPosition = Start + direction;
				var isBroken = false;
				while (currentPosition.X >= 0 && currentPosition.X <= board.Width && currentPosition.Y >= 0 && currentPosition.Y <= board.Height)
				{
					if (!isBroken && tileSet == board.TileSets.GetTileSetFromPosition(currentPosition))
					{
						current++;
					}
					else if (otherPositions.Contains(currentPosition))
					{
						break;
					}
					else
					{
						possibleAdditional++;
						possiblePositions.Add((currentPosition, i));
						isBroken = true;
					}

					currentPosition += direction;
				}
			}

			return (tileSet, current, current + possibleAdditional, possiblePositions);
		}
	}
}
