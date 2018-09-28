using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace Enigma
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlugBoard plugBoard;
        private Encoder encoder;

        public MainWindow()
        {
            InitializeComponent();
            plugBoard = new PlugBoard(plugListBox);
            encoder = new Encoder(plugBoard);
            DataBinding();
        }


        private void DataBinding()
        {
            rotorTextBox.DataContext = ringTextBox.DataContext = keyTextBox.DataContext =
            plainTextBox.DataContext = cipherTextBox.DataContext = encoder;

        }

        private void plugListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             removeButton.IsEnabled = plugListBox.SelectedItems.Count > 0;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            plugBoard.Add(fromTextBox.Text, toTextBox.Text);
            removeButton.IsEnabled = plugListBox.SelectedItems.Count > 0;
            clearButton.IsEnabled = plugListBox.Items.Count > 0;
            addButton.IsEnabled = plugListBox.Items.Count < 13;
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            plugBoard.Remove();
            removeButton.IsEnabled = plugListBox.SelectedItems.Count > 0;
            clearButton.IsEnabled = plugListBox.Items.Count > 0;
            addButton.IsEnabled = plugListBox.Items.Count < 13;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            plugBoard.Clear();
            addButton.IsEnabled = true;
            removeButton.IsEnabled = clearButton.IsEnabled = false;
        }

        private void addExpander_Click(object sender, RoutedEventArgs e)
        {
            expander.IsExpanded = !expander.IsExpanded;

        }

        private void fromTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = e.Text.Trim().ToUpper();
            if(text.Length>0)
            {
                char c = text[0];
                if (c >= 'A' && c <= 'Z')
                    (sender as TextBox).Text = c.ToString();
            }
            e.Handled = true;
        }

        private void expander_Slide(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists("Resources/Slide.wav"))
                    throw new Exception();
                var player = new MediaPlayer();
                player.Open(new Uri("Resources/Slide.wav", UriKind.Relative));
                player.Play();
            }
            catch { }
        }

        private void encodeButton_Click(object sender, RoutedEventArgs e)
        {
             encoder.BeginEncode(this, true);
        }

        private void decodeButton_Click(object sender, RoutedEventArgs e)
        {
            encoder.BeginEncode(this, false);
        }
    }
}
