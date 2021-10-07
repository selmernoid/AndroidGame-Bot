using InfinitodeBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfinitodeBot.Interfaces
{
    internal interface ICommand
    {
        string Name { get; }
        Task Execute(GameClient game);
    }
}
