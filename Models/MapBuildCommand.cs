using InfinitodeBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinitodeBot.Models
{
    internal class MapBuildCommand : ICommand
    {
        public string Name => "map-build";

        const bool WHOLE_GRID = true;
        
        const int TILESIZE_MIN = 27; // Size of the cell in px.

        // Coordinates of top right menu menu where u choose item to put on the map
        const int X_Left = 1500, X_Middle = 1600, X_Right = 1700;
        const int Y_First = 160, Y_Second = 330, Y_Third = 480;

        Point? barrierPoint = new Point(X_Right, Y_Second);

        public async Task Execute(GameClient game)
        {
                /*
                BEFORE START:
                    1. Open Map to edit
                    2. Zoom out on maximum scale
                    3. Set the most top left pixel in the frame
                    4. Choose mode "click-to-place" in game
                    5. left 3*3 tiles hovered by game info
                */

                Point TOP_LEFT = new Point(315, 119); // Info button crosses
                var screenshot = await game.GetScreenshot();
                TOP_LEFT = game.tiles[TileName.MapEditor_Corner_TopLeft].TryFindPosition(screenshot, tolerancePercent: 1).Value;

                if (WHOLE_GRID)
                {
                    /// The whole grid:
                    // Road tiles
                    PutSelectedTiles(game, TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32, 32);
                    //PutSelectedTiles(TOP_LEFT.X + TILESIZE_MIN / 2 + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 - 3, 3);

                    // Barriers & TPs
                    PutSelectedTiles(game, TOP_LEFT.X, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32 + 1, 32, barrierPoint);
                    PutSelectedTiles(game, TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y, TILESIZE_MIN, 32, 32 + 1, barrierPoint);

                }
                else
                {
                    // Road tiles
                    PutSelectedTiles(game, TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32, 32 - 3);
                    PutSelectedTiles(game, TOP_LEFT.X + TILESIZE_MIN / 2 + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 - 3, 3);

                    // PutSelectedTiles(370, 250, 60, 32, 3);
                    // PutSelectedTiles(250, 400, 27, 32, 1);


                    // PutSelectedTiles(370, 250, 60, 32, 3);
                    // PutSelectedTiles(250, 400, 27, 32, 1);

                    // Barriers & TPs

                    PutSelectedTiles(game, TOP_LEFT.X, TOP_LEFT.Y + TILESIZE_MIN / 2, TILESIZE_MIN, 32 + 1, 32 - 3, barrierPoint);
                    PutSelectedTiles(game, TOP_LEFT.X + 3 * TILESIZE_MIN, TOP_LEFT.Y + TILESIZE_MIN / 2 + TILESIZE_MIN * (32 - 3), TILESIZE_MIN, 32 + 1 - 3, 3, barrierPoint);

                    PutSelectedTiles(game, TOP_LEFT.X + TILESIZE_MIN / 2, TOP_LEFT.Y, TILESIZE_MIN, 32, 32 + 1 - 3, barrierPoint);
                    PutSelectedTiles(game, TOP_LEFT.X + 3 * TILESIZE_MIN + TILESIZE_MIN / 2, TOP_LEFT.Y + TILESIZE_MIN * (32 + 1 - 3), TILESIZE_MIN, 32 - 3, 3, barrierPoint);
                    //*/
                }
        }

        static void PutSelectedTiles(GameClient game, int x, int y, int tileSize, int numX, int numY, Point? clickBeforeEach = null)
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
                    game.Swipe(x, y, x, y - tileSize, swipe_delay);
                    bottomSwipes++;
                    requiredSwipeToBottom = false;
                }

                for (int i = 0; i < numX; i++)
                {
                    if (clickBeforeEach != null)
                        game.Click(clickBeforeEach.Value);
                    game.Click(posClickX, posClickY);
                    posClickX += tileSize;
                    if (posClickX > rightBarX)
                    {
                        game.Swipe(rightBarX, posClickY, rightBarX - swipeSizeX * tileSize, posClickY, swipe_delay);
                        posClickX -= swipeSizeX * tileSize;
                        rightSwipes++;
                    }
                }

                posClickX -= (numX - swipeSizeX * rightSwipes) * tileSize;
                for (int i = 0; i < rightSwipes; i++)
                {
                    game.Swipe(rightBarX - swipeSizeX * tileSize, posClickY, rightBarX, posClickY, swipe_delay);
                }
                rightSwipes = 0;
                if (posClickY + tileSize < bottomLineY)
                {
                    posClickY += tileSize;
                }
                else
                    requiredSwipeToBottom = true;
            }
            for (int i = 0; i < bottomSwipes; i++)
            {
                game.Swipe(x, y - tileSize, x, y, swipe_delay);
            }
        }
    }
}
