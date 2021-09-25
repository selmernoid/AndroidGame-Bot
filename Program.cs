using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public enum TileName
    {
        Plus_TopLeft,
        MapEditor_Corner_TopLeft,
        Bag_Left,
        Stack50,
        ButtonOpen,
        ButtonOk,
        ButtonGreyOk,
        ButtonBuy,
        KeyGrey,
    }
    class Program
    {
        const int WIDTH = 1920;
        const int HEIGTH = 1080;
        protected static IDictionary<TileName, ImageProcessor> tiles = new Dictionary<TileName, ImageProcessor>();
        protected static IDictionary<string, ImageProcessor> buy_tiles = new Dictionary<string, ImageProcessor>();
        protected static string[] TileRoad_Letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "k" };
        protected static string[] TileResearch_Letters = new string[] { "a", "d", "x", "z" };
        static AdbClient client;

        const int TILESIZE_MIN = 27;
        static int[] BAG_TILE_X = new int[6] { 470, 600, 730, 860, 990, 1120};
        static int[] BAG_TILE_Y = new int[4] { 260, 400, 540, 680 };
        static async Task Main(string[] args)
        {
            /*
             * Before start:
             *      1. Run emulator
             *      2. Execute command: "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe" connect localhost:5585
            */
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            StartServer();
            client = GetClient();
            // Init images
            LoadTiles();

            var mode = "upgrade";
            mode = "map-build";
            //mode = "open-boxes";

            if (mode == "upgrade")
            {
                await UpgradeTiles();
            }
            else if (mode == "map-build")
            {
                var WHOLE_GRID = true;
                int X_Left = 1500, X_Middle = 1600, X_Right = 1700;
                int Y_First = 160, Y_Second = 330, Y_Third = 480;
                Point? barrierPoint = new Point(X_Right, Y_Second);
                /*
                BEFORE START:
                    1. Open Map to edit
                    2. Zoom out on maximum scale
                    3. Set the most top left pixel in the frame
                    4. Choose mode "click-to-place" in game
                    5. left 3*3 tiles hovered by game info
                */

                Point TOP_LEFT = new Point(315, 119); // Info button crosses
                var screenshot = await GetScreenshot(client);
                TOP_LEFT = tiles[TileName.MapEditor_Corner_TopLeft].TryFindPosition(screenshot, tolerancePercent: 1).Value;

                if (WHOLE_GRID)
                {
                    /// The whole grid:
                    // Road tiles
                    PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32, 32);
                    //PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2 + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 - 3, 3);

                    // Barriers & TPs
                    PutSelectedTiles(TOP_LEFT.X, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32 + 1, 32, barrierPoint);
                    PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y, TILESIZE_MIN, 32, 32 + 1, barrierPoint);

                }
                else
                {
                    // Road tiles
                    PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32, 32 - 3);
                    PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2 + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 - 3, 3);

                    // PutSelectedTiles(370, 250, 60, 32, 3);
                    // PutSelectedTiles(250, 400, 27, 32, 1);


                    // PutSelectedTiles(370, 250, 60, 32, 3);
                    // PutSelectedTiles(250, 400, 27, 32, 1);

                    // Barriers & TPs

                    PutSelectedTiles(TOP_LEFT.X, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32 + 1, 32 - 3, barrierPoint);
                    PutSelectedTiles(TOP_LEFT.X + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 + 1 - 3, 3, barrierPoint);

                    PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y, TILESIZE_MIN, 32, 32 + 1 - 3, barrierPoint);
                    PutSelectedTiles(TOP_LEFT.X + 3 * TILESIZE_MIN + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN * (32 + 1 - 3), TILESIZE_MIN, 32 - 3, 3, barrierPoint); 
                                                                                                                                                       //*/
                }
            } else if (mode == "open-boxes")
            {
                var plus_top_right = new Point(1675, 40);
                var bag_top_left = new Point(55, 300);

                // Boxes
                // Open Green box
                await OpenLootItem(4, 4);
                // Open Blue box
                await OpenLootItem(3, 4);
                // Open Purple box
                await OpenLootItem(4, 3);
                // Open Orange box
                await OpenLootItem(1, 3);
                // Open Red box
                await OpenLootItem(5, 2);
                // Open Ocean box
                await OpenLootItem(2, 1);

                // Keys
                // Open Menu
                Click(plus_top_right, 400);
                Click(plus_top_right, 400);
                Swipe(WIDTH/2, HEIGTH/2, WIDTH/2, HEIGTH/2 - 400, 2000);
                // Open Blue keys
                await OpenKeyItem(new Point(845 ,665));
                // Open Purple keys
                await OpenKeyItem(new Point(1450, 665));
                // Open Orange keys
                await OpenKeyItem(new Point(845 , 850));
                // Open Ocean keys
                await OpenKeyItem(new Point(1450, 850));

                // Random Tiles
                // Open Bag menu
                Click(bag_top_left, 400);
                for (byte i = 1; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        await OpenLootItem(i, 1);
                    }
                }


            }

            stopwatch.Stop();
            Console.WriteLine($"Finished in {stopwatch.Elapsed.ToString()}");
            Console.ReadKey();
            return;
            /*
            var screenshot = await GetScreenshot(client);
            //GetScreenshot(client).GetAwaiter().GetResult(); .ContinueWith(x =>
            {
                var start = new Stopwatch();
                start.Start();
                var position = tiles[TileName.Plus_TopLeft].FindPosition(screenshot);
                start.Stop();
                Console.WriteLine($"{position.X} {position.Y} {start.Elapsed}");
            }
            */

            //SaveImage(client);
            //Click(client, (int)(WIDTH * 0.9), (int)(HEIGTH * 0.8)); // New Game
            //SaveImage(client);
        }

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


        static async Task UpgradeTiles()
        {
            Point btnBuy = new Point(1322, 900);
            Point btnStackMax = new Point(1315,775);
            Point btnOk = new Point(1154,911);
            Point btnCancel = new Point(1300,911);

            Point screen_SearchMin = new Point(360, 120);
            Point screen_SearchMax = new Point(1200, 960);

            Point screen_OkMin = new Point(1080, 860);
            Point screen_OkMax = new Point(1220, 970);

            int researchClicks = 20;
            int roadTileClicks = 40;
            // Swipe to bottom
            Swipe(WIDTH / 2, HEIGTH * 8 / 10, WIDTH / 2, 10);
            Swipe(WIDTH / 2, HEIGTH * 8 / 10, WIDTH / 2, 10);

            #region Get Btn Coordinates
            /*
            var screenshot = await GetScreenshot(client);
            Click(buy_tiles["a"].FindPosition(screenshot));

            var screenshotBuy = await GetScreenshot(client);
            btnBuy = tiles[TileName.ButtonBuy].FindPosition(screenshotBuy);
            Click(btnBuy);

            var screenshotMenu = await GetScreenshot(client);
            btnOk = tiles[TileName.ButtonOk].FindPosition(screenshotMenu);
            btnStackMax = tiles[TileName.Stack50].FindPosition(screenshotMenu);
            Click(btnStackMax);
            Click(btnOk);
            */
            #endregion
            var screenshot = await GetScreenshot(client);
            // Buy researches
            foreach (var letter in TileResearch_Letters)
            {
                await BuyItem(letter, researchClicks);
            }
            // Buy 1l tiles
            foreach (var letter in TileRoad_Letters)
            {
                await BuyItem($"1{letter}", roadTileClicks);
            }
            // Swipe higher
            Swipe(600, 165, 600, 875, 5000);
            // Buy 2l - 4l tiles
            for (int i = 2; i <= 4; i++)
            {
                foreach (var letter in TileRoad_Letters)
                {
                    await BuyItem($"{i}{letter}", roadTileClicks);
                }
            }

            async Task BuyItem(string key, int clicks) {
                var itemTile = buy_tiles[key].TryFindPosition(screenshot, screen_SearchMin, screen_SearchMax);
                if (!itemTile.HasValue)
                    return;
                Click(itemTile.Value);
                for (int i = 0; i < clicks; i++)
                {
                    Click(btnBuy, 250);
                    //var screenshotTask = GetScreenshot(client);
                    //screenshotTask.Start();
                    Click(btnStackMax);
                    var screenshotBuyMenu = await GetScreenshot(client);
                    var greyOk = tiles[TileName.ButtonGreyOk].TryFindPosition(screenshotBuyMenu, screen_OkMin, screen_OkMax);
                    if (greyOk == null)
                        Click(btnOk, 700);
                    else {
                        Click(btnCancel, 700);
                        return;
                    }
                }
            }
        }

        static void PutSelectedTiles(int x, int y, int tileSize, int numX, int numY, Point? clickBeforeEach = null)
        {
            var rightBarX = 1200;
            var bottomLineY = 945; // 1050;// 800;
            var swipeSizeX = 5;

            var posClickX = x;
            var posClickY = y;
            var rightSwipes = 0;
            var bottomSwipes = 0;
            var requiredSwipeToBottom = false;

            var swipe_delay = 1000;

            for (int j = 0; j < numY; j++)
            {
                if (requiredSwipeToBottom)
                {
                    Swipe(x, y, x, y - tileSize, swipe_delay);
                    bottomSwipes++;
                    requiredSwipeToBottom = false;
                }

                for (int i = 0; i < numX; i++)
                {
                    if (clickBeforeEach != null)
                        Click(clickBeforeEach.Value);
                    Click(posClickX, posClickY);
                    posClickX += tileSize;
                    if (posClickX > rightBarX)
                    {
                        Swipe(rightBarX, posClickY, rightBarX - swipeSizeX * tileSize, posClickY, swipe_delay);
                        posClickX -= swipeSizeX * tileSize;
                        rightSwipes++;
                    }
                }

                posClickX -= (numX - swipeSizeX * rightSwipes) * tileSize;
                for (int i = 0; i < rightSwipes; i++)
                {
                    Swipe(rightBarX - swipeSizeX * tileSize, posClickY, rightBarX, posClickY, swipe_delay);
                }
                rightSwipes = 0;
                if (posClickY + tileSize < bottomLineY)
                {
                    posClickY += tileSize;
                } else
                    requiredSwipeToBottom = true;
            }
            for (int i = 0; i < bottomSwipes; i++)
            {
                Swipe(x, y - tileSize, x, y, swipe_delay);
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
                foreach (var letter in TileRoad_Letters) {
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
            tiles[name] = new ImageProcessor(new System.Drawing.Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
        static void LoadTile(string name, string filename)
        {
            buy_tiles[name] = new ImageProcessor(new System.Drawing.Bitmap(@"S:\tmp\infinitode-imgs\" + filename));
        }
    }

}
