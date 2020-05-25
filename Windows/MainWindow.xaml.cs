using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dinokin.ScanlationTools.Functions;
using Dinokin.ScanlationTools.Others;
using ImageMagick;
using Ookii.Dialogs.Wpf;

namespace Dinokin.ScanlationTools.Windows
{
    public partial class MainWindow
    {
        private Others.Functions _selectedFunction = Others.Functions.None;
        private OutputFormats _selectedOutput = OutputFormats.None;
        private bool _processing;

        public MainWindow() => InitializeComponent();

        private void SelectFunction(object sender, RoutedEventArgs routedEventArgs)
        {
            var trigger = (MenuItem) sender;
            
            ChangeFunction((Others.Functions) Enum.Parse(typeof(Others.Functions), trigger.Name.Replace("Function", string.Empty), true));
        }
        
        private void SelectOutput(object sender, RoutedEventArgs routedEventArgs)
        {
            var trigger = (MenuItem) sender;
            
            ChangeOutput((OutputFormats) Enum.Parse(typeof(OutputFormats), trigger.Name.Replace("Output", string.Empty), true));
        }

        private void SelectRipper(object sender, RoutedEventArgs routedEventArgs)
        {
            throw new NotImplementedException();
        }

        private async void ReceiveFiles(object sender, DragEventArgs dragEventArgs)
        {
            if (_processing)
            {
                MessageBox.Show(this, ScanlationTools.Resources.BusyPleaseAwait, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            if (_selectedFunction == Others.Functions.None)
            {
                MessageBox.Show(this, ScanlationTools.Resources.PleaseSelectFunction, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            
            if (_selectedOutput == OutputFormats.None)
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
            SetFileReceiverState(true);

            var images = (await ImageDealer.FetchImages(((string[]) dragEventArgs.Data.GetData(DataFormats.FileDrop)).Select(path => new FileInfo(path)))).ToArray();

            if (!images.Any())
            {
                MessageBox.Show(this, ScanlationTools.Resources.NoSupportedImageFormatFound, ScanlationTools.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                
                SetFileReceiverState(false);
                
                return;
            }

            await ImageDealer.SaveImages(await RunFunction(images), _selectedOutput, location);
            MessageBox.Show(this, ScanlationTools.Resources.TaskCompletedSuccessfully, ScanlationTools.Resources.TaskCompletedSuccessfully, MessageBoxButton.OK, MessageBoxImage.Information);
            SetFileReceiverState(false);
        }

        private void ChangeFunction(Others.Functions function)
        {
            var convertMenuItem = (MenuItem) FindName("FunctionConvert");
            var resizerMenuItem = (MenuItem) FindName("FunctionResizer");
            var borderRemoverMenuItem = (MenuItem) FindName("FunctionBorderRemover");
            var webtoonJoinerMenuItem = (MenuItem) FindName("FunctionWebtoonJoiner");
            
            switch (function)
            {
                case Others.Functions.Convert:
                    _selectedFunction = Others.Functions.Convert;
                    convertMenuItem.IsChecked = true;
                    resizerMenuItem.IsChecked = false;
                    borderRemoverMenuItem.IsChecked = false;
                    webtoonJoinerMenuItem.IsChecked = false;
                    break;
                case Others.Functions.ResizeByPercent:
                    _selectedFunction = Others.Functions.ResizeByPercent;
                    convertMenuItem.IsChecked = false;
                    resizerMenuItem.IsChecked = true;
                    borderRemoverMenuItem.IsChecked = false;
                    webtoonJoinerMenuItem.IsChecked = false;
                    break;
                case Others.Functions.TransparentBorderRemover:
                    _selectedFunction = Others.Functions.TransparentBorderRemover;
                    convertMenuItem.IsChecked = false;
                    resizerMenuItem.IsChecked = false;
                    borderRemoverMenuItem.IsChecked = true;
                    webtoonJoinerMenuItem.IsChecked = false;
                    break;
                case Others.Functions.WebtoonPageJoiner:
                    _selectedFunction = Others.Functions.WebtoonPageJoiner;
                    convertMenuItem.IsChecked = false;
                    resizerMenuItem.IsChecked = false;
                    borderRemoverMenuItem.IsChecked = false;
                    webtoonJoinerMenuItem.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(function), function, null);
            }
        }

        private void ChangeOutput(OutputFormats output)
        {
            var pngMenuItem = (MenuItem) FindName("OutputPNG");
            var jpgMenuItem = (MenuItem) FindName("OutputJPG");
            var psdMenuItem = (MenuItem) FindName("OutputPSD");

            switch (output)
            {
                case OutputFormats.PNG:
                    _selectedOutput = OutputFormats.PNG;
                    pngMenuItem.IsChecked = true;
                    jpgMenuItem.IsChecked = false;
                    psdMenuItem.IsChecked = false;
                    break;
                case OutputFormats.JPG:
                    _selectedOutput = OutputFormats.JPG;
                    pngMenuItem.IsChecked = false;
                    jpgMenuItem.IsChecked = true;
                    psdMenuItem.IsChecked = false;
                    break;
                case OutputFormats.PSD:
                    _selectedOutput = OutputFormats.PSD;
                    pngMenuItem.IsChecked = false;
                    jpgMenuItem.IsChecked = false;
                    psdMenuItem.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(output), output, null);
            }
        }

        private async Task<IEnumerable<MagickImage>> RunFunction(IEnumerable<MagickImage> images) => _selectedFunction switch
        {
            Others.Functions.Convert => images,
            Others.Functions.ResizeByPercent => await Resizer.Percent(images, GetResizePercentDialog()),
            Others.Functions.TransparentBorderRemover => await BorderRemover.RemoveTransparentBorders(images),
            Others.Functions.WebtoonPageJoiner => await WebtoonPageJoiner.JoinPages(images, GetImagesPerPageDialog()),
            _ => throw new ArgumentOutOfRangeException(nameof(_selectedFunction), _selectedFunction, null)
        };

        private void SetFileReceiverState(bool active)
        {
            var fileReceiver = (Label) FindName("FileReceiver");

            if (active)
            {
                _processing = true;
                fileReceiver.Content = ScanlationTools.Resources.PleaseWait;

                return;
            }
            
            _processing = false;
            fileReceiver.Content = ScanlationTools.Resources.DropFilesHere;
        }

        private float GetResizePercentDialog()
        {
            throw new NotImplementedException();
        }

        private ushort GetImagesPerPageDialog()
        {
            throw new NotImplementedException();
        }
    }
}