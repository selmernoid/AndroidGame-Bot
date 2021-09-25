using ConsoleApp1.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    internal class MapUpgradeCommand : ICommand
    {
        public string Name { get => "map-build"; }

        public Task Execute(GameClient game)
        {
            throw new NotImplementedException();
        }
    }
}
