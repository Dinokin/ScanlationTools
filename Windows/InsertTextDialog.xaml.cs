using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Dinokin.ScanlationTools.Windows
{
    /// <summary>
    /// Interaction logic for InsertTextDialog.xaml
    /// </summary>
    public partial class InsertTextDialog : Window
    {
        public string? Input { get; private set; }

        private bool _onlyNumbers;
        
        public InsertTextDialog(string windowTitle, string message, bool onlyNumbers = false)
        {
            InitializeComponent();

            Title = windowTitle;
            Message.Text = message;
            _onlyNumbers = onlyNumbers;
        }

        private void OkButtonClick(object sender, RoutedEventArgs routeProcessText)
        {
            Input = Answer.Text;

            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs routedEventArgs) => Close();

        private void ProcessText(object sender, TextCompositionEventArgs textCompositionEventArgs) => textCompositionEventArgs.Handled = !IsValidInput(textCompositionEventArgs.Text);
        
        private bool IsValidInput(string text)
        {
            if (!_onlyNumbers)
                return true;

            return Regex.IsMatch(text, "[0-9]+");
        }
    }
}
