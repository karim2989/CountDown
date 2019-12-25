using System;
using System.Linq;
using System.Collections.Generic;

namespace CountDownCore
{
    public class CountDownGame
    {
        private (int[] givenNumbers, int target) startingVariables;
        public (int[] givenNumbers, int target) StartingVariables => startingVariables;
        public CountDownGame((int[] givenNumbers, int target) startingVariables)
        {
            this.startingVariables = startingVariables;
        }
        #region Analysing the response
        //expeted input is somthing like : "1+5;3*6;18-2;16*9"
        public ResponseAnalysisResult AnalyseResponse(string response)
        {
            var output = new ResponseAnalysisResult();
            //checking for illegal characters 
            if (ContainesIllegalCharacters(response)) output.Error = "Illegal characters found in the input.";

            //extracting operations
            Queue<Operation> operations = new Queue<Operation>();
            try
            {
                foreach (string s in response.Split(';'))
                {
                    if (s.Length < 3) continue;

                    operations.Enqueue(Operation.FromString(s, output));
                }
                output.Operations = operations.ToArray();
            }
            catch
            {
                output.Error = "failed parsing";
                return output;
            }

            //executing operations
            //note : operation only effect currentlyAvailableNumbers
            List<int> currentlyAvailableNumbers = new List<int>(startingVariables.givenNumbers);
            while (operations.Count > 0)
            {
                var op = operations.Dequeue();
                if (currentlyAvailableNumbers.Contains(op.o1) && currentlyAvailableNumbers.Contains(op.o2))
                {
                    currentlyAvailableNumbers.Add(Calculator.Calculate(op));//Calculator.Calculate(op) == op.Result
                    currentlyAvailableNumbers.Remove(op.o1);
                    currentlyAvailableNumbers.Remove(op.o2);
                }
                else
                {
                    //if the player uses a non existante number, he loses immediately
                    output.Error = $"an inavailable number was used {op.o1} {op.o2}";
                    return output;
                }
            }

            //analysing the result:
            bool targetReached = currentlyAvailableNumbers.Contains(startingVariables.target);

            if (targetReached) { output.Score = 1; return output; }//perfect score
            else { output.Score = calculateNonPerfectResult(currentlyAvailableNumbers.Max(), startingVariables.target); return output; }


            //local functions :
            float calculateNonPerfectResult(int numberReached, int target)
            {
                int difference = Math.Abs(target - numberReached);
                return (target - difference) / (float)target;
            }
        }
        public static bool ContainesIllegalCharacters(string input)
        {
            if (input.Length == 0) return true;
            char[] acceptedCharacters = { '+', '*', '-', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ';' };
            foreach (char c in input)
            {
                if (!acceptedCharacters.Contains(c)) return true;
            }
            return false;
        }
        public class ResponseAnalysisResult
        {
            public Operation[] Operations;
            public float Score { get; set; }
            public bool PerfectScore => Score == 1;
            public string Error { get; set; }
            public bool HasError => Error != default(string);
            public override string ToString() => HasError ? $"Error: {Error}" : PerfectScore ? "Perfect !!!" : $"You Scored {Math.Round(Score * 10, 1)}/10";
        }
        public static class Calculator
        {
            public static int Calculate(Operation input)
            {
                Func<int, int, int> functionUsed = getRelevantFunction(input.operatorUsed);
                return functionUsed(input.o1, input.o2);
            }
            private static Func<int, int, int> getRelevantFunction(char c)
            {
                switch (c) //note : i tried using a dictionary instead of a switch statement but it didnt work
                {
                    case '+': return addOperation;
                    case '-': return subOperation;
                    case '*': return mulOperation;
                    case '/': return divOperation;
                    default: throw new Exception();
                }
            }
            private static Func<int, int, int> addOperation = (v1, v2) => v1 + v2;
            private static Func<int, int, int> subOperation = (v1, v2) => v1 - v2;
            private static Func<int, int, int> mulOperation = (v1, v2) => v1 * v2;
            private static Func<int, int, int> divOperation = (v1, v2) => v1 / v2;
        }
        public struct Operation
        {
            public Operation(int o1, char c, int o2)
            {
                this.o1 = o1;
                this.operatorUsed = c;
                this.o2 = o2;
            }
            public static Operation FromString(string s, ResponseAnalysisResult output = null)//output is used for error hadeling
            {
                string[] numbersUsed = s.Split(new char[] { '+', '-', '/', '*' });
                if (numbersUsed.Length != 2) output.Error = "At least one operation is corrupt.";

                var operation = new Operation();
                operation.o1 = int.Parse(numbersUsed[0]);
                operation.o2 = int.Parse(numbersUsed[1]);
                operation.operatorUsed = s[numbersUsed[0].Length];
                return operation;
            }
            public int o1 { get; set; }
            public char operatorUsed { get; set; }
            public int o2 { get; set; }
            public int Result => Calculator.Calculate(this);
            public override string ToString() => $"{o1} {operatorUsed} {o2} = {Result}";
        }
        #endregion
    }
}
