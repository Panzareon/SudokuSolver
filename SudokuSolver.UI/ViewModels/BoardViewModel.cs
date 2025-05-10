using ReactiveUI;
using SudokuSolver.Constraints;
using SudokuSolver.Model;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SudokuSolver.UI.ViewModels
{
	public class BoardViewModel : ViewModelBase
	{
		private readonly IConstraint[] constraints;

		private Board board;

		public BoardViewModel()
			: this(CreateTestBoard(), [..ConstraintFactory.DefaultSudoku(2, 2)])
		{
		}

		public BoardViewModel(Board board, IConstraint[] constraints)
		{
			this.board = board;
			this.constraints = constraints;
			this.Tiles = new ObservableCollection<TileViewModel>();
			this.SolveCommand = ReactiveCommand.Create(this.Solve);
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

		public ICommand SolveCommand { get; }

		public void Solve()
		{
			new LogicSolver(this.constraints).Solve(this.board);
			this.Refresh();
		}


		/// <summary>
		/// Refreshes all tiles after a bigger change in the board.
		/// </summary>
		private void Refresh()
		{
			foreach (var tile in this.Tiles)
			{
				tile.Refresh();
			}
		}

		private static Board CreateTestBoard()
		{
			var board = Model.Board.Parse(
				"""
				1234
				341 
				    
				    
				""");
			return board;
		}
	}
}
