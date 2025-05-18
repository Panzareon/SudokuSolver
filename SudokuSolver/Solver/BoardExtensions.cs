using SudokuSolver.Constraints;
using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public static class BoardExtensions
	{
		public static bool CanPlace(this Board board, NextStep nextStep, IConstraint[] constraints)
		{
			for (var i = 0; i < constraints.Length; i++)
			{
				if (!constraints[i].CanPlace(board, nextStep))
				{
					return false;
				}
			}

			return true;
		}
		public static bool CanSet(this Board board, TileSet set, Position position, IChaosConstructionConstraint[] constraints)
		{
			for (var i = 0; i < constraints.Length; i++)
			{
				if (!constraints[i].CanSet(board, set, position))
				{
					return false;
				}
			}

			return true;
		}

		public static void EnsureIsInitialized(this Board board, IConstraint[] constaints)
		{
			if (board.IsInitialized)
			{
				return;
			}

			foreach (var initialization in constaints.OfType<IBoxDefiningConstraint>())
			{
				initialization.InitializeBoxPositions(board);
			}


			foreach (var chaosConstructionConstraint in constaints.OfType<IChaosConstructionConstraint>())
			{
				chaosConstructionConstraint.InitializeBoard(board);
			}

			board.IsInitialized = true;
		}
	}
}
