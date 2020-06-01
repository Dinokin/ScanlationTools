using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dinokin.ScanlationTools.Interfaces;
using Dinokin.ScanlationTools.Others;
using ImageMagick;
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
            if (_processing)
            {
                MessageBox.Show(ScanlationTools.Resources.BusyPleaseAwait, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            var menuItem = (MenuItem) sender;

            switch (menuItem.Name)
            {
                case "FunctionConverter":
                    _selectedFunction = SelectableFunctions.Converter;
                    FunctionResizer.IsChecked = false;
                    FunctionBorderRemover.IsChecked = false;
                    FunctionPageJoiner.IsChecked = false;
                    break;
                case "FunctionResizer":
                    _selectedFunction = SelectableFunctions.Resizer;
                    FunctionConverter.IsChecked = false;
                    FunctionBorderRemover.IsChecked = false;
                    FunctionPageJoiner.IsChecked = false;
                    break; 
                case "FunctionBorderRemover":
                    _selectedFunction = SelectableFunctions.BorderRemover;
                    FunctionConverter.IsChecked = false;
                    FunctionResizer.IsChecked = false;
                    FunctionPageJoiner.IsChecked = false;
                    break;
                case "FunctionPageJoiner":
                    _selectedFunction = SelectableFunctions.PageJoiner;
                    FunctionConverter.IsChecked = false;
                    FunctionResizer.IsChecked = false;
                    FunctionBorderRemover.IsChecked = false;
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


        private async void ReceiveFiles(object sender, DragEventArgs dragEventArgs)
        { 
            if (_processing)
            {
                MessageBox.Show(this, ScanlationTools.Resources.BusyPleaseAwait, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            if (_selectedFunction == SelectableFunctions.None)
            {
                MessageBox.Show(this, ScanlationTools.Resources.PleaseSelectFunction, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            if (_selectedOutputFormat == SelectableOutputFormats.None)
            {
                MessageBox.Show(this, ScanlationTools.Resources.PleaseSelectOutputFormat, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            var folderDialog = new VistaFolderBrowserDialog { ShowNewFolderButton = true};
            
            folderDialog.ShowDialog(this);

            if (string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                MessageBox.Show(this, ScanlationTools.Resources.PleaseSelectOutputPath, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            var location = new DirectoryInfo(folderDialog.SelectedPath);
            var images = (await ImageIO.FetchImages(((string[]) dragEventArgs.Data.GetData(DataFormats.FileDrop)).Select(path => new FileInfo(path)))).ToArray();

            if (!images.Any())
            {
                MessageBox.Show(this, ScanlationTools.Resources.NoSupportedImageFormatFound, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }

            switch (_selectedFunction)
            {
                case SelectableFunctions.Converter:
                    await RunFunction("Converter", null, images, location);
                    break;
                case SelectableFunctions.Resizer:
                    var resizerDialog = new InsertTextDialog(ScanlationTools.Resources.Resizer, ScanlationTools.Resources.InsertResizePercentage, true);
                    resizerDialog.ShowDialog();

                    if (resizerDialog.Input == null )
                    {
                        MessageBox.Show(this, ScanlationTools.Resources.InvalidValueInserted, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                        return;
                    }
                    
                    await RunFunction("Resizer", new []{(name: "percentage", value: resizerDialog.Input)}, images, location);
                    break;
                case SelectableFunctions.BorderRemover:
                    await RunFunction("BorderRemover", null, images, location);
                    break;
                case SelectableFunctions.PageJoiner:
                    var pageJoinerDialog = new InsertTextDialog(ScanlationTools.Resources.PageJoiner, ScanlationTools.Resources.InsertNumberOfPagesPerJoinedPage, true);
                    pageJoinerDialog.ShowDialog();

                    if (pageJoinerDialog.Input == null )
                    {
                        MessageBox.Show(this, ScanlationTools.Resources.InvalidValueInserted, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                        return;
                    }
                    
                    await RunFunction("PageJoiner", new []{(name: "pagesPerPage", value: pageJoinerDialog.Input)}, images, location);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task RunFunction(string functionName, IEnumerable<(string name, string value)>? arguments, IEnumerable<MagickImage> images, DirectoryInfo destination)
        {
            images = images.ToArray();
            
            SetStatus(true);
            SetupProgressBar((ushort) images.Count(), 3);
            AddProgress((ushort) images.Count());
            
            var function = _app.GetServices<IFunction>().Single(target => target.Name == functionName);
            var result = await function.DoWork(images, arguments, new Progress<ushort>(AddProgress));
            
            await ImageIO.SaveImages(result, _selectedOutputFormat, destination, new Progress<ushort>(AddProgress));
            MessageBox.Show(this, ScanlationTools.Resources.TaskCompletedSuccessfully, ScanlationTools.Resources.TaskCompletedSuccessfully, MessageBoxButton.OK, MessageBoxImage.Information);
            SetStatus(false);
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
            
            SetupProgressBar((ushort) addresses.Length, 2);

            var images = await ripper.RipPages(addresses, new Progress<ushort>(AddProgress));
            await ImageIO.SaveImages(images, _selectedOutputFormat, new DirectoryInfo(folderDialog.SelectedPath), new Progress<ushort>(AddProgress));
            MessageBox.Show(this, ScanlationTools.Resources.TaskCompletedSuccessfully, ScanlationTools.Resources.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            SetStatus(false);
        }
        
        private void SetStatus(bool active)
        { 
            ProgressBar.Value = 0;
            
            if (active)
            {
                _processing = true;
                FileReceiver.Content = ScanlationTools.Resources.PleaseWait;

                return;
            }
            
            _processing = false;
            FileReceiver.Content = ScanlationTools.Resources.DropFilesHere;
        }

        private void SetupProgressBar(ushort numberOfPages, ushort numberOfInteractions)
        {
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = numberOfInteractions * numberOfPages;
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