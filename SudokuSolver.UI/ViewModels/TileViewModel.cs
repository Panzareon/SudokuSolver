using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	public class TileViewModel : ViewModelBase
	{
		private readonly Board board;

		public TileViewModel(Board board, int x, int y)
		{
			this.board = board;
			this.X = x;
			this.Y = y;
		}

		public string Display => this.board.GetTile(this.X, this.Y).Display;

		public int X { get; }

		public int Y { get; }
	}
}
