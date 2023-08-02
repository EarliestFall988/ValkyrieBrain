
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace revelationStateMachine
{
    public sealed class GuessFunction : FunctionDefinition
    {

        public GuessFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            // ExpectedParameters = new List<StateMachineVariableType>()
            // {
            //     StateMachineVariableType.Text, // name
            //     StateMachineVariableType.Integer // guess
            // };

            ExpectedParameters = new Dictionary<string, (StateMachineVariableType type, bool applied)>()
            {
                { "name", (StateMachineVariableType.Text, false) },
                { "guess", (StateMachineVariableType.Integer, false) }
            };

            Name = "Guess";

            Function = () =>
            {

                string name = Parameters["name"].GetText();
                int guessNumber = Parameters["guess"].GetInt();

                Console.WriteLine($"Okay {name}, Guess a number");
                string? number = Console.ReadLine();

                if (number == null || number.Trim() == "")
                    return 0;


                if (number.ToLower().Trim() == "exit") // exit the program
                {
                    return -1;
                }

                bool result = Int32.TryParse(number, out int numberInt);

                if (!result)
                {
                    Console.WriteLine("That's not a number! Try again!");
                    return 0;
                }


                if (guessNumber == numberInt)
                {
                    Console.WriteLine("Correct.");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Incorrect. Try Again");
                    return 0;
                }
            };
        }
    }
}