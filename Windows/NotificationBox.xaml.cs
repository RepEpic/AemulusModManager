using System;
using System.Windows;
using System.Windows.Shell;
using AemulusModManager.Utilities;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for NotificationBox.xaml
    /// </summary>
    public partial class NotificationBox : Window
    {
        public bool YesNo = false;
        public NotificationBox(string message, bool OK = true)
        {
            InitializeComponent();
            Notification.Text = message;
            if (OK)
            {
                OkButton.Visibility = Visibility.Visible;
                Platform.PlayNotificationSound();
                taskBarItem.ProgressState = TaskbarItemProgressState.Indeterminate;
            }
            else
            {
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
                Platform.PlayNotificationSound();
            }
            if (message.Length > 40)
                Notification.TextAlignment = TextAlignment.Left;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            YesNo = true;
            Close();
        }

    }

}
