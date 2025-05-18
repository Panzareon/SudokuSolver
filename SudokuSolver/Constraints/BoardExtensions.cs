using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public static class BoardExtensions
	{
		/// <summary>
		/// Gets the possible sum of the positions in the <paramref name="region"/>.
		/// </summary>
		/// <param name="board">The board to check.</param>
		/// <param name="region">The region to check.</param>
		/// <param name="index">An index to specify an explicit value for. -1 if this should be ignored.</param>
		/// <param name="nextValue">The value to test at the given index.</param>
		/// <returns></returns>
		public static ISet<int> GetPossibleSumsOfPositions(this Board board, IReadOnlyList<Position> region, int index = -1, int nextValue = 0)
		{
			var possibleValues = new HashSet<int> { 0 };
			for (var i = 0; i < region.Count; i++)
			{
				if (index == i)
				{
					possibleValues = [.. possibleValues.Select(x => x + nextValue)];
					continue;
				}

				var position = region[i];
				var possibleValuesAtPosition = board.GetPossibleValues(position.X, position.Y);
				var previousPossibleValues = possibleValues;
				possibleValues = new HashSet<int>();
				var current = possibleValuesAtPosition.Values.First;
				while (current != null)
				{
					foreach (var previousPossibleValue in previousPossibleValues)
					{
						var newValue = previousPossibleValue + current.Value;
						possibleValues.Add(newValue);
					}
					current = current.Next;
				}
			}

			return possibleValues;
		}

	}
}
