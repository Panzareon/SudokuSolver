using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public interface IConstraint
	{
		IEnumerable<Position> MostImpactedPositions { get; }

		bool CanPlace(Board board, NextStep nextStep);

		/// <summary>
		/// Removes values from the <see cref="PossibleValues"/> of the different tiles in the <paramref name="board"/> which can not be used with this constraint.
		/// </summary>
		/// <param name="board">The board to check.</param>
		/// <returns>False, if the <paramref name="board"/> cannot fulfil this constraint anymore in the current state.</returns>
		bool RemoveNotPossibleValues(Board board);
	}
}
