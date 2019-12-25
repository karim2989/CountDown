using System;
using System.Linq;
using System.Collections.Generic;
using Calculator = CountDownCore.CountDownGame.Calculator;
using Operation = CountDownCore.CountDownGame.Operation;

namespace CountDownCore
{
    public class CountDownGameGenerator
    {
        private static Random random = new Random();
        public static CountDownGame GenerateGame(int startingNumbersCount, bool ignoreLimits = false)
        {
            int[] startingNumbers = new int[startingNumbersCount];
            int target;
            do
            {
                target = 0;
                for (int i = 0; i < startingNumbersCount; i++)
                {
                    int choosenNumber = randomNumber;
                    startingNumbers[i] = choosenNumber;
                    target = Calculator.Calculate(new Operation(target, randomOperator, choosenNumber));
                }
                if ((target > 100 && target < 1000) || ignoreLimits) break;
            } while (true);

            startingNumbers = shuffleArray<int>(startingNumbers);
            return new CountDownGame((startingNumbers, target));
        }
        private static int randomNumber => possibleNumbers[random.Next(0, possibleNumbers.Length)];
        private static readonly int[] possibleNumbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 25, 50, 75, 100 };
        private static char randomOperator => possibleOperators[random.Next(0, possibleOperators.Length)];
        private static readonly char[] possibleOperators = new char[] { '+', '-', '*', '/' };

        private static T[] shuffleArray<T>(T[] input)
        {
            return input.OrderBy(x => random.Next()).ToArray();
        }
    }
}