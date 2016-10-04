using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Point> foodPoints = new List<Point>();
        private List<Point> snakePoints = new List<Point>();
        private enum MOVINGDIRECTION
        {
            UP = 8,
            DOWN = 2,
            LEFT = 4,
            RIGHT = 6
        };
        private Point currentPosition = new Point();
        private int direction = 0;
        private int previousDirection = 0;
        private int headSize = 8;
        private int length = 10;
        private int score = 0;
        private Random rand = new Random();
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(10000);
            timer.Start();
            Point startingPoint = new Point(rand.Next(50, 190), rand.Next(50, 190));

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            paintSnake(startingPoint);
            currentPosition = startingPoint;
            paintFood(0);
        }

        private void paintSnake(Point currentposition)
        {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Fill = Brushes.Black;
            newEllipse.Width = headSize;
            newEllipse.Height = headSize;

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = paintCanvas.Children.Count;
            paintCanvas.Children.Add(newEllipse);
            
            snakePoints.Add(currentposition);
            
            if (count > length)  
            {
                paintCanvas.Children.RemoveAt(count - length );
                snakePoints.RemoveAt(count - length - 1);
            }
        }

        private void paintFood(int index)
        {
            Point foodPoint = new Point(rand.Next(15, 350), rand.Next(15, 190));

            Ellipse newEllipse = new Ellipse();

            newEllipse.Fill = Brushes.Red;
            newEllipse.Width = headSize;
            newEllipse.Height = headSize;

            Canvas.SetTop(newEllipse, foodPoint.Y);
            Canvas.SetLeft(newEllipse, foodPoint.X);
            paintCanvas.Children.Insert(index, newEllipse);
            foodPoints.Insert(index, foodPoint);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (direction)
            {
                case (int)MOVINGDIRECTION.DOWN:
                    currentPosition.Y += 1;
                    paintSnake(currentPosition);
                    break;
                case (int)MOVINGDIRECTION.UP:
                    currentPosition.Y -= 1;
                    paintSnake(currentPosition);
                    break;
                case (int)MOVINGDIRECTION.LEFT:
                    currentPosition.X -= 1;
                    paintSnake(currentPosition);
                    break;
                case (int)MOVINGDIRECTION.RIGHT:
                    currentPosition.X += 1;
                    paintSnake(currentPosition);
                    break;
            }

            if ((currentPosition.X < 5) || (currentPosition.X > 370) || (currentPosition.Y < 5) || (currentPosition.Y > 210))
            {
                GameOver();
            }

            int n = 0;
            foreach (Point point in foodPoints)
            {
                n = foodPoints.IndexOf(point);
                if ((Math.Abs(point.X - currentPosition.X) < (headSize - 1)) && (Math.Abs(point.Y - currentPosition.Y) < (headSize - 1)))
                {
                    length += headSize;
                    score += 10;
                    labelScore.Content = "SCORE: " + score.ToString();
                    foodPoints.RemoveAt(n);
                    paintCanvas.Children.RemoveAt(n);
                    paintFood(n);
                    break;
                }
            }

            for (int i = 0; i < (snakePoints.Count - headSize * 2); i++)
            {
                Point point = new Point(snakePoints[i].X, snakePoints[i].Y);
                if ((Math.Abs(point.X - currentPosition.X) < headSize) && (Math.Abs(point.Y - currentPosition.Y) < headSize))
                {
                    GameOver();
                    break;
                }
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (previousDirection != (int)MOVINGDIRECTION.UP)
                        direction = (int)MOVINGDIRECTION.DOWN;
                    break;
                case Key.Up:
                    if (previousDirection != (int)MOVINGDIRECTION.DOWN)
                        direction = (int)MOVINGDIRECTION.UP;
                    break;
                case Key.Left:
                    if (previousDirection != (int)MOVINGDIRECTION.RIGHT)
                        direction = (int)MOVINGDIRECTION.LEFT;
                    break;
                case Key.Right:
                    if (previousDirection != (int)MOVINGDIRECTION.LEFT)
                        direction = (int)MOVINGDIRECTION.RIGHT;
                    break;
                case Key.Q:
                    timer.Stop();
                    if (MessageBox.Show("Are you sure?", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                    else
                    {
                        timer.Start();
                    }
                    break;
            }
            previousDirection = direction;
        }

        private void GameOver()
        {
            MessageBox.Show("You Lose! Your score is " + score.ToString(), "Game Over", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            MessageBox.Show("Press OK to continue.", "Pause", MessageBoxButton.OK);
            timer.Start();
        }
    }
}
