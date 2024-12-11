using System.Windows;
using ExamGame2D.Windows.GameWindows;
using ExamGame2D.Windows.SettingsWindows;

namespace ExamGame2D.Windows.StartWindow
{
    public partial class StartWindow : Window 
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
            
            this.Hide();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            
            this.Hide(); 
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}