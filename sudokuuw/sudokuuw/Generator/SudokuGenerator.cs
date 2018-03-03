using System.Collections.Generic;
using System.Linq;
using System;

namespace Sudokuuw.Generator
{
    public enum Level
    {
        VeryEasy,
        Easy,
        Medium,
        Hard,
        VeryHard
    }

    public struct SudoCell {
        public byte value;
        public Boolean isReadOnly;
        public HashSet<byte> testValues;
    }

    public class Sudoku
    {
        public int BlockSize { get; private set; }
        public int BoardSize { get; private set; }
        private readonly byte[] possibleValues;
        private readonly IDictionary<int, int> blockIndex = new Dictionary<int, int>();
        private readonly IDictionary<int, int> inBlockIndex = new Dictionary<int, int>();
        private byte[][] rows;
        private byte[][] columns;
        private byte[][] blocks;
        private Boolean[][] readOnlyCells;
        private HashSet<byte>[][] testValues;
        private HashSet<byte>[] rowValues;
        private HashSet<byte>[] columnValues;
        private HashSet<byte>[] blockValues;
        


        public Sudoku(int blockSize)
        {
            this.BlockSize = blockSize;
            this.BoardSize = blockSize * blockSize;
            this.possibleValues = Enumerable.Range(1, this.BoardSize).Select(value => (byte)value).ToArray();

            this.rows = new byte[this.BoardSize][];
            this.columns = new byte[this.BoardSize][];
            this.blocks = new byte[this.BoardSize][];
            this.readOnlyCells = new Boolean[this.BoardSize][];
            this.testValues = new HashSet<byte>[this.BoardSize][];
            this.rowValues = new HashSet<byte>[this.BoardSize];
            this.columnValues = new HashSet<byte>[this.BoardSize];
            this.blockValues = new HashSet<byte>[this.BoardSize];
            
            for (var x = 0; x < this.BoardSize; x++)
            {
                this.rows[x] = new byte[this.BoardSize];
                this.columns[x] = new byte[this.BoardSize];
                this.blocks[x] = new byte[this.BoardSize];
                this.readOnlyCells[x] = new Boolean[this.BoardSize];
                this.testValues[x] = new HashSet<byte>[this.BoardSize];
                for(var y=0; y<this.BoardSize; y++)
                {
                    this.testValues[x][y] = new HashSet<byte>();
                }
                this.rowValues[x] = new HashSet<byte>();
                this.columnValues[x] = new HashSet<byte>();
                this.blockValues[x] = new HashSet<byte>();
            }

            for (var blockX = 0; blockX < this.BlockSize; blockX++)
            {
                for (var blockY = 0; blockY < this.BlockSize; blockY++)
                {
                    for (var x = 0; x < this.BlockSize; x++)
                    {
                        for (var y = 0; y < this.BlockSize; y++)
                        {
                            var itemX = blockX * this.BlockSize + x;
                            var itemY = blockY * this.BlockSize + y;
                            this.blockIndex[itemY * this.BoardSize + itemX] = blockY * this.BlockSize + blockX;
                            this.inBlockIndex[itemY * this.BoardSize + itemX] = y * this.BlockSize + x;
                        }
                    }
                }
            }
        }

        private Sudoku(Sudoku sudoku)
        {
            this.BlockSize = sudoku.BlockSize;
            this.BoardSize = sudoku.BoardSize;
            this.possibleValues = sudoku.possibleValues;
            this.blockIndex = sudoku.blockIndex;
            this.inBlockIndex = sudoku.inBlockIndex;
            
            this.rows = sudoku.rows.Select(row => row.ToArray()).ToArray();
            this.columns = sudoku.columns.Select(column => column.ToArray()).ToArray();
            this.blocks = sudoku.blocks.Select(block => block.ToArray()).ToArray();
            this.readOnlyCells = sudoku.readOnlyCells.Select(readOnlyCell => readOnlyCell.ToArray()).ToArray();

            this.testValues = sudoku.testValues.Select(testValue => testValue.ToArray()).ToArray();

            this.rowValues = sudoku.rowValues.Select(values => new HashSet<byte>(values)).ToArray();
            this.columnValues = sudoku.rowValues.Select(values => new HashSet<byte>(values)).ToArray();
            this.blockValues = sudoku.rowValues.Select(values => new HashSet<byte>(values)).ToArray();
        }

