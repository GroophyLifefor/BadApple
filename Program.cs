using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using AForge.Video;
using AForge.Video.DirectShow;
using ConsoleObjectHandler;
using static System.Net.Mime.MediaTypeNames;
using Font = ConsoleObjectHandler.Font;

namespace BadApple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Font.SetFont("Consolas", 9);
            Console.SetWindowSize(240, 80);
            Console.CursorVisible = false;
            FileVideoSource videoSource = new FileVideoSource( "BadApple.mp4" );
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.PlayingFinished += (sender, reason) => { 
                Console.WriteLine("end");
                Console.ReadKey();
            };
            videoSource.VideoSourceError += (sender, eventArgs) => { Console.WriteLine("Install https://www.codecguide.com/download_k-lite_codec_pack_mega.htm"); };
            videoSource.Start( );
            Process.Start("run.bat");
            Console.WriteLine("Initalizing...");
        }

        private static string[] _AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", "&nbsp;" };

        private static int framecount = 0;
        private static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = eventArgs.Frame;
            if (framecount % 5 == 0 && framecount != 0)
            {
                Console.Clear();
                image = ResizeBitmap(image, 240, 180);
                Boolean toggle = false;
                for (int h = 0; h < image.Height; h++)
                {
                    for (int w = 0; w < image.Width; w++)
                    {
                        Color pixelColor = image.GetPixel(w, h);
                        int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        Color grayColor = Color.FromArgb(red, green, blue);

                        if (!toggle)
                        {
                            int index = (grayColor.R * 10) / 255;
                            FastConsole.Write(_AsciiChars[index].Replace("&nbsp;", " "));

                        }
                    }
                    if (!toggle)
                    {
                        FastConsole.Write("\n");

                        toggle = true;
                    }
                    else
                    {
                        toggle = false;
                    }
                }
                FastConsole.Flush();

            }
            if (framecount == 30) framecount = 0;
            else framecount++;
        }

        static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
    }

    public static class FastConsole
    {
        static readonly BufferedStream str;

        static FastConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;

            str = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
        }

        public static void WriteLine(String s) { Write(s + "\r\n"); }

        public static void Write(String s)
        {
            var rgb = new byte[s.Length << 1];
            Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);

            lock (str)  
                str.Write(rgb, 0, rgb.Length);
        }

        public static void Flush() { lock (str) str.Flush(); }
    }
}