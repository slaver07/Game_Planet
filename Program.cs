using System;

namespace GamePlanet
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(1800, 1200))
            {
                game.Run();
            }
        }
    }
}