        public SudoCell GetSudoCell(int x, int y)
        {
            return new SudoCell() {
                value = this.rows[x][y],
                isReadOnly = this.readOnlyCells[x][y],
                testValues = this.testValues[x][y]
            };
        }

        public byte GetValue(int x, int y)
        {
            return this.rows[x][y];
        }

        public void SetValue(int x, int y, byte value, Boolean isInital = false)
        {            
            var oldValue = GetValue(x, y);
            this.rows[x][y] = value;
            this.columns[y][x] = value;
            var blockIndex = this.blockIndex[y * this.BoardSize + x];
            this.blocks[blockIndex][this.inBlockIndex[y * this.BoardSize + x]] = value;
            this.rowValues[x].Remove(oldValue);
            this.rowValues[x].Add(value);
            this.columnValues[y].Remove(oldValue);
            this.columnValues[y].Add(value);
            this.blockValues[blockIndex].Remove(oldValue);
            this.blockValues[blockIndex].Add(value);
            
            if (isInital)
            {
                if (value == 0)
                {
                    this.readOnlyCells[x][y] = false;
                }
                else
                {
                    this.readOnlyCells[x][y] = true;
                }
            }
            
            
            
        }

        public bool CanSetValue(int x, int y, byte value)
        {
            return !this.rowValues[x].Contains(value) && !this.columnValues[y].Contains(value) && !this.blockValues[this.blockIndex[y * this.BoardSize + x]].Contains(value);
        }


        public void SetTestValue(int x, int y, byte value)
        {
            if (this.testValues[x][y].Contains(value)) {
                this.testValues[x][y].Remove(value);
            } else {
                this.testValues[x][y].Add(value);
            }
        }

        public IList<byte> GetPossibleValues(int x, int y)
        {
            return this.possibleValues.Where(value => CanSetValue(x, y, value)).ToList();
        }

        public Sudoku Clone()
        {
            return new Sudoku(this);
        }
    }


    public class SudoGenerator
    {
        private readonly Random random = new Random();

        private const int MaxTries = 10000;
        private const int RandomCells = 15;
        private static readonly IDictionary<Level, Tuple<int, int>> LevelToClearCells = new Dictionary<Level, Tuple<int, int>>()
        {
            { Level.VeryEasy, new Tuple<int, int>(20, 30) },
            { Level.Easy, new Tuple<int, int>(31, 45) },
            { Level.Medium, new Tuple<int, int>(46, 49) },
            { Level.Hard, new Tuple<int, int>(50, 33) },
            { Level.VeryHard, new Tuple<int, int>(54, 81) },
        };

        public Sudoku Generate(int blockSize, Level level)
        {
            for (var count = 0; count < MaxTries; count++)
            {
                var sudoku = new Sudoku(blockSize);
                if (GenerateSolution(sudoku))
                {
                    if (CreateBoard(sudoku, level))
                    {
                        return sudoku;
                    }
                }
            }
            return null;
        }

        private bool GenerateSolution(Sudoku sudoku)
        {
            return GenerateRandomValues(sudoku) && SolveSolution(sudoku);
        }

        private bool GenerateRandomValues(Sudoku sudoku)
        {
            var indexes = Enumerable.Range(0, sudoku.BoardSize * sudoku.BoardSize).ToList();
            for (int i = 0; i < RandomCells; i++)
            {
                var index = indexes[this.random.Next(indexes.Count)];
                indexes.Remove(index);
                var x = index / sudoku.BoardSize;
                var y = index % sudoku.BoardSize;
                var possibleValues = sudoku.GetPossibleValues(x, y);
                if (possibleValues.Count == 0)
                {
                    return false;
                }
                sudoku.SetValue(x, y, possibleValues[random.Next(possibleValues.Count)], true);
            }
            return true;
        }

