using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Rippers;
using Dinokin.ScanlationTools.Tools;

namespace Dinokin.ScanlationTools
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length > 0)
                switch (args[0])
                {
                    case "alphapolis":
                        if (args.Length == 4)
                            await ImageDealer.SaveImages(await new Alphapolis().GetImages(new Uri(args[1])), ImageDealer.ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "comicride":
                        if (args.Length == 4)
                        {
                            await ImageDealer.SaveImages(await new ComicRider().GetImages(new Uri(args[1])), ImageDealer.ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "webace":
                        if (args.Length == 4)
                            await ImageDealer.SaveImages(await new WebAce().GetImages(new Uri(args[1])), ImageDealer.ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "webtoonjoiner":
                        if (args.Length == 5)
                        {
                            var images = await WebtoonJoiner.Join(await ImageDealer.FetchImages(new DirectoryInfo(args[1])), ushort.Parse(args[3]));
                            await ImageDealer.SaveImages(images, ImageDealer.ParseFormat(args[4]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "resizepercent":
                        if (args.Length == 5)
                        {
                            var images = await Resizer.Percent(await ImageDealer.FetchImages(new DirectoryInfo(args[1])), double.Parse(args[3]));
                            await ImageDealer.SaveImages(images, ImageDealer.ParseFormat(args[4]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "removetransparent":
                        if (args.Length == 4)
                        {
                            var images = await BorderRemover.RemoveTransparentBorders(await ImageDealer.FetchImages(new DirectoryInfo(args[1])));
                            await ImageDealer.SaveImages(images, ImageDealer.ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        }
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

                        break;
                    case "convert":
                        if (args.Length == 4)
                            await ImageDealer.SaveImages(await ImageDealer.FetchImages(new DirectoryInfo(args[1])), ImageDealer.ParseFormat(args[3]), Directory.CreateDirectory(args[2]));
                        else
                            Console.WriteLine(
                                "One or more arguments are missing. Use argument \"help\" for usage details.");

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
            sb.AppendLine();
            sb.AppendLine("Module: WebAce");
            sb.AppendLine("Description: Module dedicated to rip manga pages from web-ace.jp.");
            sb.AppendLine("Usage: ScanlationTools webace <chapterurl> <outputdir> <fileformat>");
            sb.AppendLine();
            sb.AppendLine("Module: Webtoon Joiner");
            sb.AppendLine("Description: Join webtoon pages vertically.");
            sb.AppendLine("Usage: ScanlationTools webtoonjoiner <origindir> <outputdir> <imagesperpage> <fileformat>");
            sb.AppendLine();
            sb.AppendLine("Module: Resizer");
            sb.AppendLine("Description: Resize images by the specified percent.");
            sb.AppendLine("Usage: ScanlationTools resizepercent <origindir> <outputdir> <percent> <fileformat>");
            sb.AppendLine();
            sb.AppendLine("Module: Border Remover");
            sb.AppendLine("Description: Remove transparent borders from images.");
            sb.AppendLine("Usage: ScanlationTools removetransparent <origindir> <outputdir> <fileformat>");
            sb.AppendLine();
            sb.AppendLine("Module: Convert");
            sb.AppendLine("Description: Convert images from one of the supported formats to another supported format.");
            sb.AppendLine("Usage: ScanlationTools convert <origindir> <outputdir> <fileformat>");

            return sb.ToString();
        }
    }
}