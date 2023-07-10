using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace revelationStateMachine
{
    public sealed class ExitQuestionFunction : FunctionDefinition
    {

        public ExitQuestionFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            Name = "ExitQuestion";

            Func<int> ExitQuestion = () =>
            {

                Console.WriteLine("Would you like to exit?");
                string? answer = Console.ReadLine();

                if (answer == null || answer.Trim() == "")
                    return 0;

                // Console.WriteLine("You typed in " + number);

                if (answer.Trim().ToLower() == "yes") // exit the program
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            };

            Function = ExitQuestion;
        }
    }
}