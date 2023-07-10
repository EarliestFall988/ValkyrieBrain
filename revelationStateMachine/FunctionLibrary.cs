using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace revelationStateMachine
{
    public class FunctionLibrary
    {
        public Dictionary<string, FunctionDefinition> ImportedFunctions = new Dictionary<string, FunctionDefinition>();

        public FunctionLibrary()
        {

            ImportFunctions();
            // Func<string, bool> Exit = (x) =>
            // {
            //     if (x.ToLower() == "exit")
            //     {
            //         return true;
            //     }
            //     else
            //     {
            //         return false;
            //     }
            // };

            // Func<int> Guess = () =>
            // {

            //     bool res = stateMachine.ReadKey(0, 0, out var name);

            //     if (!res)
            //     {
            //         Console.WriteLine("Error reading name.");
            //         return -1;
            //     }

            //     Console.WriteLine($"Okay {name}, Guess in a number");
            //     string? number = Console.ReadLine();

            //     if (number == null || number.Trim() == "")
            //         return 0;

            //     // Console.WriteLine("You typed in " + number);

            //     if (Exit(number)) // exit the program
            //     {
            //         return -1;
            //     }

            //     // Console.WriteLine("Checking Data @ 1,1");
            //     bool result = stateMachine.ReadKey(1, 1, out var str);
            //     if (result && str == number)
            //     {
            //         Console.WriteLine("Correct.");
            //         return 1;
            //     }
            //     else
            //     {
            //         Console.WriteLine("Incorrect. Try Again");
            //         return 0;
            //     }
            // };

            // Func<int> ExitQuestion = () =>
            // {

            //     Console.WriteLine("Would you like to exit?");
            //     string? answer = Console.ReadLine();

            //     if (answer == null || answer.Trim() == "")
            //         return 0;

            //     // Console.WriteLine("You typed in " + number);

            //     if (answer.Trim().ToLower() == "yes") // exit the program
            //     {
            //         return 1;
            //     }
            //     else
            //     {
            //         return 0;
            //     }
            // };

            // Func<int> GreetUser = () =>
            // {

            //     string? result = "";
            //     Console.WriteLine("Hello!");

            //     while (result == null || result == string.Empty)
            //     {
            //         Console.WriteLine("What is your Name?");
            //         result = Console.ReadLine();

            //         if (result == null || result == string.Empty)
            //         {
            //             Console.WriteLine("That's not your name!");
            //         }

            //         if (Exit(result ?? ""))
            //         {
            //             return -1;
            //         }

            //         bool success = stateMachine.WriteValue(0, 0, result?.Replace(',', '\'') ?? "");

            //         if (!success)
            //         {
            //             Console.WriteLine("could not write to store");
            //             return -1;
            //         }
            //     }

            //     return 1;
            // };

            // Func<int> Reset = () => {

            //     // var Random random = new Random();

            //     bool success = stateMachine.WriteValue(1, 1, "0");

            //     if (!success)
            //     {
            //         Console.WriteLine("could not write to store");
            //         return -1;
            //     }

            //     return 1;
            // }

            // Func<int> Next = () => 0;
            // Func<int> ExitProgram = () => 0;

            // functions.Add("Guess", Guess); // returns 1 if correct, zero if not 
            // functions.Add("Next", Next); //simple connect to next state
            // functions.Add("Exit", ExitProgram); //exit the program
            // functions.Add("ExitQuestion", ExitQuestion); //exit the program question
            // functions.Add("GreetUser", GreetUser); // greet the user at the beginning of the interaction
        }

        public void ImportFunctions()
        {
            Console.WriteLine("\t>Gathering Functions...\n");
            ContinueFunction continueFunction = new ContinueFunction();
            ExitQuestionFunction exitQuestion = new ExitQuestionFunction();
            GreetUserFunction greetFunction = new GreetUserFunction();
            GuessFunction guessFunction = new GuessFunction();
            SetRandomNumberFunction randomFunction = new SetRandomNumberFunction();


            ImportedFunctions.Add(continueFunction.Name, continueFunction);
            ImportedFunctions.Add(exitQuestion.Name, exitQuestion);
            ImportedFunctions.Add(greetFunction.Name, greetFunction);
            ImportedFunctions.Add(guessFunction.Name, guessFunction);
            ImportedFunctions.Add(randomFunction.Name, randomFunction);
        }

        public bool TryGetFunction(string name, out FunctionDefinition? function)
        {

            FunctionDefinition? func = null;

            bool res = ImportedFunctions.TryGetValue(name, out func);

            function = func;

            return res;
        }
    }
}