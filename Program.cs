using InfinitodeBot.Interfaces;
using InfinitodeBot.Models;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InfinitodeBot
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
        public const int WIDTH = 1920;
        public const int HEIGTH = 1080;
        protected static IDictionary<TileName, ImageProcessor> tiles = new Dictionary<TileName, ImageProcessor>();
        protected static IDictionary<string, ImageProcessor> buy_tiles = new Dictionary<string, ImageProcessor>();
        protected static string[] TileRoad_Letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "k" };
        protected static string[] TileResearch_Letters = new string[] { "a", "d", "x", "z" };
        static AdbClient client;

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

            var allowedCommands = new List<ICommand>{
                new UpgradeTilesCommand(),
                new MapBuildCommand(),
            };

            stopwatch.Start();

            var game = new GameClient();
            ICommand command;

            var mode = "upgrade";
            mode = "map-build";
            //mode = "open-boxes";

            command = allowedCommands.FirstOrDefault(x=> x.Name == mode);
            if (command == null)
                throw new NotSupportedException($"Unrecognized mode: {mode}");

            await command.Execute(game);
                
            if (mode == "open-boxes")
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


       
    }

}
