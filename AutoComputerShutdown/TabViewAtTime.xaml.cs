using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace AutoComputerShutdown
{
    /// <summary>
    /// Interaction logic for TabView.xaml
    /// </summary>
    public partial class TabViewAtTime : UserControl
    {
        private DispatcherTimer liveTime;
        private int countDownSeconds;
        private DateTime endTime;
        private string filepath;
        private readonly TimeSpinner timeSpinner;
        private Label ActionLogger;
        public TabViewAtTime()
        {
            InitializeComponent();
            timeSpinner = new TimeSpinner(true, ItemsControl, DatePickerAt);
            DatePickerAt.SelectedDate = DateTime.Now;

            this.Loaded += delegate
            {
                EnableCountdownMode(((MainWindow)Application.Current.MainWindow).IsCountdownMode);
            };
        }
        private void StartAction(object sender, RoutedEventArgs e)
        {
            ActionLogger = ((MainWindow)Application.Current.MainWindow).LabelLogger;
            liveTime = MainWindow.LiveTime;

            TimeSpan timespanDifference = new TimeSpan(DatePickerAt.SelectedDate.GetValueOrDefault().DayOfYear - DateTime.Now.DayOfYear, timeSpinner.Hours - DateTime.Now.Hour, timeSpinner.Minutes - DateTime.Now.Minute, timeSpinner.Seconds - DateTime.Now.Second);

            if (timespanDifference.TotalSeconds < 0)
            {
                ActionLogger.Content = "Make sure to set a time that has not already happened.";
                return;
            }
            EnableCountdownMode(true);

            countDownSeconds = (int)timespanDifference.TotalSeconds;
            endTime = DateTime.Now.Add(timespanDifference);
            DetermineEventBasedOnCheckedRadioButton(false);
        }
        private void DisableAction(object sender, RoutedEventArgs e)
        {
            EnableCountdownMode(false);
            DetermineEventBasedOnCheckedRadioButton(true);
            ActionLogger.Content = "The action was interrupted, select new.";
        }

        private void EnableCountdownMode(bool enable)
        {
            if (enable)
            {
                ButtonCancel.IsEnabled = true;
                ButtonCancel.Foreground = new SolidColorBrush(Colors.Red);
                ButtonCancel.Opacity = 1;

                LogoutRadio.IsEnabled = false;
                LogoutRadio.Opacity = 0.2;

                ShutdownRadio.IsEnabled = false;
                ShutdownRadio.Opacity = 0.2;

                RestartRadio.IsEnabled = false;
                RestartRadio.Opacity = 0.2;

                RunRadio.IsEnabled = false;
                RunRadio.Opacity = 0.2;

                ButtonStart.IsEnabled = false;
                ButtonStart.Foreground = new SolidColorBrush(Colors.Black);
                ButtonStart.Opacity = 0.2;

               ((MainWindow)Application.Current.MainWindow).IsCountdownMode = true;
            }
            else
            {
                ButtonCancel.IsEnabled = false;
                ButtonCancel.Foreground = new SolidColorBrush(Colors.Black);
                ButtonCancel.Opacity = 0.2;

                LogoutRadio.IsEnabled = true;
                LogoutRadio.Opacity = 1;

                ShutdownRadio.IsEnabled = true;
                ShutdownRadio.Opacity = 1;

                RestartRadio.IsEnabled = true;
                RestartRadio.Opacity = 1;

                RunRadio.IsEnabled = true;
                RunRadio.Opacity = 1;

                ButtonStart.IsEnabled = true;
                ButtonStart.Foreground = new SolidColorBrush(Colors.Green);
                ButtonStart.Opacity = 1;

                ((MainWindow)Application.Current.MainWindow).IsCountdownMode = false;
            }
        }

        private void DetermineEventBasedOnCheckedRadioButton(bool disable)
        {
            if (ShutdownRadio.IsChecked.GetValueOrDefault())
            {
                if (disable)
                {
                    liveTime.Tick -= ShutdownAction;
                }
                else
                {
                    liveTime.Tick += ShutdownAction;
                }
            }
            else if (RestartRadio.IsChecked.GetValueOrDefault())
            {
                if (disable)
                {
                    liveTime.Tick -= RestartAction;
                }
                else
                {
                    liveTime.Tick += RestartAction;
                }
            }
            else if (LogoutRadio.IsChecked.GetValueOrDefault())
            {
                if (disable)
                {
                    liveTime.Tick -= LogoutAction;
                }
                else
                {
                    liveTime.Tick += LogoutAction;
                }
            }
            else if (RunRadio.IsChecked.GetValueOrDefault())
            {
                if (disable)
                {
                    liveTime.Tick -= RunAction;
                }
                else
                {
                    liveTime.Tick += RunAction;
                }
            }
        }
        private void RunAction(object sender, EventArgs e)
        {
            TimeSpan secondsTimespan = new TimeSpan(0, 0, --countDownSeconds);

            if (secondsTimespan.TotalSeconds == 120)
            {
                SendNotification("File will be run in 2 minutes!", $"The file {filepath.Substring(filepath.LastIndexOf("\\") + 1)} will be run in 2 minutes!");
            }

            if (secondsTimespan.TotalSeconds == 0)
            {
                ProcessStartInfo process = new ProcessStartInfo(filepath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = true
                };
                Process.Start(process);
                liveTime.Tick -= RunAction;
                ActionLogger.Content = $"The {filepath.Substring(filepath.LastIndexOf("\\") + 1)} was run.";
            }
            else
            {
                ActionLogger.Content = $"The {filepath.Substring(filepath.LastIndexOf("\\") + 1)} will be run in {secondsTimespan.Hours} h, {secondsTimespan.Minutes} m, {secondsTimespan.Seconds} s ({endTime:dd. MM. yyyy HH:mm:ss}).";
            }
        }
        private void SendNotification(String title, String message)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier("AutoComputerShutdown").Show(toast);
        }
        private void LogoutAction(object sender, EventArgs e)
        {
            TimeSpan secondsTimespan = new TimeSpan(0, 0, --countDownSeconds);

            if (secondsTimespan.TotalSeconds == 120)
            {
                SendNotification("Log out in two minutes!", $"You will be logged out in two minutes!");
            }

            if (secondsTimespan.TotalSeconds == 0)
            {
                ProcessStartInfo process = new ProcessStartInfo("logoff")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(process);
            }

            ActionLogger.Content = $"The user will be logged out in {secondsTimespan.Hours} h, {secondsTimespan.Minutes} m, {secondsTimespan.Seconds} s ({endTime:dd. MM. yyyy HH:mm:ss}).";
        }

        private void RestartAction(object sender, EventArgs e)
        {
            TimeSpan secondsTimespan = new TimeSpan(0, 0, --countDownSeconds);

            if (secondsTimespan.TotalSeconds == 120)
            {
                SendNotification("Restart in two minutes!", $"The computer will be restarted in two minutes!");
            }

            if (secondsTimespan.TotalSeconds == 0)
            {
                ProcessStartInfo process = new ProcessStartInfo("shutdown", "/r /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(process);
            }

            ActionLogger.Content = $"The computer will restart in {secondsTimespan.Hours} h, {secondsTimespan.Minutes} m, {secondsTimespan.Seconds} s ({endTime:dd. MM. yyyy HH:mm:ss}).";
        }

        private void ShutdownAction(object sender, EventArgs e)
        {
            TimeSpan secondsTimespan = new TimeSpan(0, 0, --countDownSeconds);

            if (secondsTimespan.TotalSeconds == 120)
            {
                SendNotification("Shutdown in two minutes!", $"The computer will be shut down in two minutes!");
            }
            if (secondsTimespan.TotalSeconds == 0)
            {
                ProcessStartInfo process = new ProcessStartInfo("shutdown", "/s /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(process);
            }

            ActionLogger.Content = $"The computer will shutdown in {secondsTimespan.Hours} h, {secondsTimespan.Minutes} m, {secondsTimespan.Seconds} s ({endTime:dd. MM. yyyy HH:mm:ss}).";
        }
        private void DecrementTime(object sender, RoutedEventArgs e)
        {
            timeSpinner.DecreaseTime();
        }
        private void IncrementTime(object sender, RoutedEventArgs e)
        {
            timeSpinner.IncreaseTimer();
        }
        private void RunFileChecked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a file to run",
                CheckFileExists = true,
                CheckPathExists = true
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                filepath = openFileDialog.FileName;
            }
            else
            {
                RunRadio.IsChecked = false;
                ShutdownRadio.IsChecked = true;
            }
        }
    }
}
