using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Variable Declarations

        DispatcherTimer Timer;
        Random RandomShape;

        private const int SPEEDCONST = 700;
        private int RowCount = 0;
        private int ColumnCount = 0;
        private int LeftPosition = 0;
        private int DownPosition = 0;
        private int RotationDegree = 0;
        private int Score = 0;
        private int Level = 1;
        private int LevelIncrement = 60;

        private double LevelTimer = 0;

        private bool GameOver = false;
        private bool GameActive = false;
        private bool NextShapeDrawn = false;
        private bool Rotated = false;
        private bool BottomCollision = false;
        private bool LeftCollision = false;
        private bool RightCollision = false;

        private int[,] CurrentShape = null;

        private int CurrentShapeWidth;
        private int CurrentShapeHeight;
        private int CurrentShapeNumber;
        private int NextShapeNumber;
        private int TetrisGridColumn;
        private int TetrisGridRow;
        private int GameSpeed;

        List<int> CurrentRow = null;
        List<int> CurrentColumn = null;

        #endregion

        #region ColorDeclarations

        private static Color SquareShapeColor = Colors.Gold;
        private static Color IShapeColor = Colors.DarkTurquoise;
        private static Color SShapeColor = Colors.Lime;
        private static Color ZShapeColor = Colors.Crimson;
        private static Color JShapeColor = Colors.SlateBlue;
        private static Color LShapeColor = Colors.OrangeRed;
        private static Color TShapeColor = Colors.Orchid;

        Color[] ShapeColor = { SquareShapeColor, IShapeColor, SShapeColor, ZShapeColor, JShapeColor, LShapeColor, TShapeColor };

        #endregion

        #region Shape Declarations
        
        // Square Shape
        public int[,] SquareShape = new int[2, 2] { { 1, 1 },       // * *
                                                    { 1, 1 } };     // * *

        // I Shape
        public int[,] IShape0 = new int[2, 4] { { 1, 1, 1, 1 }, 
                                                { 0, 0, 0, 0 } };   // * * * *


        public int[,] IShape90 = new int[4, 2] { { 1, 0 },          // *
                                                { 1, 0 },           // *
                                                { 1, 0 },           // *
                                                { 1, 0 } };         // *
        // S Shape
        public int[,] SShape0 = new int[2, 3] { { 0,1,1 },          //   * *
                                                { 1,1,0 }};         // * *
        
        public int[,] SShape90 = new int[3, 2] { { 1,0 },           // *
                                                 { 1,1 },           // * *
                                                 { 0,1 }};          //   *

        // Z Shape
        public int[,] ZShape0 = new int[2, 3] { { 1,1,0 },          // * *
                                                { 0,1,1 }};         //   * *
        
        public int[,] ZShape90 = new int[3, 2] { { 0,1},            //    *
                                                 { 1,1 },           //  * *
                                                 { 1,0 }};          //  *

        // J Shape    
        public int[,] JShape0 = new int[2, 3] { { 1,0,0 },          // *
                                                {1,1,1 } };         // * * *

        public int[,] JShape90 = new int[3, 2] { { 1,1 },           // * *
                                                { 1,0 },            // * 
                                                { 1,0 }};           // *

        public int[,] JShape180 = new int[2, 3] { { 1,1,1 },        // * * *
                                                  { 0,0,1 }};       //     * 

        public int[,] JShape270 = new int[3, 2] { { 0,1},           //   *
                                                  {0,1 },           //   *
                                                  {1,1 }};          // * *

        // L Shape    
        public int[,] LShape0 = new int[2, 3] { { 0,0,1 },          //     *
                                                {1,1,1 } };         // * * *

        public int[,] LShape90 = new int[3, 2] { { 1,0 },           // * 
                                                { 1,0 },            // * 
                                                { 1,1 }};           // * *

        public int[,] LShape180 = new int[2, 3] { { 1,1,1 },        // * * *
                                                  { 1,0,0 }};       // *     

        public int[,] LShape270 = new int[3, 2] { { 1,1},           // * *
                                                  {0,1 },           //   *
                                                  {0,1 }};          //   *

        // T Shape    
        public int[,] TShape0 = new int[2, 3] { { 0,1,0 },          //   *  
                                                {1,1,1 } };         // * * *

        public int[,] TShape90 = new int[3, 2] { { 1,0 },           // * 
                                                { 1,1 },            // * *
                                                { 1,0 }};           // * 

        public int[,] TShape180 = new int[2, 3] { { 1,1,1 },        // * * *
                                                  { 0,1,0 }};       //   *     

        public int[,] TShape270 = new int[3, 2] { { 0,1},           //   *
                                                  {1,1 },           // * *
                                                  {0,1 }};          //   *


        string[] ShapeArray = { "", "SquareShape", "IShape0", "SShape0", "ZShape0", "JShape0", "LShape0", "TShape0" };


        #endregion

        public MainWindow()
        {
            InitializeComponent();
            
            GameSpeed = SPEEDCONST;
          
            // Create event for key press
            KeyDown += MainWindow_KeyDown;
          
            // Initialize Timer
            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;

            // Time interval will be set in miliseconds 
            Timer.Interval = new TimeSpan(0,0,0,0,GameSpeed);


            TetrisGridColumn = TetrisGrid.ColumnDefinitions.Count;
            TetrisGridRow = TetrisGrid.RowDefinitions.Count;

            // Creates Random Integers for shapes to be drawn
            RandomShape = new Random();

            // First shape and next shape are determined 
            CurrentShapeNumber = RandomShape.Next(1, 8);
            NextShapeNumber = RandomShape.Next(1, 8);

            // Removes "Next", "Level" and "Game Over" from UI visibility
            NextLabel.Visibility = LevelTextBlock.Visibility = GameOverTextBlock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Timer Tick even handler will run every timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Each interval increase the down position by 1
            DownPosition++;

            // Call MoveShape Function
            MoveShape();
            
            // Every 60 seconds the level will increase by 1 
            if (LevelTimer >= LevelIncrement)
            {
                if (GameSpeed >= 50)
                {
                    // By decreasing GameSpeed, the speed of the game will increase by decreasing interval between Timer Tick
                    GameSpeed -= 50;
                    
                    // Incement Level by 1
                    Level++;

                    // Update Level on the UI
                    LevelTextBlock.Text = "Level: " + Level.ToString();
                }

                // If GameSpeed is less than 50 it will reset to 50. This is the fastest the game will go
                else
                {
                    GameSpeed = 50;
                }
                Timer.Stop();
                // Update the Timer Interval which increases the GameSpeed
                Timer.Interval = new TimeSpan(0, 0, 0, 0, GameSpeed);
                Timer.Start();
                LevelTimer = 0;
            }
            // Keeps track of the seconds passed since last increased Level
            LevelTimer += (GameSpeed / 1000f);
        }

        
        /// <summary>
        /// Key Down Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!Timer.IsEnabled)
            {
                return;
            }

            switch (e.Key.ToString())
            {
                case "Up":
                    RotationDegree += 90;
                    if (RotationDegree > 270) { RotationDegree = 0; }
                    ShapeRotation(RotationDegree);
                    break;
                case "Down":
                    DownPosition++;
                    break;
                case "Right":
                    // Check there is a colision first
                    Collision();
                    if(!RightCollision) { LeftPosition++; }
                    RightCollision = false;
                    break;
                case "Left":
                    // Check there is a colision first
                    Collision();
                    if (!LeftCollision) { LeftPosition--; }
                    LeftCollision = false;
                    break;
            }

            MoveShape();
        }

        // Add new shape in new location
        private void MoveShape()
        {
            LeftCollision = false;
            RightCollision = false;

            // Check for a collision
            Collision();
            if (LeftPosition > (TetrisGridColumn - CurrentShapeWidth))
            {
                LeftPosition = (TetrisGridColumn - CurrentShapeWidth);
            }
            else if (LeftPosition < 0) { LeftPosition = 0; }
        
            if (BottomCollision)
            {
                StopShape();
                return;
            }
            AddShape(CurrentShapeNumber, LeftPosition, DownPosition);
        
        }

        // Method called when shape has collided or arrived at the bottom
        private void StopShape()
        {
            Timer.Stop();

            // If the down position of the last shape is less than two, than the shape is too high on the game board for a new shape to drop, game over
            if (DownPosition <= 2)
            {
                GameOverFunc();
                return;
            }

            int idx = 0;
            while (idx < TetrisGrid.Children.Count)
            {
                UIElement Element = TetrisGrid.Children[idx];
                if (Element is Rectangle)
                {
                    Rectangle Square = (Rectangle)Element;
                    if (Square.Name.IndexOf("unfixed_") ==0)
                    {
                        // replace name of square to fixed
                        string NewName = Square.Name.Replace("unfixed_", "fixed_");
                        Square.Name = NewName;
                    }
                }
                idx++;
            }
            // Check if line is complete and descend down to other shapes
            CheckComplete();
            Reset();
            Timer.Start();
        }

        // Resets...
        private void Reset()
        {
            DownPosition = 0;
            LeftPosition = 3;
            Rotated = false;
            RotationDegree = 0;
            CurrentShapeNumber = NextShapeNumber;
            if (!GameOver) { AddShape(CurrentShapeNumber, LeftPosition); }
            NextShapeDrawn = false;
            RandomShape = new Random();
            NextShapeNumber = RandomShape.Next(1, 8);
            BottomCollision = false;
            LeftCollision = false;
            RightCollision = false;
        }

        // Checks if we have a solid line
        private void CheckComplete()
        {
            int GridRow = TetrisGrid.RowDefinitions.Count;
            int GridColumn = TetrisGrid.ColumnDefinitions.Count;
            int SquareCount = 0;

            for (int Row = GridRow; Row >= 0; Row--)
            {
                SquareCount = 0;
                for (int Column = GridColumn; Column >= 0; Column--)
                {
                    Rectangle Square;
                    Square = (Rectangle)TetrisGrid.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == Row && Grid.GetColumn(e) == Column);
                    if (Square != null)
                    {
                        if (Square.Name.IndexOf("fixed") == 0)
                        {
                            SquareCount++;
                        }
                    }

                }
                // If SquareCount == GridColumn this mean that the line is complete and must be deleted
                if( SquareCount == GridColumn)
                {
                    DeleteLine(Row);
                    ScoreTextBlock.Text = GetScore().ToString();
                    CheckComplete();
                }

            }
        }

        // Get current score
        private object GetScore()
        {
            Score += 50 * Level;
            return Score;
        }

        // Remove complete line when we have a full row
        private void DeleteLine(int _row)
        {
            for (int idx = 0; idx < TetrisGrid.ColumnDefinitions.Count; idx++)
            {
                Rectangle Square;
                try
                {
                    Square = (Rectangle)TetrisGrid.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == _row && Grid.GetColumn(e) == idx);
                    TetrisGrid.Children.Remove(Square);
                }
                catch { }
            }
            // Move rest of grid down
            foreach (UIElement Element in TetrisGrid.Children)
            {
                Rectangle Square = (Rectangle) Element;
                if (Square.Name.IndexOf("fixed") == 0  && Grid.GetRow(Square) <= _row)
                {
                    Grid.SetRow(Square, Grid.GetRow(Square) + 1 );
                }
            }
        }

        // Game over
        private void GameOverFunc()
        {
            GameOver = true;
            Reset();
            StartButton.Content = "Start Game";
            GameOverTextBlock.Visibility = Visibility.Visible;
            RowCount = 0;
            ColumnCount = 0;
            LeftPosition = 0;
            LevelTimer = 0;
            GameSpeed = SPEEDCONST;
            Level = 1;
            GameActive = false;
            Score = 0;
            NextShapeDrawn = false;
            CurrentShape = null;
            CurrentShapeNumber = RandomShape.Next(1, 8);
            NextShapeNumber = RandomShape.Next(1, 8);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, GameSpeed);
        }

        /// <summary>
        /// Check for collision with left, right or bottom of grid
        /// </summary>
        private void Collision()
        {
            BottomCollision = CollisionCheck(0, 1);
            LeftCollision = CollisionCheck(-1, 0);
            RightCollision = CollisionCheck(1, 0);
        }

        /// <summary>
        /// Rotates the shape and returns the proper orientation to the grid
        /// </summary>
        /// <param name="_rotation"></param>
        private void ShapeRotation(int _rotation)
        {
            // Check if there will be a collision if the shape is rotated
            if (RotationCollision(RotationDegree))
            {
                RotationDegree -= 90;
                return;
            }

            if (ShapeArray[CurrentShapeNumber].IndexOf("I") ==0)
            {
                if(_rotation > 90) { _rotation = RotationDegree = 0; }
                CurrentShape = GetVariableByString("IShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("T") == 0)
            {
                CurrentShape = GetVariableByString("TShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("S") == 0)
            {
                if (_rotation > 90) { _rotation = RotationDegree = 0; }
                CurrentShape = GetVariableByString("SShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("Z") == 0)
            {
                if (_rotation > 90) { _rotation = RotationDegree = 0; }
                CurrentShape = GetVariableByString("ZShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("J") == 0)
            {
                CurrentShape = GetVariableByString("JShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("L") == 0)
            {
                CurrentShape = GetVariableByString("LShape" + _rotation);
            }

            else if (ShapeArray[CurrentShapeNumber].IndexOf("Sq") == 0)
            {
                return;
            }

            Rotated = true;
            AddShape(CurrentShapeNumber, LeftPosition, DownPosition);

        }

        /// <summary>
        /// Generates the shape on the grid
        /// </summary>
        /// <param name="currentShapeNumber"></param>
        /// <param name="_leftPosition"></param>
        /// <param name="_downPosition"></param>
        private void AddShape(int _currentShapeNumber, int _leftPosition=0, int _downPosition=0)
        {
            // Removes the shape's previous position
            RemoveShape();

            CurrentRow = new List<int>();
            CurrentColumn = new List<int>();

            // This will be the individual grid squares that make up the shape
            Rectangle Square = null;
            
            // Checks if the shape has been roated, if not it retrieves current shape type
            if (!Rotated)
            {
                CurrentShape = null;
                CurrentShape = GetVariableByString(ShapeArray[_currentShapeNumber].ToString());
            }


            // Get the height and width of current shape
            int HeightDimension = CurrentShape.GetLength(0);
            int WidthDimension = CurrentShape.GetLength(1);

            CurrentShapeHeight = HeightDimension;
            CurrentShapeWidth = WidthDimension;

            // For I Shape Only, set the height or width to one square
            if (CurrentShape == IShape0 ) { CurrentShapeHeight = 1; }
            else if (CurrentShape == IShape90 ) { CurrentShapeWidth = 1; }

            // Loops through the shape's 2-dimensional array. 
            for (int Row = 0; Row < HeightDimension; Row++)
            {
                for (int Column = 0; Column < WidthDimension; Column++)
                {

                    int Unit = CurrentShape[Row, Column];
                    // Where the shape's 2-dimensional array has a '1' is where each individual grid square should be drawn on the grid
                    if (Unit == 1)
                    {
                        Square = CreateIndividualSquare(ShapeColor[_currentShapeNumber - 1]);
                        TetrisGrid.Children.Add(Square);
                        Square.Name = "unfixed_" + Grid.GetRow(Square) + "_" + Grid.GetColumn(Square);
                        if (_downPosition >= TetrisGrid.RowDefinitions.Count - CurrentShapeHeight)
                        {
                            _downPosition = TetrisGrid.RowDefinitions.Count - CurrentShapeHeight;
                        }

                        Grid.SetRow(Square, RowCount + _downPosition);
                        Grid.SetColumn(Square, ColumnCount + _leftPosition);
                        CurrentRow.Add(RowCount + _downPosition);
                        CurrentColumn.Add(ColumnCount + _leftPosition);
                    }

                    ColumnCount++;
                }
                // reset column count
                ColumnCount = 0;
                RowCount++;
            }

            // reset both column and row
            ColumnCount = 0;
            RowCount = 0;

            // Draw next shape
            if (!NextShapeDrawn)
            {
                DrawNextShape(NextShapeNumber);
            }


        }

        // Draw next shape and show on the screen
        private void DrawNextShape(int _nextShapeNumber)
        {
            NextCanvas.Children.Clear();
            int[,] NextShape = null;
            NextShape = GetVariableByString(ShapeArray[_nextShapeNumber]);

            // Get the height and width of current shape
            int HeightDimension = NextShape.GetLength(0);
            int WidthDimension = NextShape.GetLength(1);

            int idx = 0;
            int idy = 0;
            Rectangle Square;

            for (int Row = 0; Row < HeightDimension; Row++)
            {
                for (int Column  = 0; Column < WidthDimension; Column++)
                {
                    int Unit = NextShape[Row, Column];
                    if (Unit == 1)
                    {
                        Square = CreateIndividualSquare(ShapeColor[_nextShapeNumber - 1]);
                        NextCanvas.Children.Add(Square);
                        Canvas.SetLeft(Square, idx);
                        Canvas.SetTop(Square, idy);
                    }
                    idx += 25;
                }
                idx = 0;
                idy += 25;
            }
            NextShapeDrawn = true;
        }

        /// <summary>
        /// Creates the individual squares that make up the shape
        /// </summary>
        /// <param name="_color"></param>
        /// <returns></returns>
        private Rectangle CreateIndividualSquare(Color _color)
        {
            Rectangle Sq = new Rectangle();
            // Sets height and in px
            Sq.Width = 25;
            Sq.Height = 25;
            // Sets square outline
            Sq.StrokeThickness = 1;
            Sq.Stroke = Brushes.White;
            // Sets fill color
            Sq.Fill = SetGradientColor(_color);
            return Sq;
        }

        /// <summary>
        /// Generates the color gradient that will fill individual squares
        /// </summary>
        /// <param name="_color"></param>
        /// <returns></returns>
        private Brush SetGradientColor(Color _color)
        {
            LinearGradientBrush GradientColor = new LinearGradientBrush();
            GradientColor.StartPoint = new Point(0, 0);
            GradientColor.EndPoint = new Point(1, 1.5);
            GradientStop Black = new GradientStop();
            Black.Color = Colors.Black;
            Black.Offset = -1.5;
            GradientColor.GradientStops.Add(Black);
            GradientStop Other = new GradientStop();
            Other.Color = _color;
            GradientColor.GradientStops.Add(Other);
            return GradientColor;
        }

        // Remove shape from the grid
        private void RemoveShape()
        {
            int idx = 0;
            while (idx < TetrisGrid.Children.Count)
            {
                UIElement Element = TetrisGrid.Children[idx];
                if (Element is Rectangle)
                {
                    Rectangle Square = (Rectangle)Element;
                    if (Square.Name.IndexOf("unfixed_") == 0)
                    {
                        TetrisGrid.Children.Remove(Element);
                        idx = -1;
                    }
                }
                idx++;
            }
        }

        // Access variable by string name
        private int[,] GetVariableByString(string _v)
        {
            return (int[,])this.GetType().GetField(_v).GetValue(this);
        }

        // Check for a collision while rotating shape
        private bool RotationCollision(int _rotation)
        {
            if (CollisionCheck(0, CurrentShapeWidth - 1)) { return true; }          // Bottom Collision
            else if (CollisionCheck(0, - (CurrentShapeWidth - 1))) { return true; } // Top Collision
            else if (CollisionCheck(0, - 1)) { return true; }                       // Top Collision
            else if (CollisionCheck(-1, CurrentShapeWidth - 1)) { return true; }    // Left Collision
            else if (CollisionCheck(1, CurrentShapeWidth - 1)) { return true; }     // Right Collision
            return false;
        }

        // Method called in other collision checks to check for a collision
        private bool CollisionCheck(int _leftRightOffSet, int _bottomOffSet)
        {
            Rectangle UnfixedSquare;
            int SquareRow = 0;
            int SquareColumn = 0;

            for (int idx = 0; idx <= 3; idx++)
            {
                SquareRow = CurrentRow[idx];
                SquareColumn = CurrentColumn[idx];
                try
                {
                    UnfixedSquare = (Rectangle)TetrisGrid.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == SquareRow + _bottomOffSet && Grid.GetColumn(e) == SquareColumn + _leftRightOffSet);
                    if (UnfixedSquare != null)
                    {
                        if (UnfixedSquare.Name.IndexOf("fixed") == 0)
                        {
                            return true;
                        }
                    }
                }
                catch { }
            }
            if(DownPosition > (TetrisGridRow - CurrentShapeHeight)) { return true; }
            return false;
        }

        /// <summary>
        /// Start / stop button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // If game is over, reset the grid
            if (GameOver)
            {
                TetrisGrid.Children.Clear();
                NextCanvas.Children.Clear();
                GameOverTextBlock.Visibility = Visibility.Collapsed;
                GameOver = false;
            }

            // If the timer is paused or not running...
            if(!Timer.IsEnabled)
            {
                // If game is not paused, reset score, center pc on grid and add pc to the grid
                if (!GameActive)
                {
                    ScoreTextBlock.Text = "0";
                    LeftPosition = 3;

                    // Add first shape to the grid
                    AddShape(CurrentShapeNumber, LeftPosition);
                }

                // Turns on visibility for the Next Label and Level Text Bloack
                NextLabel.Visibility = LevelTextBlock.Visibility = Visibility.Visible;
                LevelTextBlock.Text = "Level: " + Level.ToString();

                // Starts initial timer
                Timer.Start();
                StartButton.Content = "Stop Game";
                GameActive = true;
            }

            // Pauses current game
            else
            {
                // pauses timer
                Timer.Stop();
                StartButton.Content = "Start Game";
            }
        }

    }
}
