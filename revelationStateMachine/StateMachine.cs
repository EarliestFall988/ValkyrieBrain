using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public class StateMachine
    {
        /// <summary>
        /// CSV data that the state machine is operating on.
        /// </summary>
        /// <value></value>
        public string Store { get; set; } = "";

        /// <summary>
        /// The list of states that this machine can be in.
        /// </summary>
        /// <typeparam name="State"></typeparam>
        /// <returns></returns>
        public List<State> States = new List<State>();

        /// <summary>
        /// The current state of the machine.
        /// </summary>
        /// <value></value>
        public State? CurrentState { get; set; }

        /// <summary>
        /// The state that the machine will fall back to if it cannot transition to the next state, or can be used as a 'throw error' state
        /// </summary>
        /// <value></value>
        public State? FallbackState { get; set; }

        /// <summary>
        /// The Initial State
        /// </summary>
        /// <value></value>
        public State? InitialState { get; set; }

        /// <summary>
        /// Creates a new state machine with the given initial state and fallback state.
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="fallbackState"></param>
        public StateMachine()
        {

        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state"></param>
        public void AddState(State state)
        {
            States.Add(state);
        }

        public void RunStateMachine()
        {

            // if (Store == string.Empty)
            // {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine("The Store is empty");
            //     Console.ResetColor();
            //     return;
            // }

            if (FallbackState == null || InitialState == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("the fallback state or initial state is null");
                Console.ResetColor();
                return;
            }

            CurrentState = InitialState; // load the first state

            if (!CurrentState.active)
                CurrentState.active = true;

            Console.WriteLine("\t>Start\n\n");

            while (CurrentState != FallbackState)
            {
                Evaluate();
            }

            Console.WriteLine("\n\n\t>End");
        }

        /// <summary>
        /// Evaluates the transitions of the current state.
        /// </summary>
        public void Evaluate()
        {

            if (CurrentState == null)
            {
                throw new NullReferenceException("the current state is null");
            }
            else
            {
                // Console.ForegroundColor = ConsoleColor.Green;
                // Console.WriteLine($"------------------[{CurrentState.Name}]------------------");
                // Console.ResetColor();
                Console.WriteLine();

                int result = CurrentState.Function();
                // Console.WriteLine("Result: " + result);
                State? nextState = CurrentState.EvaluateTransitions(result);

                // Console.WriteLine("Next State: " + nextState?.Name);

                // Console.WriteLine(Store);

                if (nextState != null)
                {
                    CurrentState.active = false;
                    CurrentState = nextState;
                    CurrentState.active = true;
                }
            }
        }

        /// <summary>
        /// Write a value to the store
        /// </summary>
        /// <param name="col">the column</param>
        /// <param name="row">the row</param>
        /// <param name="data">the data</param>
        /// <returns>returns true if the value is written, false if there was an error</returns>
        public bool WriteValue(int col, int row, string data)
        {

            if (data.Contains(","))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Cannot write a value to the store with a comma");
                Console.ResetColor();
                return false;
            }

            string[]? lines = Store.Split("\n");
            if (lines != null)
            {
                string line = lines[row];
                string[]? cells = line.Split(",");

                if (cells != null)
                {
                    cells[col] = data;
                    lines[row] = string.Join(',', cells);
                    string result = string.Join('\n', lines);
                    Store = result;
                    // Console.WriteLine(data);
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Cells is null for this line.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: This line does not exist.");
                Console.ResetColor();
            }

            return false;
        }

        /// <summary>
        /// Evaluates the transitions of the current state. (Note that commas cannot be used in the store)
        /// </summary>
        /// <param name="col">the column</param>
        /// <param name="row">the row</param>
        /// <returns>returns the value from the csv</returns>
        public bool ReadKey(int col, int row, out string result)
        {
            var lines = Store.Split("\n");
            if (lines.Length > row)
            {
                var line = lines[row];
                var cells = line.Split(",");

                if (cells.Length > col)
                {
                    var cell = cells[col];
                    result = cell;
                    return true;
                }
            }

            result = "";
            return false;
        }
    }
}