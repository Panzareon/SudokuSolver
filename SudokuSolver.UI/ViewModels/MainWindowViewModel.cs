using System.Collections.ObjectModel;

namespace SudokuSolver.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			this.Board = new BoardViewModel(Model.Board.Parse(
				"""
				1234
				3412
				2341
				4123
				"""));
		}
		public BoardViewModel Board { get; set; }
		public ObservableCollection<TileViewModel> Tiles { get; } = new();
		public string Rows => "* * * *";
	}
}
