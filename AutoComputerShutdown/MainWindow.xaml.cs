using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AutoComputerShutdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DispatcherTimer LiveTime { get; private set; }

        public Label LabelLogger { get; private set; }
        public bool IsCountdownMode { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ScanCurrentTime();
            LabelLogger = ActionLogger;
        }

        private void ScanCurrentTime()
        {
            Timer_Tick(null, null);
            LiveTime = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            LiveTime.Tick += Timer_Tick;
            LiveTime.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("dd. MM. yyyy HH:mm:ss");
        }
    }
}
