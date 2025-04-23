using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	public class PossibleValues
	{
		private readonly int index;

		public PossibleValues(int numberValues, int index)
		{
			for (var i = 1; i <= numberValues; i++)
			{
				this.Values.AddLast(i);
			}

			this.index = index;
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

		public void Remove(LinkedListNode<int> node)
		{
			this.Values.Remove(node);
		}

		public static PossibleValues Create(int value)
		{
			var possibleValue = new PossibleValues(0, -1);
			possibleValue.Values.AddFirst(value);
			return possibleValue;
		}
	}
}
