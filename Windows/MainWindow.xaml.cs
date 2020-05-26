using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dinokin.ScanlationTools.Interfaces;
using Dinokin.ScanlationTools.Others;
using Ookii.Dialogs.Wpf;

namespace Dinokin.ScanlationTools.Windows
{
    public partial class MainWindow
    {
        private readonly App _app = (App) Application.Current;
        private SelectableFunctions _selectedFunction = SelectableFunctions.None;
        private SelectableOutputFormats _selectedOutputFormat = SelectableOutputFormats.None;
        private bool _processing;

        public MainWindow() => InitializeComponent();

        private void SelectFunction(object sender, RoutedEventArgs routedEventArgs)
        {
            throw new NotImplementedException();
        }
        
        private void SelectOutput(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_processing)
            {
                MessageBox.Show(ScanlationTools.Resources.BusyPleaseAwait, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            var menuItem = (MenuItem) sender;

            switch (menuItem.Name)
            {
                case "OutputPNG":
                    OutputJPG.IsChecked = false;
                    OutputPSD.IsChecked = false;
                    _selectedOutputFormat = SelectableOutputFormats.PNG;
                    break;
                case "OutputJPG":
                    OutputPNG.IsChecked = false;
                    OutputPSD.IsChecked = false;
                    _selectedOutputFormat = SelectableOutputFormats.JPG;
                    break; 
                case "OutputPSD": 
                    OutputPNG.IsChecked = false;
                    OutputJPG.IsChecked = false;
                    _selectedOutputFormat = SelectableOutputFormats.PSD;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(menuItem.Name), menuItem.Name, null);
            }
        }

        private void SelectRipper(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_processing)
            {
                MessageBox.Show(ScanlationTools.Resources.BusyPleaseAwait, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            if (_selectedOutputFormat == SelectableOutputFormats.None)
            {
                MessageBox.Show(ScanlationTools.Resources.PleaseSelectOutputFormat, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            var menuItem = (MenuItem) sender;

            switch (menuItem.Name)
            {
                case "RippersAlphaPolis":
                    RippersComicRide.IsChecked = false;
                    RippersYoungAceUp.IsChecked = false;
                    RunRipper("AlphaPolis");
                    break;
                case "RippersComicRide":
                    RippersAlphaPolis.IsChecked = false;
                    RippersYoungAceUp.IsChecked = false;
                    RunRipper("ComicRIDE");
                    break; 
                case "RippersYoungAceUp": 
                    RippersAlphaPolis.IsChecked = false;
                    RippersComicRide.IsChecked = false;
                    RunRipper("Young Ace UP");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(menuItem.Name), menuItem.Name, null);
            }
        }

        private void ReceiveFiles(object sender, DragEventArgs dragEventArgs)
        {
            throw new NotImplementedException();
        }

        private async void RunRipper(string ripperName)
        {
            var ripper = _app.GetServices<IRipper>().Single(target => target.Name == ripperName);
            var urlDialog = new InsertTextDialog(ripperName, ScanlationTools.Resources.InsertAddressToChapter);
            urlDialog.ShowDialog();

            if (urlDialog.Input == null || !ripper.IsValidURI(new Uri(urlDialog.Input)))
            {
                MessageBox.Show(ScanlationTools.Resources.InvalidUrlInserted, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            var folderDialog = new VistaFolderBrowserDialog { ShowNewFolderButton = true};
            folderDialog.ShowDialog(this);

            if (string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                MessageBox.Show(this, ScanlationTools.Resources.PleaseSelectOutputPath, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            SetStatus(true);
            var addresses = (await ripper.GetPagesAddresses(new Uri(urlDialog.Input))).ToArray();

            if (!addresses.Any())
            {
                MessageBox.Show(this, ScanlationTools.Resources.NoSupportedImageFormatFound, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                SetStatus(false);
                
                return;
            }
            
            SetupProgressBar((ushort) addresses.Length, 2, true);

            var images = await ripper.RipPages(addresses, new Progress<ushort>(AddProgress));
            await ImageIO.SaveImages(images, _selectedOutputFormat, new DirectoryInfo(folderDialog.SelectedPath), new Progress<ushort>(AddProgress));
            MessageBox.Show(this, ScanlationTools.Resources.TaskCompletedSuccessfully, ScanlationTools.Resources.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            SetStatus(false);
        }
        
        private void SetStatus(bool active)
        {
            if (active)
            {
                ProgressBar.Value = 0;
                _processing = true;
                FileReceiver.Content = ScanlationTools.Resources.PleaseWait;

                return;
            }
            
            ProgressBar.Value = 0;
            _processing = false;
            FileReceiver.Content = ScanlationTools.Resources.DropFilesHere;
        }

        private void SetupProgressBar(ushort numberOfPages, ushort numberOfInteractions, bool resetProgress = false)
        {
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = numberOfInteractions * numberOfPages;

            if (resetProgress)
                ProgressBar.Value = 0;
        }

        private void AddProgress(ushort amount)
        {
            if (ProgressBar.Value + amount <= ProgressBar.Maximum)
                ProgressBar.Value += amount;
            else if (ProgressBar.Value + amount > ProgressBar.Maximum)
                ProgressBar.Value = ProgressBar.Maximum;
        }
    }
}