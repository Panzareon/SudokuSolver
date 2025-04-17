using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public class PossibleValues
	{
		public PossibleValues(int numberValues)
		{
			for (var i = 1; i <= numberValues; i++)
			{
				this.Values.AddLast(i);
			}
		}
		public LinkedList<int> Values { get; } = new();

		public void RemoveValues(IEnumerable<int> values)
		{
			foreach (var value in values)
			{
				this.RemoveValue(value);
			}
		}

		public void RemoveValue(int value)
		{
			this.Values.Remove(value);
		}
	}
}
