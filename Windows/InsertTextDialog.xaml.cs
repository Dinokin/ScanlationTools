using System.Windows;

namespace Dinokin.ScanlationTools.Windows
{
    /// <summary>
    /// Interaction logic for InsertTextDialog.xaml
    /// </summary>
    public partial class InsertTextDialog : Window
    {
        public string? Input { get; private set; }

        public InsertTextDialog(string windowTitle, string message)
        {
            InitializeComponent();

            Title = windowTitle;
            Message.Text = message;
        }

        private void OkButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Input = Answer.Text;

            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs routedEventArgs) => Close();
    }
}
