using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	/// <summary>
	/// A constraint that specifies, that the values on each segment of the line defined by the <paramref name="positions"/>
	/// split by region boundaries must sum to the same values.
	/// </summary>
	/// <param name="positions"></param>
	public class RegionSumConstraint(params Position[] positions) : IConstraint
	{
		public IEnumerable<Position> MostImpactedPositions => positions;

		public bool CanPlace(Board board, NextStep nextStep)
		{
			var index = positions.FindIndex(new Position(nextStep.SetX, nextStep.SetY));
			if (index < 0)
			{
				return true;
			}

			var regions = this.SplitByRegion(board);
			if (regions == null)
			{
				return true;
			}

			var possibleValues = board.GetPossibleSumsOfPositions(regions[0], index, nextStep.NextValue);
			index -= regions[0].Count;
			for (var i = 1; i < regions.Count; i++)
			{
				var region = regions[i];
				var newPossibleValues = board.GetPossibleSumsOfPositions(region, index, nextStep.NextValue);
				index -= region.Count;
				foreach (var possibleValueToCheck in possibleValues.ToArray())
				{
					if (!newPossibleValues.Contains(possibleValueToCheck))
					{
						possibleValues.Remove(possibleValueToCheck);
					}
				}

				if (possibleValues.Count == 0)
				{
					return false;
				}
			}

			return possibleValues.Count > 0;
		}

		public bool RemoveNotPossibleValues(Board board)
		{
			return true;
		}
		private List<List<Position>>? SplitByRegion(Board board)
		{
			var result = new List<List<Position>>();
			var firstPosition = positions[0];
			var previousTileSet = board.TileSets.GetTileSetFromPosition(firstPosition);
			if (previousTileSet == null)
			{
				return null;
			}

			var currentRegion = new List<Position> { firstPosition };
			result.Add(currentRegion);
			for (var i = 1; i < positions.Length; i++)
			{
				var position = positions[i];
				var currentTileSet = board.TileSets.GetTileSetFromPosition(position);
				if (currentTileSet == null)
				{
					return null;
				}

				if (currentTileSet == previousTileSet)
				{
					currentRegion.Add(position);
				}
				else
				{
					currentRegion = new List<Position> { position };
					result.Add(currentRegion);
					previousTileSet = currentTileSet;
				}
			}

			return result;
		}
	}
}
