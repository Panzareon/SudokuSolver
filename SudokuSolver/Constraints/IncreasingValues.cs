using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	/// <summary>
	/// A constraint that the values in the <paramref name="positions"/> need to strictly increase.
	/// </summary>
	/// <remarks>Also known as Thermometers.</remarks>
	/// <param name="positions">The positions in the order that they need to be increasing.</param>
	public class IncreasingValues(params Position[] positions) : IConstraint
	{
		public bool CanPlace(Board board, NextStep nextStep)
		{
			var index = positions.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			var minValue = 0;
			for (var i = 0; i < positions.Length; i++)
			{
				if (index == i)
				{
					if (minValue >= nextStep.NextValue)
					{
						return false;
					}

					minValue = nextStep.NextValue;
					continue;
				}

				var position = positions[i];
				var tile = board.GetTile(position);
				if (tile.IsSet)
				{
					if (tile.Value <= minValue)
					{
						return false;
					}

					minValue = tile.Value;
				}
				else
				{
					var possibleValues = board.GetPossibleValues(position.X, position.Y);
					var newMinValue = possibleValues.Values.Where(x => x > minValue).Take(1).ToArray();
					if (newMinValue.Length == 0)
					{
						return false;
					}

					minValue = newMinValue[0];
				}
			}

			return true;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return this.UpdateFromMinValue(board)
				&& UpdateFromMaxValue(board);
		}

		private bool UpdateFromMaxValue(Board board)
		{
			var maxValue = board.MaxNumber + 1;
			for (var i = positions.Length - 1; i >= 0; i--)
			{
				var position = positions[i];
				var tile = board.GetTile(position);
				if (tile.IsSet)
				{
					if (tile.Value >= maxValue)
					{
						return false;
					}

					maxValue = tile.Value;
				}
				else
				{
					var possibleValues = board.GetPossibleValues(position.X, position.Y);
					var newMaxValueSet = false;
					var node = possibleValues.Values.Last;
					while (node != null)
					{
						if (node.Value >= maxValue)
						{
							var next = node.Previous;
							possibleValues.Remove(node);
							node = next;
						}
						else
						{
							if (!newMaxValueSet)
							{
								newMaxValueSet = true;
								maxValue = node.Value;
							}

							node = node.Previous;
						}
					}

					if (!newMaxValueSet)
					{
						return false;
					}
				}
			}

			return true;
		}
		private bool UpdateFromMinValue(Board board)
		{
			var minValue = 0;
			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				var tile = board.GetTile(position);
				if (tile.IsSet)
				{
					if (tile.Value <= minValue)
					{
						return false;
					}

					minValue = tile.Value;
				}
				else
				{
					var possibleValues = board.GetPossibleValues(position.X, position.Y);
					var newMinValueSet = false;
					var node = possibleValues.Values.First;
					while (node != null)
					{
						if (node.Value <= minValue)
						{
							var next = node.Next;
							possibleValues.Remove(node);
							node = next;
						}
						else
						{
							if (!newMinValueSet)
							{
								newMinValueSet = true;
								minValue = node.Value;
							}

							node = node.Next;
						}
					}

					if (!newMinValueSet)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
