using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreakOutGame
{
    public partial class MainForm : Form
    {
        // Declare game components
        private Timer gameTimer;
        private const int BrickRows = 5;
        private const int BrickColumns = 10;
        private const int BrickWidth = 75;
        private const int BrickHeight = 20;
        private const int PaddleWidth = 100;
        private const int PaddleHeight = 10;
        private const int BallSize = 10;
        private Rectangle[,] bricks;
        private Color[,] brickColors;
        private Rectangle paddle;
        private Rectangle ball;
        private int ballSpeedX = 4;
        private int ballSpeedY = 4;
        private bool[,] brickVisible;
        private int score;
        private Label scoreLabel;
        private Button resetButton;

        public MainForm()
        {
            InitializeComponent(); // Use the designer-generated InitializeComponent method
            InitGame(); // Initialize the game-specific components
        }

        private void InitGame()
        {
            this.DoubleBuffered = true;

            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;

            paddle = new Rectangle((ClientSize.Width - PaddleWidth) / 2, ClientSize.Height - PaddleHeight - 10, PaddleWidth, PaddleHeight);
            ball = new Rectangle((ClientSize.Width - BallSize) / 2, ClientSize.Height / 2, BallSize, BallSize);

            bricks = new Rectangle[BrickRows, BrickColumns];
            brickColors = new Color[BrickRows, BrickColumns];
            brickVisible = new bool[BrickRows, BrickColumns];
            Random rnd = new Random();

            for (int i = 0; i < BrickRows; i++)
            {
                for (int j = 0; j < BrickColumns; j++)
                {
                    bricks[i, j] = new Rectangle(j * BrickWidth, i * BrickHeight, BrickWidth, BrickHeight);
                    brickVisible[i, j] = true;
                    brickColors[i, j] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
            }

            score = 0;

            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Location = new Point(10, 10);
            scoreLabel.AutoSize = true;
            this.Controls.Add(scoreLabel);

            resetButton = new Button();
            resetButton.Text = "Reset";
            resetButton.Location = new Point(ClientSize.Width - 80, 10);
            resetButton.Click += ResetButton_Click;
            this.Controls.Add(resetButton);

            gameTimer.Start();
            this.Paint += MainForm_Paint;
            this.KeyDown += MainForm_KeyDown;
            this.KeyPreview = true; // Ensure the form captures key events
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < BrickRows; i++)
            {
                for (int j = 0; j < BrickColumns; j++)
                {
                    if (brickVisible[i, j])
                    {
                        using (Brush brickBrush = new SolidBrush(brickColors[i, j]))
                        {
                            g.FillRectangle(brickBrush, bricks[i, j]);
                        }
                    }
                }
            }

            g.FillRectangle(Brushes.Blue, paddle);
            g.FillRectangle(Brushes.DarkGray, ball); // Change ball color to dark gray
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            ball.X += ballSpeedX;
            ball.Y += ballSpeedY;

            if (ball.Left <= 0 || ball.Right >= ClientSize.Width)
            {
                ballSpeedX = -ballSpeedX;
            }

            if (ball.Top <= 0)
            {
                ballSpeedY = -ballSpeedY;
            }

            if (ball.IntersectsWith(paddle))
            {
                ballSpeedY = -ballSpeedY;
            }

            for (int i = 0; i < BrickRows; i++)
            {
                for (int j = 0; j < BrickColumns; j++)
                {
                    if (brickVisible[i, j] && ball.IntersectsWith(bricks[i, j]))
                    {
                        brickVisible[i, j] = false;
                        ballSpeedY = -ballSpeedY;
                        score += 10;
                        scoreLabel.Text = "Score: " + score;
                        break;
                    }
                }
            }

            if (ball.Bottom >= ClientSize.Height)
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over");
            }

            Invalidate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            int paddleSpeed = 20;

            if (e.KeyCode == Keys.Left && paddle.Left > 0)
            {
                paddle.X -= paddleSpeed;
            }
            else if (e.KeyCode == Keys.Right && paddle.Right < ClientSize.Width)
            {
                paddle.X += paddleSpeed;
            }

            Invalidate(); // Force the form to repaint and update the paddle position
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            gameTimer.Stop();
            InitGame();
        }
    }
}
