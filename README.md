Solver for variant sudoku puzzles, which is mostly based on a brute force approach doing a depth first search through the possibilities (BacktraceSolver),
combined with Sudoku solve techniques (LogicSolver) to reduce the amount of possibilities to check.

The main idea is to make it easy to extend the additional constraints on the Sudoku digits in the final solution.
