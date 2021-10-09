using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InfinitodeBot.Models
{
    internal class GameClient
    {
        public const int WIDTH = 1920;
        public const int HEIGTH = 1080;
        public IDictionary<TileName, ImageProcessor> tiles = new Dictionary<TileName, ImageProcessor>();
        public IDictionary<string, ImageProcessor> buy_tiles = new Dictionary<string, ImageProcessor>();
        AdbClient client;

        const int TILESIZE_MIN = 27;
        static int[] BAG_TILE_X = new int[6] { 470, 600, 730, 860, 990, 1120 };
        static int[] BAG_TILE_Y = new int[4] { 260, 400, 540, 680 };



        static Point BAG_OPEN_BTN = new Point(1400, 900);
        static Point OK_BTN = new Point(1345, 670);

        static Point screen_OpenMin = new Point(1245, 875);
        static Point screen_OpenMax = new Point(1410, 925);

        public GameClient()
        {
            StartServer();
            client = GetClient();
            // Init images
            LoadTiles();
        }

        void StartServer()
        {
            AdbServer server = new AdbServer();
            var result = server.StartServer(@"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe", restartServerIfNewer: false);
        }

        AdbClient GetClient()
        {
            var client = new AdbClient(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort), Factories.AdbSocketFactory);
            client.Connect("localhost:5585");
            return client;
        }

        IShellOutputReceiver output = new ConsoleOutputReceiver();
        public void Click(Point p, int delay = 100)
        {
            Click(p.X, p.Y);
            if (delay > 0)
                Thread.Sleep(delay);
        }
        public void Click(int x, int y)
        {
            client.ExecuteRemoteCommand($"input tap {x} {y}", client.GetDevices().LastOrDefault(), output);
        }

        public void Swipe(Point from, Point to, int delay = 150) => Swipe(from.X, from.Y, to.X, to.Y, delay);
        public void Swipe(int x, int y, int toX, int toY, int delay = 150)
        {
            client.ExecuteRemoteCommand($"input swipe {x} {y} {toX} {toY} {delay}", client.GetDevices().LastOrDefault(), output);
        }
        public void SaveImage()
        {
            GetScreenshot().ContinueWith(x =>
            {
                Console.WriteLine("Image received");
                x.Result.Save(@"S:\tmp\1.bmp");

            });
        }

        public Task<Image> GetScreenshot()
        {
            return client.GetFrameBufferAsync(client.GetDevices().LastOrDefault(), new CancellationToken());
        }

        void LoadTiles()
        {
            LoadTile(TileName.Plus_TopLeft, "top-left-plus.bmp");
            LoadTile(TileName.MapEditor_Corner_TopLeft, "map_editor_top_left.bmp");
            LoadTile(TileName.Bag_Left, "bag-left.bmp");
            LoadTile(TileName.ButtonBuy, "buy_btn.bmp");
            LoadTile(TileName.ButtonOpen, "open_btn.bmp");
            LoadTile(TileName.ButtonOk, "ok_btn.bmp");
            LoadTile(TileName.ButtonGreyOk, "ok_grey_btn.bmp");
            LoadTile(TileName.KeyGrey, "key_grey.bmp");
            LoadTile(TileName.Stack50, "bag-left.bmp");
            for (int i = 1; i <= 4; i++)
            {
                foreach (var letter in Config.TileRoad_Letters)
                {
                    var suffix = $"{i}{letter}";
                    LoadTile(suffix, $"tile_{suffix}.bmp");
                }
            }

            foreach (var letter in Config.TileResearch_Letters)
            {
                LoadTile(letter, $"tile_research_{letter}.bmp");
            }
        }

        void LoadTile(TileName name, string filename)
        {
            tiles[name] = new ImageProcessor(new Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
        void LoadTile(string name, string filename)
        {
            buy_tiles[name] = new ImageProcessor(new Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
    }
}
