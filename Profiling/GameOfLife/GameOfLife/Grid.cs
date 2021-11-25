using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
	class Grid
	{

		private int SizeX;
		private int SizeY;
		private Cell[,] cells;
		private Cell[,] nextGenerationCells;
		private static Random rnd;
		private Canvas drawCanvas;
		private Ellipse[,] cellsVisuals;


		public Grid(Canvas c)
		{
			this.drawCanvas = c;
			rnd = new Random();
			this.SizeX = (int)(c.Width / 5);
			this.SizeY = (int)(c.Height / 5);
			this.cells = new Cell[this.SizeX, this.SizeY];
			this.nextGenerationCells = new Cell[this.SizeX, this.SizeY];
			this.cellsVisuals = new Ellipse[this.SizeX, this.SizeY];

			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
				{
					this.cells[i, j] = new Cell(i, j, 0, false);
					this.nextGenerationCells[i, j] = new Cell(i, j, 0, false);
				}

			this.SetRandomPattern();
			this.InitCellsVisuals();
			this.UpdateGraphics();

		}


		public void Clear()
		{
			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
				{
					this.cells[i, j] = new Cell(i, j, 0, false);
					this.nextGenerationCells[i, j] = new Cell(i, j, 0, false);
					this.cellsVisuals[i, j].Fill = Brushes.Gray;
				}
		}


		void MouseMove(object sender, MouseEventArgs e)
		{
			var cellVisual = sender as Ellipse;

			int i = (int)cellVisual.Margin.Left / 5;
			int j = (int)cellVisual.Margin.Top / 5;


			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!this.cells[i, j].IsAlive)
				{
					this.cells[i, j].IsAlive = true;
					this.cells[i, j].Age = 0;
					cellVisual.Fill = Brushes.White;
				}
			}
		}

		public void UpdateGraphics()
		{
			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
					this.cellsVisuals[i, j].Fill = this.cells[i, j].IsAlive
												  ? (this.cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
												  : Brushes.Gray;
		}

		public void InitCellsVisuals()
		{
			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
				{
					this.cellsVisuals[i, j] = new Ellipse();
					this.cellsVisuals[i, j].Width = this.cellsVisuals[i, j].Height = 5;
					double left = this.cells[i, j].PositionX;
					double top = this.cells[i, j].PositionY;
					this.cellsVisuals[i, j].Margin = new Thickness(left, top, 0, 0);
					this.cellsVisuals[i, j].Fill = Brushes.Gray;
					this.drawCanvas.Children.Add(this.cellsVisuals[i, j]);

					this.cellsVisuals[i, j].MouseMove += this.MouseMove;
					this.cellsVisuals[i, j].MouseLeftButtonDown += this.MouseMove;
				}
			this.UpdateGraphics();

		}


		public static bool GetRandomBoolean()
		{
			return rnd.NextDouble() > 0.8;
		}

		public void SetRandomPattern()
		{
			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
					this.cells[i, j].IsAlive = GetRandomBoolean();
		}

		public void UpdateToNextGeneration()
		{
			for (int i = 0; i < this.SizeX; i++)
				for (int j = 0; j < this.SizeY; j++)
				{
					this.cells[i, j].IsAlive = this.nextGenerationCells[i, j].IsAlive;
					this.cells[i, j].Age = this.nextGenerationCells[i, j].Age;
				}

			this.UpdateGraphics();
		}


		public void Update()
		{
			bool alive = false;
			int age = 0;

			for (int i = 0; i < this.SizeX; i++)
			{
				for (int j = 0; j < this.SizeY; j++)
				{
					// nextGenerationCells[i, j] = CalculateNextGeneration(i,j);          // UNOPTIMIZED
					this.CalculateNextGeneration(i, j, ref alive, ref age);   // OPTIMIZED
					this.nextGenerationCells[i, j].IsAlive = alive;  // OPTIMIZED
					this.nextGenerationCells[i, j].Age = age;  // OPTIMIZED
				}
			}
			this.UpdateToNextGeneration();
		}

		public Cell CalculateNextGeneration(int row, int column)    // UNOPTIMIZED
		{
			bool alive;
			int count, age;

			alive = this.cells[row, column].IsAlive;
			age = this.cells[row, column].Age;
			count = this.CountNeighbors(row, column);

			if (alive && count < 2)
				return new Cell(row, column, 0, false);

			if (alive && (count == 2 || count == 3))
			{
				this.cells[row, column].Age++;
				return new Cell(row, column, this.cells[row, column].Age, true);
			}

			if (alive && count > 3)
				return new Cell(row, column, 0, false);

			if (!alive && count == 3)
				return new Cell(row, column, 0, true);

			return new Cell(row, column, 0, false);
		}

		public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)     // OPTIMIZED
		{
			isAlive = this.cells[row, column].IsAlive;
			age = this.cells[row, column].Age;

			int count = this.CountNeighbors(row, column);

			if (isAlive && count < 2)
			{
				isAlive = false;
				age = 0;
			}

			if (isAlive && (count == 2 || count == 3))
			{
				this.cells[row, column].Age++;
				isAlive = true;
				age = this.cells[row, column].Age;
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

		public int CountNeighbors(int i, int j)
		{
			int count = 0;

			if (i != this.SizeX - 1 && this.cells[i + 1, j].IsAlive) count++;
			if (i != this.SizeX - 1 && j != this.SizeY - 1 && this.cells[i + 1, j + 1].IsAlive) count++;
			if (j != this.SizeY - 1 && this.cells[i, j + 1].IsAlive) count++;
			if (i != 0 && j != this.SizeY - 1 && this.cells[i - 1, j + 1].IsAlive) count++;
			if (i != 0 && this.cells[i - 1, j].IsAlive) count++;
			if (i != 0 && j != 0 && this.cells[i - 1, j - 1].IsAlive) count++;
			if (j != 0 && this.cells[i, j - 1].IsAlive) count++;
			if (i != this.SizeX - 1 && j != 0 && this.cells[i + 1, j - 1].IsAlive) count++;

			return count;
		}
	}
}