        public bool SolveSolution(Sudoku sudoku)
        {
            var range = Enumerable.Range(0, sudoku.BoardSize);
            var solutions =
                range
                .SelectMany(x => range.Select(y => new Tuple<int, int>(x, y)))
                .Where(item => sudoku.GetValue(item.Item1, item.Item2) == 0)
                .OrderBy(item => sudoku.GetPossibleValues(item.Item1, item.Item2).Count)
                .ToList();
            var index = 0;
            var possibleValuesCache = new Dictionary<int, IList<byte>>();
            while (index >= 0 && index < solutions.Count)
            {
                var x = solutions[index].Item1;
                var y = solutions[index].Item2;
                if (!possibleValuesCache.ContainsKey(index))
                {
                    possibleValuesCache[index] = sudoku.GetPossibleValues(x, y);
                }
                var possibleValues = possibleValuesCache[index];
                if (possibleValues.Count == 0)
                {
                    sudoku.SetValue(x, y, 0, true);
                    possibleValuesCache.Remove(index);
                    index--;
                }
                else
                {
                    var value = possibleValues[random.Next(0, possibleValues.Count)];
                    possibleValues.Remove(value);
                    sudoku.SetValue(x, y, value, true);
                    index++;
                }
            }
            return index >= solutions.Count;
        }

        /// <summary>
        /// Empties cells from the solved sudoku board to create playable game board. 
        /// The result has a number of empty cells dependant on the level and only one correct solution.
        /// </summary>
        private bool CreateBoard(Sudoku sudoku, Level level)
        {
            // All indexes
            var cells = Enumerable.Range(0, sudoku.BoardSize).SelectMany(x => Enumerable.Range(0, sudoku.BoardSize).Select(y => new Tuple<int, int>(x, y))).ToList();
            // List of already veryfied, cleared cell indexes
            var clearedCells = new List<Tuple<int, int>>();
            // Number/index of the cell under try 
            var cellNumber = 0;
            // How many cleared cells are required for the given level
            var clearedCellsRange = LevelToClearCells[level];

            // As long as there are more cells to try and the number of cleared cells is not higher then required maximum.
            while (cells.Count > 0 && clearedCells.Count < clearedCellsRange.Item2)
            {
                // Cell index to try
                var cell = GetNextCell(cells, level, cellNumber++);
                var currentValue = sudoku.GetValue(cell.Item1, cell.Item2);
                // Make sure that any value different from the current one disables the solution.
                // Otherwise more than 1 solution would be possible after clearing this cell.
                var valuesToCheck = sudoku.GetPossibleValues(cell.Item1, cell.Item2).ToList();
                valuesToCheck.Remove(currentValue);
                var correct = true;
                foreach (var valueToCheck in valuesToCheck)
                {
                    sudoku.SetValue(cell.Item1, cell.Item2, valueToCheck);
                    if (SolveSolution(sudoku))
                    {
                        // Other solution possible - this cell cannot be cleared.
                        correct = false;
                        break;
                    }
                }
                // If no other solution is possible we can clear this cell.
                if (correct)
                {
                    clearedCells.Add(new Tuple<int, int>(cell.Item1, cell.Item2));
                }
                // Make sure the cleared cells are really clear.
                foreach (var clearedCell in clearedCells)
                {
                    sudoku.SetValue(clearedCell.Item1, clearedCell.Item2, 0, true);
                }

                if (!correct)
                {
                    // Other solution possible - this cell cannot be cleared.
                    // Set it to its original value.
                    sudoku.SetValue(cell.Item1, cell.Item2, currentValue, true);
                }
            }
            return clearedCells.Count >= clearedCellsRange.Item1;
        }

        /// <summary>
        /// Gets the next cell to try to clear it.
        /// For the easiest levels it is a random cell.
        /// For medium the method returns every other cell.
        /// For hardest levels it returns the first not tried cell.
        /// </summary>
        private Tuple<int, int> GetNextCell(IList<Tuple<int, int>> cells, Level level, int cellNumber)
        {
            var index = 0;
            if (level == Level.VeryEasy || level == Level.Easy)
            {
                index = this.random.Next(cells.Count);
            }
            if (level == Level.Medium)
            {
                index = cellNumber % cells.Count;
            }
            var cell = cells[index];
            cells.RemoveAt(index);
            return cell;
        }
    }
}
