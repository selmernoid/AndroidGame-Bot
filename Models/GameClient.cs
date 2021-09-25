using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    internal class GameClient
    {
        const int WIDTH = 1920;
        const int HEIGTH = 1080;
        protected static IDictionary<TileName, ImageProcessor> tiles = new Dictionary<TileName, ImageProcessor>();
        protected static IDictionary<string, ImageProcessor> buy_tiles = new Dictionary<string, ImageProcessor>();
        protected static string[] TileRoad_Letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "k" };
        protected static string[] TileResearch_Letters = new string[] { "a", "d", "x", "z" };
        static AdbClient client;

        const int TILESIZE_MIN = 27;
        static int[] BAG_TILE_X = new int[6] { 470, 600, 730, 860, 990, 1120 };
        static int[] BAG_TILE_Y = new int[4] { 260, 400, 540, 680 };



        static Point BAG_OPEN_BTN = new Point(1400, 900);
        static Point OK_BTN = new Point(1345, 670);

        static Point screen_OpenMin = new Point(1245, 875);
        static Point screen_OpenMax = new Point(1410, 925);
        static async Task OpenLootItem(byte x, byte y)
        {

            Click(BAG_TILE_X[x - 1], BAG_TILE_Y[y - 1]);

            var screenshotBagMenu = await GetScreenshot(client);
            while (tiles[TileName.ButtonOpen].TryFindPosition(screenshotBagMenu, screen_OpenMin, screen_OpenMax) != null)
            {
                // Select bulk action
                Swipe(BAG_OPEN_BTN, BAG_OPEN_BTN, 750);
                // Press OK and wait
                Click(OK_BTN, 1000);
                // Close dialog. Point could be changed
                Click(BAG_OPEN_BTN, 250);
                // Refresh the screenshot
                screenshotBagMenu = await GetScreenshot(client);
            }
        }

        static async Task OpenKeyItem(Point clickPlace)
        {
            var tile = tiles[TileName.KeyGrey];
            var minPoint = new Point(clickPlace.X - tile.width, clickPlace.Y - tile.height);
            var maxPoint = new Point(clickPlace.X + tile.width, clickPlace.Y + tile.height);
            var screenshotKeysMenu = await GetScreenshot(client);
            while (tile.TryFindPosition(screenshotKeysMenu, minPoint, maxPoint, 0) == null)
            {
                // Select bulk action
                Swipe(clickPlace, clickPlace, 750);
                // Press OK and wait
                Click(OK_BTN, 1000);
                // Close dialog. Point could be changed if needed
                Click(BAG_OPEN_BTN, 250);
                // Refresh the screenshot
                screenshotKeysMenu = await GetScreenshot(client);
            }
        }


        static void StartServer()
        {
            AdbServer server = new AdbServer();
            var result = server.StartServer(@"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe", restartServerIfNewer: false);
        }

        static AdbClient GetClient()
        {
            //var Sync = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device);
            var client = new AdbClient(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort), Factories.AdbSocketFactory);
            //            //client.EndPoint = new IPEndPoint(IPAddress.Loopback, 5585);
            client.Connect("localhost:5585");
            return client;
            //var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
            //monitor.DeviceConnected += OnDeviceConnected;
            //monitor.Start();
        }

        static void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            Console.WriteLine($"The device {e.Device.Name} has connected to this PC");
            //var receiver = new ConsoleOutputReceiver();

            ///AdbClient.Instance.ExecuteRemoteCommand("echo Hello, World", e.Device, receiver);
        }

        static IShellOutputReceiver output = new ConsoleOutputReceiver();
        static void Click(Point p, int delay = 100)
        {
            Click(p.X, p.Y);
            if (delay > 0)
                Thread.Sleep(delay);
        }
        static void Click(int x, int y)
        {
            client.ExecuteRemoteCommand($"input tap {x} {y}", client.GetDevices().LastOrDefault(), output);
            //Console.WriteLine("The device responded:");
            //Console.WriteLine(output.ToString());
        }

        static void Swipe(Point from, Point to, int delay = 150) => Swipe(from.X, from.Y, to.X, to.Y, delay);
        static void Swipe(int x, int y, int toX, int toY, int delay = 150)
        {
            client.ExecuteRemoteCommand($"input swipe {x} {y} {toX} {toY} {delay}", client.GetDevices().LastOrDefault(), output);
            //Console.WriteLine("The device responded:");
            //Console.WriteLine(output.ToString());
        }
        static void SaveImage()
        {
            GetScreenshot(client).ContinueWith(x =>
            {
                Console.WriteLine("Image received");
                x.Result.Save(@"S:\tmp\1.bmp");

            });
        }
        static Task<Image> GetScreenshot(AdbClient client)
        {
            return client.GetFrameBufferAsync(client.GetDevices().LastOrDefault(), new System.Threading.CancellationToken());
        }

        static void LoadTiles()
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
                foreach (var letter in TileRoad_Letters)
                {
                    var suffix = $"{i}{letter}";
                    LoadTile(suffix, $"tile_{suffix}.bmp");
                }
            }

            foreach (var letter in TileResearch_Letters)
            {
                LoadTile(letter, $"tile_research_{letter}.bmp");
            }
        }

        static void LoadTile(TileName name, string filename)
        {
            tiles[name] = new ImageProcessor(new Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
        static void LoadTile(string name, string filename)
        {
            buy_tiles[name] = new ImageProcessor(new Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
    }
}
