using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Interfaces
{
    internal interface ICommand
    {
        string Name { get; }
        Task Execute(GameClient game);
    }
}
