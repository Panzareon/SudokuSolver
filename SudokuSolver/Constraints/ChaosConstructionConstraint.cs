using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class ChaosConstructionConstraint(int SizeOfRegion) : IChaosConstructionConstraint
	{
		public IEnumerable<Position> MostImpactedPositions { get; } = [];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < board.TileSets.Count; i++)
			{
				var tileSet = board.TileSets[i];
				if (tileSet.Positions.Count < 2)
				{
					continue;
				}

				if (tileSet.Positions.Contains(new Position(nextStep.SetX, nextStep.SetY)))
				{
					var position = tileSet.Positions.First;
					while (position != null)
					{
						if (position.Value.X != nextStep.SetX || position.Value.Y != nextStep.SetY)
						{
							var tile = board.GetTile(position.Value);
							if (tile.IsSet && tile.Value == nextStep.NextValue)
							{
								return false;
							}
						}
						position = position.Next;
					}
				}
			}

			return true;
		}

		public bool CanSet(Board board, TileSet tileSet, Position position)
		{
			if (tileSet.Positions.Count >= SizeOfRegion)
			{
				return false;
			}

			var positionsToConnect = new LinkedList<Position>(tileSet.Positions);
			positionsToConnect.AddFirst(position);
			var connectedSubGroups = new List<LinkedList<Position>>();
			while (positionsToConnect.First != null)
			{
				var first = positionsToConnect.First;
				var connectedGroup = new LinkedList<Position>();
				connectedGroup.AddFirst(first.Value);
				connectedSubGroups.Add(connectedGroup);
				positionsToConnect.Remove(first);
				var current = connectedGroup.First;
				while (current != null)
				{
					AddAdjacent(current.Value, positionsToConnect, connectedGroup);
					current = current.Next;
				}
			}

			var remainingTiles = tileSet.MaxPositions - (tileSet.Positions.Count + 1);
			if (remainingTiles == 0 && connectedSubGroups.Count > 1)
			{
				return false;
			}
			if (connectedSubGroups.Count == 1)
			{
				return true;
			}

			return CheckCanBeCombined(board, connectedSubGroups, remainingTiles, tileSet);
		}

		private bool CheckCanBeCombined(Board board, List<LinkedList<Position>> connectedSubGroups, int remainingTiles, TileSet tileSet)
		{
			int checkSize = board.Width * board.Height;
			var distancesBase = InitializePathfindingArray(board, tileSet);
			var overallDistances = new int[checkSize];
			var toCheck = new Queue<Position>();
			for (var i = 0; i < connectedSubGroups.Count; i++)
			{
				var distances = new int[board.Width * board.Height];
				distancesBase.CopyTo(distances, 0);
				var current = connectedSubGroups[i].First;
				while (current != null)
				{
					distances[current.Value.X + current.Value.Y * board.Width] = 1;
					toCheck.Enqueue(current.Value);
					current = current.Next;
				}
				while (toCheck.TryDequeue(out var next))
				{
					PathfindingStep(board, remainingTiles, toCheck, distances, next);
				}

				for (var j = 0; j < checkSize; j++)
				{
					var value = distances[j];
					if (value == 0)
					{
						overallDistances[j] = -1;
					}
					else if (value > 0)
					{
						var currentDistance = overallDistances[j];
						if (currentDistance >= 0)
						{
							var newValue = currentDistance + value - 2;
							overallDistances[j] = newValue;
						}
					}
				}
			}

			for (var i = 0; i < checkSize; i++)
			{
				// The overall distances contain one less than the number of tiles needed to place
				if (overallDistances[i] >= 0 && overallDistances[i] < remainingTiles)
				{
					return true;
				}
			}

			return false;
		}

		private static int[] InitializePathfindingArray(Board board, TileSet tileSet)
		{
			var checkSize = board.Width * board.Height;
			var distancesBase = new int[checkSize];
			for (int i = 0; i < board.TileSets.Count; i++)
			{
				var otherTileSet = board.TileSets[i];
				if (otherTileSet == tileSet)
				{
					continue;
				}

				var current = tileSet.Positions.First;
				while (current != null)
				{
					distancesBase[current.Value.X + current.Value.Y * board.Width] = -1;
					current = current.Next;
				}
			}

			return distancesBase;
		}

		private static void PathfindingStep(Board board, int remainingTiles, Queue<Position> toCheck, int[] distances, Position next)
		{
			var currentValue = distances[next.X + next.Y * board.Width];
			if (next.X > 0)
			{
				var adjacent = new Position(next.X - 1, next.Y);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.X < board.Width - 1)
			{
				var adjacent = new Position(next.X + 1, next.Y);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.Y > 0)
			{
				var adjacent = new Position(next.X, next.Y - 1);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.Y < board.Height - 1)
			{
				var adjacent = new Position(next.X, next.Y + 1);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
		}

		private static void CheckAdjacent(Board board, int remainingTiles, int[] distances, Queue<Position> toCheck, int currentValue, Position adjacent)
		{
			var adjacentPosition = adjacent.X + adjacent.Y * board.Width;
			var adjacentValue = distances[adjacentPosition];
			if (adjacentValue >= 0 && adjacentValue > currentValue + 1)
			{
				distances[adjacentPosition] = currentValue + 1;
				if (currentValue + 1 < remainingTiles)
				{
					toCheck.Enqueue(adjacent);
				}
			}
		}

		private void AddAdjacent(Position value, LinkedList<Position> positionsToConnect, LinkedList<Position> connectedGroup)
		{
			var check = positionsToConnect.First;
			while (check != null)
			{
				if (value.IsAdjacent(check.Value))
				{
					connectedGroup.AddLast(check.Value);
					var next = check.Next;
					positionsToConnect.Remove(check);
					check = next;
				}
				else
				{
					check = check.Next;
				}
			}
		}

		public void InitializeBoard(Board board)
		{
			var numberOfRegions = board.Height * board.Width / SizeOfRegion;
			for (var i = 0; i < numberOfRegions; i++)
			{
				board.TileSets.Add(new TileSet(TileSetType.SudokuRegion, SizeOfRegion));
			}
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			for (var i = 0; i < board.TileSets.Count; i++)
			{
				var tileSet = board.TileSets[i];
				if (tileSet.Positions.Count > 1)
				{
					if (!UniqueConstraint.HandleSet(board, tileSet.Positions.ToArray()))
					{
						return false;
					}
				}

				if (!this.HasSpaceRemaining(board, tileSet))
				{
					return false;
				}
			}

			return true;
		}

		private bool HasSpaceRemaining(Board board, TileSet tileSet)
		{
			var remaining = tileSet.MaxPositions - tileSet.Positions.Count;
			if (remaining > 0)
			{
				return true;
			}

			var distances = InitializePathfindingArray(board, tileSet);
			var toCheck = new Queue<Position>();
			var current = tileSet.Positions.First;
			while (current != null)
			{
				toCheck.Enqueue(current.Value);
				distances[current.Value.X + current.Value.Y * board.Width] = 1;
				current = current.Next;
			}

			var numberOfSpaces = 0;
			while (toCheck.TryDequeue(out var next))
			{
				numberOfSpaces++;
				PathfindingStep(board, remaining, toCheck, distances, next);
				if (toCheck.Count + numberOfSpaces >= tileSet.MaxPositions)
				{
					return true;
				}
			}

			return false;
		}
	}
}
