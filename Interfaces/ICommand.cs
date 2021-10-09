using InfinitodeBot.Models;
using System.Threading.Tasks;

namespace InfinitodeBot.Interfaces
{
    internal interface ICommand
    {
        string Name { get; }
        Task Execute(GameClient game);
    }
}
