using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfinitodeBot.Models
{
    internal class Config
    {
        public const int WIDTH = 1920;
        public const int HEIGTH = 1080;

        protected IDictionary<TileName, ImageProcessor> tiles = new Dictionary<TileName, ImageProcessor>();
        protected IDictionary<string, ImageProcessor> buy_tiles = new Dictionary<string, ImageProcessor>();
        public static ReadOnlyCollection<string> TileRoad_Letters = new ReadOnlyCollection<string> (new List <string>{ "a", "b", "c", "d", "e", "f", "g", "h", "i", "k" });
        public static ReadOnlyCollection<string> TileResearch_Letters = new ReadOnlyCollection<string>(new List<string> { "a", "d", "x", "z" });

        const int TILESIZE_MIN = 27;
        static int[] BAG_TILE_X = new int[6] { 470, 600, 730, 860, 990, 1120 };
        static int[] BAG_TILE_Y = new int[4] { 260, 400, 540, 680 };



        static Point BAG_OPEN_BTN = new Point(1400, 900);
        static Point OK_BTN = new Point(1345, 670);

        static Point screen_OpenMin = new Point(1245, 875);
        static Point screen_OpenMax = new Point(1410, 925);

    }
}
