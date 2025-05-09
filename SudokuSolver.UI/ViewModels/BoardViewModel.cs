using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	public class BoardViewModel : ViewModelBase
	{
		private Board board;

		public BoardViewModel()
			: this(CreateTestBoard())
		{

		}

		public BoardViewModel(Board board)
		{
			this.board = board;
			this.Tiles = new ObservableCollection<TileViewModel>();
			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					this.Tiles.Add(new TileViewModel(board, x, y));
				}
			}
		}

		public int Width => this.board.Width;

		public int Height => this.board.Height;

		public ObservableCollection<TileViewModel> Tiles { get; }

		private static Board CreateTestBoard()
		{
			var board = Model.Board.Parse(
				"""
				1234
				3412
				    
				    
				""");
			new LogicSolver([.. ConstraintFactory.DefaultSudoku(2, 2)])
				.Solve(board);
			return board;
		}
	}
}
