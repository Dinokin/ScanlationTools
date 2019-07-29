using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Rippers;
using ImageMagick;

namespace Dinokin.ScanlationTools
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var a = await new EBookJapan().GetImages(new Uri("https://ebookjapan.yahoo.co.jp/books/191412/A002201450/"), TimeSpan.FromSeconds(2));
            /*
            if (args.Length > 0)
                switch (args[0])
                {
                    case "alphapolis":
                        if (args.Length == 4)
                        {
                            var images = await new Alphapolis().GetImages(new Uri(args[1]));
                            ImageSaver.SaveImages(images, ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine("One or more arguments are missing. Use argument \"help\" for usage details.");
                        break;
                    case "comicride":
                        if (args.Length == 4)
                        {
                            var images = await new ComicRider().GetImages(new Uri(args[1]));
                            ImageSaver.SaveImages(images, ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine("One or more arguments are missing. Use argument \"help\" for usage details.");
                        break;
                    case "help":
                        Console.Write(GetHelp());
                        break;
                    default:
                        Console.WriteLine("Invalid Argument. Use argument \"help\" for usage details.");
                        break;
                }
            else
                Console.WriteLine("No argument found. Use argument \"help\" for usage details.");
                */
        }

        private static string GetHelp()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("Usage Information");
            sb.AppendLine();
            sb.AppendLine("Usage: ScanlationTools <module> <options>");
            sb.AppendLine();
            sb.AppendLine("Available Formats:");
            sb.AppendLine("jpg: Standard Joint Photographic Experts Group (JPEG) File Format.");
            sb.AppendLine("png: Portable Network Graphics File Format. Uses PNG-8 with b/w images and PNG-24 on colored images.");
            sb.AppendLine("psd: Standard Photoshop Document");
            sb.AppendLine();
            sb.AppendLine("Module: Alphapolis");
            sb.AppendLine("Description: Module dedicated to rip manga pages from alphapolis.co.jp.");
            sb.AppendLine("Usage: ScanlationTools alphapolis <chapterurl> <outputdir> <fileformat>");
            sb.AppendLine();
            sb.AppendLine("Module: ComicRider");
            sb.AppendLine("Description: Module can rip images from sites like comicride.jp, comic-meteor.jp and comic-polaris.jp.");
            sb.AppendLine("Usage: ScanlationTools comicrider <chapterurl> <outputdir> <fileformat>");
            
            return sb.ToString();
        }

        private static MagickFormat ParseFormat(string format)
        {
            switch (format)
            {
                case "jpg":
                    return MagickFormat.Jpg;
                case "png":
                    return MagickFormat.Png8;
                case "psd":
                    return MagickFormat.Psd;
                default:
                    return MagickFormat.Unknown;
            }
        }
    }
}