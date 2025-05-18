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
		public IReadOnlyList<Position> MostImpactedPositions { get; } = [];

		public bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < board.TileSets.Sets.Count; i++)
			{
				var tileSet = board.TileSets.Sets[i];
				if (tileSet.Positions.Count < 2)
				{
					continue;
				}

				if (tileSet == board.TileSets.GetTileSetFromPosition(new Position(nextStep.SetX, nextStep.SetY)))
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
			var (isValid, connectedSubGroups, remainingTiles) = this.GenerateConnectedSubGroups(tileSet, positionsToConnect);
			if (!isValid)
			{
				return false;
			}

			if (connectedSubGroups.Count == 1)
			{
				return true;
			}

			return CheckCanBeCombined(board, connectedSubGroups, remainingTiles, tileSet);
		}

		private (bool isValid, List<LinkedList<Position>> connectedSubGroups, int remainingTiles) GenerateConnectedSubGroups(TileSet tileSet, LinkedList<Position> positionsToConnect)
		{
			var positionsCount = positionsToConnect.Count;
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

			var remainingTiles = tileSet.MaxPositions - positionsCount;
			if (remainingTiles == 0 && connectedSubGroups.Count > 1)
			{
				return (false, connectedSubGroups, remainingTiles);
			}

			return (true, connectedSubGroups, remainingTiles);
		}

		public bool RemoveNotPossibleTileSetPositions(Board board)
		{
			for (var i = 0; i < board.TileSets.Sets.Count; i++)
			{
				var tileSet = board.TileSets.Sets[i];
				if (tileSet.Positions.Count == 0)
				{
					continue;
				}

				var positionsToConnect = new LinkedList<Position>(tileSet.Positions);
				var (isValid, connectedSubGroups, remainingTiles) = this.GenerateConnectedSubGroups(tileSet, positionsToConnect);
				if (!isValid)
				{
					return false;
				}

				if (!CheckCanBeCombined(board, connectedSubGroups, remainingTiles, tileSet, true))
				{
					return false;
				}
			}

			return true;
		}
		private bool CheckCanBeCombined(Board board, List<LinkedList<Position>> connectedSubGroups, int remainingTiles, TileSet tileSet, bool updateBoard = false)
		{
			int checkSize = board.Width * board.Height;
			var distancesBase = InitializePathfindingArray(board, tileSet);
			var overallDistances = new int[checkSize];
			distancesBase.CopyTo(overallDistances, 0);
			var toCheck = new Queue<(Position Position, int Value)>();
			for (var i = 0; i < connectedSubGroups.Count; i++)
			{
				var distances = new int[board.Width * board.Height];
				distancesBase.CopyTo(distances, 0);
				var current = connectedSubGroups[i].First;
				while (current != null)
				{
					distances[current.Value.X + current.Value.Y * board.Width] = 1;
					toCheck.Enqueue((current.Value, 1));
					current = current.Next;
				}

				// the positions already in the tile set have value 1, so adjacent tiles that can be set will have value 2
				var currentlyProcessingValue = 2;
				Position? singlePosition = null;
				while (toCheck.TryDequeue(out var next))
				{
					PathfindingStep(board, remainingTiles, toCheck, distances, next);
					if (updateBoard && next.Value == currentlyProcessingValue)
					{
						if (singlePosition == null)
						{
							singlePosition = next.Position;
							if (!toCheck.TryPeek(out var peek) || peek.Value == currentlyProcessingValue + 1)
							{
								if (singlePosition != null)
								{
									board.TileSets.AddPosition(tileSet, singlePosition);
									currentlyProcessingValue++;
								}
								else
								{
									currentlyProcessingValue = -1;
								}
							}
						}
						else
						{
							currentlyProcessingValue = -1;
						}

					}
				}

				if (remainingTiles > 0)
				{
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
			}

			if (remainingTiles == 0)
			{
				return true;
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
			for (int i = 0; i < board.TileSets.Sets.Count; i++)
			{
				var otherTileSet = board.TileSets.Sets[i];
				if (otherTileSet == tileSet)
				{
					continue;
				}

				var current = otherTileSet.Positions.First;
				while (current != null)
				{
					distancesBase[current.Value.X + current.Value.Y * board.Width] = -1;
					current = current.Next;
				}
			}

			return distancesBase;
		}

		private static void PathfindingStep(Board board, int remainingTiles, Queue<(Position Position, int Value)> toCheck, int[] distances, (Position Position, int Value) next)
		{
			var currentValue = next.Value;
			if (next.Position.X > 0)
			{
				var adjacent = new Position(next.Position.X - 1, next.Position.Y);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.Position.X < board.Width - 1)
			{
				var adjacent = new Position(next.Position.X + 1, next.Position.Y);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.Position.Y > 0)
			{
				var adjacent = new Position(next.Position.X, next.Position.Y - 1);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
			if (next.Position.Y < board.Height - 1)
			{
				var adjacent = new Position(next.Position.X, next.Position.Y + 1);
				CheckAdjacent(board, remainingTiles, distances, toCheck, currentValue, adjacent);
			}
		}

		private static void CheckAdjacent(Board board, int remainingTiles, int[] distances, Queue<(Position Position, int Value)> toCheck, int currentValue, Position adjacent)
		{
			var adjacentPosition = adjacent.X + adjacent.Y * board.Width;
			var adjacentValue = distances[adjacentPosition];
			if (adjacentValue >= 0 && (adjacentValue == 0 || adjacentValue > currentValue + 1))
			{
				distances[adjacentPosition] = currentValue + 1;
				if (currentValue + 1 < remainingTiles)
				{
					toCheck.Enqueue((adjacent, currentValue + 1));
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
				board.TileSets.Sets.Add(new TileSet(TileSetType.SudokuRegion, SizeOfRegion, board));
			}
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			for (var i = 0; i < board.TileSets.Sets.Count; i++)
			{
				var tileSet = board.TileSets.Sets[i];
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
			var toCheck = new Queue<(Position Position, int Value)>();
			var current = tileSet.Positions.First;
			while (current != null)
			{
				toCheck.Enqueue((current.Value, 1));
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
