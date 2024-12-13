using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ExamGame2D.Windows.GameWindows
{
    public partial class GameWindow : Window
    {
        private double _playerSpeed = 3;
        private double _jumpHeight = 100;
        private double _jumpDuration = 0.3;
        private bool _isJumping = false;
        private bool _movingRight = false;
        private bool _movingLeft = false;

        public GameWindow()
        {
            InitializeComponent();
            this.KeyDown += GameWindow_PreviewKeyDown;
            this.KeyUp += GameWindow_PreviewKeyUp;
            CompositionTarget.Rendering += Update;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializePlayerPosition();
            this.Focus();
        }

        private void InitializePlayerPosition()
        {
            Canvas.SetLeft(Player, 0);
            Canvas.SetBottom(Player, 0);
        }

        private void GameWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if (!_isJumping)
                    {
                        Jump();
                    }
                    break;
                case Key.A:
                    _movingLeft = true; 
                    break;
                case Key.D:
                    _movingRight = true; 
                    break;
            }
        }

        private void GameWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    _movingLeft = false; 
                    break;
                case Key.D:
                    _movingRight = false; 
                    break;
            }
        }

        private void Update(object sender, System.EventArgs e)
        {
            double left = Canvas.GetLeft(Player);
            double playerBottom = Canvas.GetBottom(Player);

            // Проверка коллизий при любом движении
            CheckPlatformCollision();

            // Движение влево
            if (_movingLeft && left > 0)
            {
                Canvas.SetLeft(Player, left - _playerSpeed);
            }

            // Движение вправо
            if (_movingRight && left < GameCanvas.ActualWidth - Player.ActualWidth)
            {
                Canvas.SetLeft(Player, left + _playerSpeed);
            }

            // Проверка коллизий после перемещения
            CheckPlatformLanding();
        }

        private void Jump()
        {
            _isJumping = true;
            double bottom = Canvas.GetBottom(Player);

            // Анимация прыжка
            DoubleAnimation jumpUp = new DoubleAnimation
            {
                To = bottom + _jumpHeight,
                Duration = TimeSpan.FromSeconds(_jumpDuration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
                AutoReverse = true
            };

            jumpUp.Completed += (s, e) =>
            {
                _isJumping = false;
                CheckPlatformLanding();
            };

            Player.BeginAnimation(Canvas.BottomProperty, jumpUp);
        }

        private void CheckPlatformCollision()
        {
            double playerBottom = Canvas.GetBottom(Player);
            double playerLeft = Canvas.GetLeft(Player);
            double playerWidth = Player.ActualWidth;

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Rectangle platform)
                {
                    double platformTop = Canvas.GetBottom(platform) + platform.Height;
                    double platformLeft = Canvas.GetLeft(platform);
                    double platformWidth = platform.Width;

                    // Проверка коллизий
                    if (playerLeft + playerWidth > platformLeft &&
                        playerLeft < platformLeft + platformWidth &&
                        playerBottom <= platformTop && playerBottom >= platformTop - Player.ActualHeight)
                    {
                        // Блокировка движения на уровне платформ
                        if (_movingLeft)
                        {
                            Canvas.SetLeft(Player, platformLeft + platformWidth);
                        }
                        else if (_movingRight)
                        {
                            Canvas.SetLeft(Player, platformLeft - playerWidth);
                        }
                    }
                }
            }
        }

        private void CheckPlatformLanding()
        {
            double playerBottom = Canvas.GetBottom(Player);
            double playerLeft = Canvas.GetLeft(Player);
            double playerWidth = Player.ActualWidth;

            bool isOnPlatform = false;

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Rectangle platform)
                {
                    double platformBottom = Canvas.GetBottom(platform);
                    double platformLeft = Canvas.GetLeft(platform);
                    double platformWidth = platform.Width;

                    // Проверка приземления
                    if (playerBottom <= platformBottom &&
                        playerBottom + Player.ActualHeight >= platformBottom &&
                        playerLeft + playerWidth > platformLeft &&
                        playerLeft < platformLeft + platformWidth)
                    {
                        Canvas.SetBottom(Player, platformBottom + Player.ActualHeight);
                        isOnPlatform = true;
                        break;
                    }
                }
            }

            if (!isOnPlatform && playerBottom > 0)
            {
                Canvas.SetBottom(Player, 0); // Если не на платформе, возвращаем на землю
            }
        }
    }
}