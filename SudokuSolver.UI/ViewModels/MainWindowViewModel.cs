using SudokuSolver.Constraints;
using SudokuSolver.Solver;
using System.Collections.ObjectModel;

namespace SudokuSolver.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			var board = Model.Board.Parse(
				"""
				1234
				3412
				    
				    
				""");
			new LogicSolver([.. ConstraintFactory.DefaultSudoku(2, 2)])
				.Solve(board);
			this.Board = new BoardViewModel(board);
		}
		public BoardViewModel Board { get; set; }
		public ObservableCollection<TileViewModel> Tiles { get; } = new();
		public string Rows => "* * * *";
	}
}
