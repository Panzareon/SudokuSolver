using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public static class ConstraintFactory
	{
		public static IConstraint GermanWhisper(params Position[] positions)
		{
			return new MinDifferenceOnAdjacentConstraint(5, positions);
		}

		public static IEnumerable<IConstraint> KillerCage(int sum, params Position[] positions)
		{
			yield return new SumConstraint(sum, positions);
			yield return new UniqueConstraint(positions);
		}
	}
}
