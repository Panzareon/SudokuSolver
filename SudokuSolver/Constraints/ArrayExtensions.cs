using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Constraints
{
	public static class ArrayExtensions
	{
		public static int FindIndex<T>(this T[] array, T value)
			where T : notnull
		{
			for (var i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(value))
				{
					return i;
				}
			}

			return -1;
		}
	}
}
