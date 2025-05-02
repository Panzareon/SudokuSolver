using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SudokuSolver.UI.Views
{
	public class GridBehavior : AvaloniaObject
	{
		static GridBehavior()
		{
			RowCountProperty.Changed.AddClassHandler<Grid>(HandleRowCountChanged);
			ColumnCountProperty.Changed.AddClassHandler<Grid>(HandleColumnCountChanged);
		}


		/// <summary>
		/// Identifies the RowCount attached property.
		/// </summary>
		public static readonly AttachedProperty<int> RowCountProperty = AvaloniaProperty.RegisterAttached<Grid, Interactive, int>(
				"RowCount", default, false, BindingMode.OneTime);

		/// <summary>
		/// Identifies the ColumnCount attached property.
		/// </summary>
		public static readonly AttachedProperty<int> ColumnCountProperty = AvaloniaProperty.RegisterAttached<Grid, Interactive, int>(
				"ColumnCount", default, false, BindingMode.OneTime);
		
		public static int GetRowCount(Grid element)
		{
			return element.RowDefinitions.Count;
		}

		public static void SetRowCount(Grid element, object value)
		{
			element.SetValue(RowCountProperty, value);
		}
		public static int GetColumnCount(Grid element)
		{
			return element.ColumnDefinitions.Count;
		}

		public static void SetColumnCount(Grid element, object value)
		{
			element.SetValue(ColumnCountProperty, value);
		}
		private static void HandleRowCountChanged(Grid grid, AvaloniaPropertyChangedEventArgs args)
		{
			var rowCount = args.GetNewValue<int>();
			while (grid.RowDefinitions.Count > rowCount)
			{
				grid.RowDefinitions.RemoveAt(rowCount);
			}

			while (grid.RowDefinitions.Count < rowCount)
			{
				grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
			}
		}
		private static void HandleColumnCountChanged(Grid grid, AvaloniaPropertyChangedEventArgs args)
		{
			var columnCount = args.GetNewValue<int>();
			while (grid.ColumnDefinitions.Count > columnCount)
			{
				grid.ColumnDefinitions.RemoveAt(columnCount);
			}

			while (grid.ColumnDefinitions.Count < columnCount)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
			}
		}
	}
}
