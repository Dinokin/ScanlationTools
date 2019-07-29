using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace Dinokin.ScanlationTools.Rippers
{
    public class EBookJapan : IDelayedRipper
    {
        public async Task<MagickImage[]> GetImages(Uri uri, TimeSpan waitTimeInSeconds)
        {
            var firefoxOptions = new FirefoxOptions();
            //firefoxOptions.AddArgument("-headless");
            
            var firefoxDriver = new FirefoxDriver(new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).AbsolutePath.Replace("%20", " ")).Directory.FullName, firefoxOptions);
            //firefoxDriver.Manage().Window.Size = new Size(1080*4, 1920*4);
            firefoxDriver.Url = uri.AbsoluteUri;
            firefoxDriver.FindElementByXPath("/html/body/div/div/div/div[5]/div[1]/div/div/div[1]/div[2]/div[3]/div[3]/a").Click();
            
            var actionsProvider = new Actions(firefoxDriver).SendKeys(Keys.ArrowLeft).Build();
            var chapterURL = firefoxDriver.Url;
            var images = new List<MagickImage>();

            await Task.Delay(TimeSpan.FromSeconds(5));

            while (chapterURL == firefoxDriver.Url)
            {
                images.Add(new MagickImage(firefoxDriver.GetScreenshot().AsByteArray));
                actionsProvider.Perform();
                //await Task.Delay(waitTimeInSeconds);
            }

            return null;
        }
    }
}