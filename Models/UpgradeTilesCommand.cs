using InfinitodeBot.Interfaces;
using System.Drawing;
using System.Threading.Tasks;

namespace InfinitodeBot.Models
{
    internal class UpgradeTilesCommand : ICommand
    {
        public string Name => "upgrade";


        static Point btnBuy = new Point(1322, 900);
        static Point btnStackMax = new Point(1315, 775); // Select max items to upgrade
        static Point btnOk = new Point(1154, 911);
        static Point btnCancel = new Point(1300, 911);
        
        // Area of middle menu. Reduce search area
        static Point screen_SearchMin = new Point(360, 120);
        static Point screen_SearchMax = new Point(1200, 960);
        
        // Upgrade menu -> Right bottom button should be grey when its impossible
        static Point screen_OkMin = new Point(1080, 860);
        static Point screen_OkMax = new Point(1220, 970);

        public async Task Execute(GameClient game)
        {


            int researchClicks = 20;
            int roadTileClicks = 40;
            // Swipe to bottom
            game.Swipe(GameClient.WIDTH / 2, GameClient.HEIGTH * 8 / 10, GameClient.WIDTH / 2, 10);
            game.Swipe(GameClient.WIDTH / 2, GameClient.HEIGTH * 8 / 10, GameClient.WIDTH / 2, 10);

            var screenshot = await game.GetScreenshot();
            // Buy researches
            foreach (var letter in Config.TileResearch_Letters)
            {
                await BuyItem(letter, researchClicks);
            }
            // Buy 1l tiles
            foreach (var letter in Config.TileRoad_Letters)
            {
                await BuyItem($"1{letter}", roadTileClicks);
            }
            // Swipe higher
            game.Swipe(600, 165, 600, 875, 5000);
            // Buy 2l - 4l tiles
            for (int i = 2; i <= 4; i++)
            {
                foreach (var letter in Config.TileRoad_Letters)
                {
                    await BuyItem($"{i}{letter}", roadTileClicks);
                }
            }

            async Task BuyItem(string key, int clicks)
            {
                var itemTile = game.buy_tiles[key].TryFindPosition(screenshot, screen_SearchMin, screen_SearchMax);
                if (!itemTile.HasValue)
                    return;
                game.Click(itemTile.Value);
                for (int i = 0; i < clicks; i++)
                {
                    game.Click(btnBuy, 250);
                    //var screenshotTask = GetScreenshot(client);
                    //screenshotTask.Start();
                    game.Click(btnStackMax);
                    var screenshotBuyMenu = await game.GetScreenshot();
                    var greyOk = game.tiles[TileName.ButtonGreyOk].TryFindPosition(screenshotBuyMenu, screen_OkMin, screen_OkMax);
                    if (greyOk == null)
                        game.Click(btnOk, 700);
                    else
                    {
                        game.Click(btnCancel, 700);
                        return;
                    }
                }
            }
        }
    }
}
