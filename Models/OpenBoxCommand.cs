using InfinitodeBot.Interfaces;
using System.Drawing;
using System.Threading.Tasks;

namespace InfinitodeBot.Models
{
    internal class OpenBoxCommand : ICommand
    {
        public string Name => "open-boxes";

        public async Task Execute(GameClient game)
        {

            var plus_top_right = new Point(1675, 40);
            var bag_top_left = new Point(55, 300);

            // Boxes
            // Open Green box
            await OpenLootItem(game, 4, 4);
            // Open Blue box
            await OpenLootItem(game, 3, 4);
            // Open Purple box
            await OpenLootItem(game, 4, 3);
            // Open Orange box
            await OpenLootItem(game, 1, 3);
            // Open Red box
            await OpenLootItem(game, 5, 2);
            // Open Ocean box
            await OpenLootItem(game, 2, 1);

            // Keys
            // Open Menu
            game.Click(plus_top_right, 400);
            game.Click(plus_top_right, 400);
            game.Swipe(Config.WIDTH / 2, Config.HEIGTH / 2, Config.WIDTH / 2, Config.HEIGTH / 2 - 400, 2000);
            // Open Blue keys
            await OpenKeyItem(game, new Point(845, 665));
            // Open Purple keys
            await OpenKeyItem(game, new Point(1450, 665));
            // Open Orange keys
            await OpenKeyItem(game, new Point(845, 850));
            // Open Ocean keys
            await OpenKeyItem(game, new Point(1450, 850));

            // Random Tiles
            // Open Bag menu
            game.Click(bag_top_left, 400);
            for (byte i = 1; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    await OpenLootItem(game, i, 1);
                }
            }
        }



        static Point BAG_OPEN_BTN = new Point(1400, 900);
        static Point OK_BTN = new Point(1345, 670);

        static Point screen_OpenMin = new Point(1245, 875);
        static Point screen_OpenMax = new Point(1410, 925);
        static async Task OpenLootItem(GameClient game, byte x, byte y)
        {

            game.Click(Config.BAG_TILE_X[x - 1], Config.BAG_TILE_Y[y - 1]);

            var screenshotBagMenu = await game.GetScreenshot();
            while (game.tiles[TileName.ButtonOpen].TryFindPosition(screenshotBagMenu, screen_OpenMin, screen_OpenMax) != null)
            {
                // Select bulk action
                game.Swipe(BAG_OPEN_BTN, BAG_OPEN_BTN, 750);
                // Press OK and wait
                game.Click(OK_BTN, 1000);
                // Close dialog. Point could be changed
                game.Click(BAG_OPEN_BTN, 250);
                // Refresh the screenshot
                screenshotBagMenu = await game.GetScreenshot();
            }
        }

        static async Task OpenKeyItem(GameClient game, Point clickPlace)
        {
            var tile = game.tiles[TileName.KeyGrey];
            var minPoint = new Point(clickPlace.X - tile.width, clickPlace.Y - tile.height);
            var maxPoint = new Point(clickPlace.X + tile.width, clickPlace.Y + tile.height);
            var screenshotKeysMenu = await game.GetScreenshot();
            while (tile.TryFindPosition(screenshotKeysMenu, minPoint, maxPoint, 0) == null)
            {
                // Select bulk action
                game.Swipe(clickPlace, clickPlace, 750);
                // Press OK and wait
                game.Click(OK_BTN, 1000);
                // Close dialog. Point could be changed if needed
                game.Click(BAG_OPEN_BTN, 250);
                // Refresh the screenshot
                screenshotKeysMenu = await game.GetScreenshot();
            }
        }

    }
}
