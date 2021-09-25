using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
