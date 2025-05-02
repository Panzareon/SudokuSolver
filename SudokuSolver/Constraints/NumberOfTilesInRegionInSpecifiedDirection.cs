using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
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

			var(tileSet, current, _) = GetCurrentState(board);
			if (tileSet == null)
			{
				return true;
			}

			return nextStep.NextValue >= current && nextStep.NextValue <= current + (tileSet.MaxPositions - tileSet.Positions.Count);
		}

		public bool CanSet(Board board, TileSet tileSet, Position position)
		{
			var (currentTileSet, current, max) = GetCurrentState(board);
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

			if (possibleValues.Values.Last?.Value >= current && possibleValues.Values.Last.Value <= max)
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
			var (currentTileSet, current, max) = GetCurrentState(board);
			if (currentTileSet == null)
			{
				return true;
			}

			var possibleValues = board.GetPossibleValues(Start.X, Start.Y);
			return CanStillFulfillCondition(currentTileSet, current, max, possibleValues);
		}
		private (TileSet? tileSet, int current, int max) GetCurrentState(Board board)
		{
			var tileSet = board.TileSets.FirstOrDefault(x => x.Positions.Contains(Start));
			if (tileSet == null)
			{
				return (null, 0, 0);
			}

			var otherPositions = board.TileSets.Except([tileSet]).SelectMany(x => x.Positions).ToList();
			var current = 1;
			var possibleAdditional = 0;
			for (var i = 0; i < Directions.Length; i++)
			{
				var direction = Directions[i];
				var currentPosition = Start + direction;
				var isBroken = false;
				while (currentPosition.X >= 0 && currentPosition.X <= board.Width && currentPosition.Y >= 0 && currentPosition.Y <= board.Height)
				{
					if (!isBroken && tileSet.Positions.Contains(currentPosition))
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
						isBroken = true;
					}

					currentPosition += direction;
				}
			}

			return (tileSet, current, current + possibleAdditional);
		}

	}
}
