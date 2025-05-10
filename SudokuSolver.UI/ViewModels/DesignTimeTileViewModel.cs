using SudokuSolver.Constraints;
using SudokuSolver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.UI.ViewModels
{
	internal class DesignTimeTileViewModel : TileViewModel
	{
		public DesignTimeTileViewModel() : base(Board.Parse("1"), 0, 0, [new ConstraintViewModel(new DefaultSudokuBox(3, 3), 0, 0)])
		{
		}
	}
}
