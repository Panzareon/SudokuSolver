using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Model
{
	[DebuggerDisplay("{Display}")]
	public struct Tile
	{
		public bool IsSet { get; set; }
		public int Value { get; set; }

		public string Display => this.IsSet ? this.Value.ToString() : " ";
	}
}
