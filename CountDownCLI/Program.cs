using System;
using CountDownCore;

namespace CountDownCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var game = new CountDownGame((new int[]{5,2,3,6},36));
            System.Console.WriteLine(game.AnalyseResponse("5*2;10*3;30+6"));*/
            var game =  CountDownGameGenerator.GenerateGame(5);
            var x = game.StartingVariables;
            foreach (int y in x.givenNumbers)
            {
                System.Console.WriteLine(y);
            }
            System.Console.WriteLine($"\n{x.target}");
            System.Console.WriteLine(game.AnalyseResponse(Console.ReadLine()));
        }
    }
}
