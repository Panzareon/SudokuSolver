using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace SudokuSolver.UI.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			return;
			for (var i = 0; i < 4; i++)
			{
				this.InnerGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
			}
			for (var i = 0; i < 4; i++)
			{
				this.InnerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
			}
		}

		protected override void OnOpened(EventArgs e)
		{
			base.OnOpened(e);
			//new Control().ApplyTemplate()
			//this.Board.ItemsPanelRoot
		}
	}
}