using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using SudokuSolver.UI.ViewModels;
using System;

namespace SudokuSolver.UI.Views
{
	public partial class TileView : UserControl
	{
		public TileView()
		{
			InitializeComponent();
			this.PointerPressed += (s, e) => this.FocusBorder.Focus();
			this.FocusBorder.GotFocus += (s, e) => { };
		}

		private TileViewModel ViewModel => this.DataContext as TileViewModel ?? throw new InvalidOperationException("The data context should be a TileViewModel");

		private void HandleTextInput(object sender, TextInputEventArgs args)
		{
			if (args.Text?.Length != 1)
			{
				return;
			}

			var charValue = args.Text[0];
			if (int.TryParse(charValue.ToString(), out var pressedNumber))
			{
				this.ViewModel.SetTile(pressedNumber);
			}
		}
	}
}