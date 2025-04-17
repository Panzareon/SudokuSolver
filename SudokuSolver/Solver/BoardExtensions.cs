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
	}
}
