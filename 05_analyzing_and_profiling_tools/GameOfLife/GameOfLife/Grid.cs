using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid : IDisposable
    {
        private readonly int SizeX;
        private readonly int SizeY;
        private readonly Cell[,] cells;
        private readonly Cell[,] nextGenerationCells;
        private static readonly Random rnd = new Random();
        private readonly Canvas drawCanvas;
        private readonly Ellipse[,] cellsVisuals;

        private bool disposed = false;

        public Grid(Canvas c)
        {
            drawCanvas = c;
            SizeX = (int) (c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            nextGenerationCells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            InitCells();
            SetRandomPattern();
            InitCellsVisuals();
            UpdateGraphics();            
        }


        public void Clear()
        {
            IterateCells((row, column) => ClearCell(row, column));
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;
            
            int row = (int)cellVisual.Margin.Left / 5;
            int column = (int)cellVisual.Margin.Top / 5;

            if (e.LeftButton == MouseButtonState.Pressed && !cells[row, column].IsAlive)
            {
                cells[row, column].IsAlive = true;
                cells[row, column].Age = 0;
                cellVisual.Fill = Brushes.White;
            }
        }

        public void UpdateGraphics()
        {
            IterateCells((row, column) =>
            {
                var newFill = cells[row, column].IsAlive ? (cells[row, column].Age < 2 ? Brushes.White : Brushes.DarkGray) : Brushes.Gray;

                if (cellsVisuals[row, column].Fill != newFill)
                {
                    cellsVisuals[row, column].Fill = newFill;
                }
            });
        }

        public void InitCellsVisuals()
        {
            IterateCells((row, column) =>
            {
                cellsVisuals[row, column] = new Ellipse();
                cellsVisuals[row, column].Width = cellsVisuals[row, column].Height = 5;

                double left = cells[row, column].PositionX;
                double top = cells[row, column].PositionY;

                cellsVisuals[row, column].Margin = new Thickness(left, top, 0, 0);
                cellsVisuals[row, column].Fill = Brushes.Gray;
                drawCanvas.Children.Add(cellsVisuals[row, column]);

                cellsVisuals[row, column].MouseMove += MouseMove;
                cellsVisuals[row, column].MouseLeftButtonDown += MouseMove;
            });

            UpdateGraphics();                    
        }
        

        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            IterateCells((row, column) => cells[row, column].IsAlive = GetRandomBoolean());
        }
        
        public void UpdateToNextGeneration()
        {
            IterateCells((row, column) =>
            {
                cells[row, column].IsAlive = nextGenerationCells[row, column].IsAlive;
                cells[row, column].Age = nextGenerationCells[row, column].Age;
            });

            UpdateGraphics();
        }
        

        public void Update()
        {
            bool alive = false;
            int age = 0;

            IterateCells((row, column) =>
            {
                CalculateNextGeneration(row, column, ref alive, ref age);
                nextGenerationCells[row, column].IsAlive = alive;
                nextGenerationCells[row, column].Age = age;
            });

            UpdateToNextGeneration();
        }

        public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)
        {
            isAlive = cells[row, column].IsAlive;
            age = cells[row, column].Age;

            int count = CountNeighbors(row, column);

            if (isAlive && count < 2)
            {
                isAlive = false;
                age = 0;
            }

            if (isAlive && (count == 2 || count == 3))
            {
                cells[row, column].Age++;
                isAlive = true;
                age = cells[row, column].Age;
            }

            if (isAlive && count > 3)
            {
                isAlive = false;
                age = 0;
            }

            if (!isAlive && count == 3)
            {
                isAlive = true;
                age = 0;
            }
        }

        public int CountNeighbors(int row, int column)
        {
            int count = 0;

            bool isNotLastRow = row != SizeX - 1;
            bool isNotLastColumn = column != SizeY - 1;
            bool isNotFirstRow = row != 0;
            bool isNotFirstColumn = column != 0;

            if (isNotLastRow && cells[row + 1, column].IsAlive) count++;
            if (isNotLastRow && isNotLastColumn && cells[row + 1, column + 1].IsAlive) count++;
            if (isNotLastColumn && cells[row, column + 1].IsAlive) count++;
            if (isNotFirstRow && isNotLastColumn && cells[row - 1, column + 1].IsAlive) count++;
            if (isNotFirstRow && cells[row - 1, column].IsAlive) count++;
            if (isNotFirstRow && isNotFirstColumn && cells[row - 1, column - 1].IsAlive) count++;
            if (isNotFirstColumn && cells[row, column - 1].IsAlive) count++;
            if (isNotLastRow && isNotFirstColumn && cells[row + 1, column - 1].IsAlive) count++;

            return count;
        }

        public void Dispose()
        {
            if (disposed) return;

            Unsubscribe();
            drawCanvas?.Children.Clear();

            disposed = true;
        }

        private void InitCells()
        {
            IterateCells((row, column) =>
            {
                cells[row, column] = new Cell(row, column, 0, false);
                nextGenerationCells[row, column] = new Cell(row, column, 0, false);
            });
        }

        private void ClearCell(int row, int column)
        {
            cells[row, column].IsAlive = false;
            cells[row, column].Age = 0;
            nextGenerationCells[row, column].IsAlive = false;
            nextGenerationCells[row, column].Age = 0;
            cellsVisuals[row, column].Fill = Brushes.Gray;
        }

        private void Unsubscribe()
        {
            IterateCells((row, column) =>
            {
                if (cellsVisuals[row, column] != null)
                {
                    cellsVisuals[row, column].MouseMove -= MouseMove;
                    cellsVisuals[row, column].MouseLeftButtonDown -= MouseMove;
                }
            });
        }

        private void IterateCells(Action<int, int> action)
        {
            for (int row = 0; row < SizeX; row++)
            {
                for (int column = 0; column < SizeY; column++)
                {
                    action(row, column);
                }
            }
        }
    }
}