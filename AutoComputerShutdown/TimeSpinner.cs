using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoComputerShutdown
{
    public class TimeSpinner
    {
        public int Seconds { get { return int.Parse(secondsTextBox.Text); } }
        public int Minutes { get { return int.Parse(minutesTextBox.Text); } }
        public int Hours { get { return int.Parse(hoursTextBox.Text); } }
        public int Days { get { return dueDate.SelectedDate.GetValueOrDefault().DayOfYear; } }

        private readonly ItemsControl itemsControl;
        private static TextBox selectedTextBox;
        private TextBox secondsTextBox;
        private TextBox minutesTextBox;
        private TextBox hoursTextBox;
        private readonly DatePicker dueDate;

        private static readonly IDictionary<Key, int> numericKeys = new Dictionary<Key, int> {
        { Key.D0, 0 },
        { Key.D1, 1 },
        { Key.D2, 2 },
        { Key.D3, 3 },
        { Key.D4, 4 },
        { Key.D5, 5 },
        { Key.D6, 6 },
        { Key.D7, 7 },
        { Key.D8, 8 },
        { Key.D9, 9 },
        { Key.NumPad0, 0 },
        { Key.NumPad1, 1 },
        { Key.NumPad2, 2 },
        { Key.NumPad3, 3 },
        { Key.NumPad4, 4 },
        { Key.NumPad5, 5 },
        { Key.NumPad6, 6 },
        { Key.NumPad7, 7 },
        { Key.NumPad8, 8 },
        { Key.NumPad9, 9 }
   };

        public TimeSpinner(bool initializeCurrentTime, ItemsControl refItemsControl, DatePicker refDueDate)
        {
            itemsControl = refItemsControl;
            dueDate = refDueDate;
            InitializeSpinner(initializeCurrentTime);
        }

        public TimeSpinner(bool initializeCurrentTime, ItemsControl refItemsControl)
        {
            itemsControl = refItemsControl;
            InitializeSpinner(initializeCurrentTime);
        }
        public void IncreaseTimer()
        {
            if (selectedTextBox == null)
            {
                return;
            }
            switch (selectedTextBox.Tag)
            {
                case "hour":
                    IncrementHours(1);
                    break;
                case "minute":
                    IncrementMinutes(1);
                    break;
                case "second":
                    IncrementSeconds(1);
                    break;
                default:
                    break;
            }
            selectedTextBox.Focus();

        }

        public void DecreaseTime()
        {
            if (selectedTextBox == null)
            {
                return;
            }
            switch (selectedTextBox.Tag)
            {
                case "hour":
                    IncrementHours(-1);
                    break;
                case "minute":
                    IncrementMinutes(-1);
                    break;
                case "second":
                    IncrementSeconds(-1);
                    break;
                default:
                    break;
            }
            selectedTextBox.Focus();
        }
        private void IncrementSeconds(int value)
        {
            if (secondsTextBox.Text.Equals("59") && value > 0)
            {
                secondsTextBox.Text = "00";
                IncrementMinutes(value);
            }
            else if (secondsTextBox.Text.Equals("00") && value < 0)
            {
                secondsTextBox.Text = "59";
                IncrementMinutes(value);
            }
            else
            {
                int newValue = int.Parse(secondsTextBox.Text) + value;
                secondsTextBox.Text = newValue.ToString("00");
            }
        }
        private void IncrementMinutes(int value)
        {
            if (minutesTextBox.Text.Equals("59") && value > 0)
            {
                minutesTextBox.Text = "00";
                IncrementHours(value);
            }
            else if (minutesTextBox.Text.Equals("00") && value < 0)
            {
                minutesTextBox.Text = "59";
                IncrementHours(value);
            }
            else
            {
                int newValue = int.Parse(minutesTextBox.Text) + value;
                minutesTextBox.Text = newValue.ToString("00");
            }
        }

        private void IncrementHours(int value)
        {
            if (hoursTextBox.Text.Equals("23") && value > 0)
            {
                hoursTextBox.Text = "00";
                IncrementDays(value);
            }
            else if (hoursTextBox.Text.Equals("00") && value < 0)
            {
                hoursTextBox.Text = "23";
                IncrementDays(value);
            }
            else
            {
                int newValue = int.Parse(hoursTextBox.Text) + value;
                hoursTextBox.Text = newValue.ToString("00");
            }
        }

        private void IncrementDays(int value)
        {
            dueDate.SelectedDate = ((DateTime)dueDate.SelectedDate).AddDays(value);
        }
        private void InitializeSpinner(bool initializeCurrentTime)
        {
            List<Control> list = new List<Control>();

            hoursTextBox = new TextBox
            {
                Text = initializeCurrentTime ? DateTime.Now.Hour.ToString("D2") : "00",
                BorderThickness = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                AcceptsReturn = false,
                AcceptsTab = false,
                AllowDrop = false
            };

            hoursTextBox.GotFocus += SelectAll;
            hoursTextBox.PreviewMouseDown += IgnoreAll;
            hoursTextBox.TextChanged += CheckValues;
            hoursTextBox.KeyDown += TextBox_KeyDown;
            hoursTextBox.Tag = "hour";

            Label doubleDot = new Label
            {
                Content = ":",
                Width = 15,
                Height = 24,
                FontSize = 11,
                Margin = new Thickness(-3, -3, -7, 0)
            };

            Label doubleDot2 = new Label
            {
                Content = ":",
                Width = 15,
                Height = 24,
                FontSize = 11,
                Margin = new Thickness(-3, -3, -7, 0)
            };

            minutesTextBox = new TextBox
            {
                Text = initializeCurrentTime ? DateTime.Now.Minute.ToString("D2") : "00",
                BorderThickness = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                AcceptsReturn = false,
                AcceptsTab = false,
                AllowDrop = false
            };
            minutesTextBox.Tag = "minute";
            minutesTextBox.TextChanged += CheckValues;
            minutesTextBox.GotFocus += SelectAll;
            minutesTextBox.PreviewMouseDown += IgnoreAll;
            minutesTextBox.KeyDown += TextBox_KeyDown;

            secondsTextBox = new TextBox
            {
                Text = initializeCurrentTime ? DateTime.Now.Second.ToString("D2") : "00",
                BorderThickness = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                AcceptsReturn = false,
                AcceptsTab = false,
                AllowDrop = false
            };
            secondsTextBox.GotFocus += SelectAll;
            secondsTextBox.PreviewMouseDown += IgnoreAll;
            secondsTextBox.Tag = "second";
            secondsTextBox.TextChanged += CheckValues;
            secondsTextBox.KeyDown += TextBox_KeyDown;

            list.Add(hoursTextBox);
            list.Add(doubleDot2);
            list.Add(minutesTextBox);
            list.Add(doubleDot);
            list.Add(secondsTextBox);
            itemsControl.ItemsSource = list;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            int myCaretIndex = selectedTextBox.CaretIndex;
            char[] characters = selectedTextBox.Text.ToCharArray();

            if (myCaretIndex < characters.Length)
            {
                if (numericKeys.ContainsKey(e.Key))
                {
                    characters[myCaretIndex] = char.Parse(GetKeyNumericValue(e).ToString());
                }
                else
                {
                    characters[myCaretIndex] = char.Parse(e.Key.ToString());
                }

                selectedTextBox.Text = string.Join("", characters);

                selectedTextBox.CaretIndex = myCaretIndex + 1;

                e.Handled = true;
            }
        }

        private int GetKeyNumericValue(KeyEventArgs e)
        {
            if (numericKeys.ContainsKey(e.Key)) return numericKeys[e.Key];
            else return 0;
        }

        private void CheckValues(object sender, TextChangedEventArgs e)
        {
            if (selectedTextBox.Text.Length == 0)
            {
                selectedTextBox.Text = "00";
                selectedTextBox.Focus();
                return;
            }
            int inputValue = int.Parse(selectedTextBox.Text);

            if (selectedTextBox.Tag.Equals("hour"))
            {
                if (inputValue > 23)
                {
                    selectedTextBox.Text = "00";
                }
            }
            else
            {
                if (inputValue > 59)
                {
                    selectedTextBox.Text = "00";
                }
            }
            if (selectedTextBox.Text.Length > 2)
            {
                selectedTextBox.Text = selectedTextBox.Text.Remove(selectedTextBox.Text.IndexOf('0'));
            }
            selectedTextBox.Focus();
        }
        private void IgnoreAll(object sender, MouseButtonEventArgs e)
        {
            var TextBox = (TextBox)sender;
            if (!TextBox.IsKeyboardFocusWithin)
            {
                TextBox.Focus();
                e.Handled = true;
            }
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            selectedTextBox = box;
            box.SelectAll();
            e.Handled = true;
        }
    }
}
