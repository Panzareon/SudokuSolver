using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
	public class OrderedNextStep : NextStep
	{
		private readonly LinkedList<int> orderedValues;

		public OrderedNextStep(Board board, LinkedList<int> orderedValues)
			: base(board)
		{
			this.orderedValues = orderedValues;
			this.NextValue = orderedValues.First();
		}

		public override NextStep? GetNextValue()
		{
			var possibleValues = this.board.GetPossibleValues(this.SetX, this.SetY);
			var nextValue = orderedValues.Find(this.NextValue)?.Next;
			while (nextValue != null)
			{
				if (possibleValues.Values.Contains(nextValue.Value))
				{
					return new OrderedNextStep(this.board, this.orderedValues)
					{
						SetX = this.SetX,
						SetY = this.SetY,
						Index = this.Index,
						NextValue = nextValue.Value,
					};
				}

				nextValue = nextValue.Next;
			}

			return null;
		}
	}
}
