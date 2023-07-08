using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public class Functions
    {
        public Dictionary<string, Func<int>> functions = new Dictionary<string, Func<int>>();

        public Functions(StateMachine stateMachine)
        {
            Func<string, bool> Exit = (x) =>
            {
                if (x.ToLower() == "exit")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            Func<int> CheckCondition = () =>
            {

                Console.WriteLine("Type in a number");
                string? number = Console.ReadLine();

                if (number == null || number.Trim() == "")
                    return 0;

                Console.WriteLine("You typed in " + number);

                if (Exit(number)) // exit the program
                {
                    return -1;
                }

                Console.WriteLine("Checking Data @ 1,1");
                bool result = stateMachine.ReadKey(1, 1, out var str);
                if (result && str == number)
                {
                    Console.WriteLine("Correct.");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Incorrect.");
                    return 0;
                }
            };

            Func<int> Next = () => 0;

            functions.Add("CheckCondition", CheckCondition);
            functions.Add("Next", Next);
        }
    }
}