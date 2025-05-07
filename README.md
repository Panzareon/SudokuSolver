Solver for variant sudoku puzzles, which is mostly based on a brute force approach doing a depth first search through the possibilities (BacktraceSolver),
combined with Sudoku solve techniques (LogicSolver) to reduce the amount of possibilities to check.

The main idea is to make it easy to extend the additional constraints on the Sudoku digits in the final solution.

There is the ChaosConstructionSolver for Sudoku puzzels where the location of the regions need to be determined, but this currently takes forever to run in bigger sudoku grids.

The UI component can currently only display the Sudoku defined in code and doesn't yet allow editing it.