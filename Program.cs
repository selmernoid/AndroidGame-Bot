using InfinitodeBot.Interfaces;
using InfinitodeBot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InfinitodeBot
{
    class Program
    {
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
                new OpenBoxCommand(),
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
                
            stopwatch.Stop();
            Console.WriteLine($"Finished in {stopwatch.Elapsed.ToString()}");
            Console.ReadKey();
            return;
        }       
    }
}
