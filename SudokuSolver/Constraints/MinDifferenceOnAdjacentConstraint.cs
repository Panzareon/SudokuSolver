using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public class MinDifferenceOnAdjacentConstraint : IConstraint
	{
		private readonly int minDistance;
		private readonly Position[] positions;

		public IReadOnlyList<Position> MostImpactedPositions => positions;

		public MinDifferenceOnAdjacentConstraint(int minDistance, params Position[] positions)
		{
			if (minDistance < 0)
			{
				throw new ArgumentException("Cannot use negative minimum distance");
			}
			this.minDistance = minDistance;
			this.positions = positions;
		}

		public bool CanPlace(Board board, NextStep nextStep)
		{
			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				if (position.X != nextStep.SetX || position.Y != nextStep.SetY)
				{
					continue;
				}

				if (i > 0)
				{
					var previous = positions[i - 1];
					if (!CheckWithOther(previous))
					{
						return false;
					}
				}
				if (i < positions.Length - 1)
				{
					var next = positions[i + 1];
					if (!CheckWithOther(next))
					{
						return false;
					}
				}
			}

			return true;

			bool CheckWithOther(Position other)
			{
				var tile = board.GetTile(other);
				if (tile.IsSet)
				{
					var value = tile.Value;
					return HasMinDifference(nextStep.NextValue, value);
				}
				else
				{
					var possibleValues = board.GetPossibleValues(other.X, other.Y);
					var node = possibleValues.Values.First;
					while (node != null)
					{
						if (HasMinDifference(nextStep.NextValue, node.Value))
						{
							return true;
						}

						node = node.Next;
					}

					return false;
				}
			}
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			for (var i = 0; i < positions.Length - 1; i++)
			{
				var firstPosition = positions[i];
				var secondPosition = positions[i + 1];
				var first = board.GetTile(firstPosition);
				var second = board.GetTile(secondPosition);
				if (first.IsSet)
				{
					var value = first.Value;
					return this.RemoveNotPossibleValues(board, secondPosition, second, value, value);
				}
				else
				{
					var values = board.GetPossibleValues(firstPosition.X, firstPosition.Y);
					var minValue = values.Values.First;
					var maxValue = values.Values.Last;
					if (minValue == null || maxValue == null)
					{
						return false;
					}
					if (!this.RemoveNotPossibleValues(board, secondPosition, second, minValue.Value, maxValue.Value))
					{
						return false;
					}
				}

				if (second.IsSet)
				{
					var value = second.Value;
					return this.RemoveNotPossibleValues(board, firstPosition, first, value, value);
				}
				else
				{
					var values = board.GetPossibleValues(secondPosition.X, secondPosition.Y);
					var minValue = values.Values.First;
					var maxValue = values.Values.Last;
					if (minValue == null || maxValue == null)
					{
						return false;
					}

					if (!this.RemoveNotPossibleValues(board, firstPosition, first, minValue.Value, maxValue.Value))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool RemoveNotPossibleValues(Board board, Position otherPosition, Tile other, int minValue, int maxValue)
		{
			if (other.IsSet)
			{
				if (!HasMinDifference(other.Value, minValue) && !HasMinDifference(other.Value, maxValue))
				{
					return false;
				}
			}
			else
			{
				var otherValues = board.GetPossibleValues(otherPosition.X, otherPosition.Y);
				var node = otherValues.Values.First;
				while (node != null)
				{
					if (!HasMinDifference(node.Value, minValue) && !HasMinDifference(node.Value, maxValue))
					{
						var next = node.Next;
						otherValues.Remove(node);
						node = next;
					}
					else
					{
						node = node.Next;
					}
				}

				if (otherValues.Values.Count == 0)
				{
					return false;
				}
			}

			return true;
		}

		private bool HasMinDifference(int otherValue, int value)
		{
			var difference = value - otherValue;
			return (difference >= 0 || -difference >= minDistance) && (difference < 0 || difference >= minDistance);
		}
	}
}
