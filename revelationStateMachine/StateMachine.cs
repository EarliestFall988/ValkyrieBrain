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
        public string Data { get; set; }

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
        public State CurrentState { get; set; }

        /// <summary>
        /// The state that the machine will fall back to if it cannot transition to the next state, or can be used as a 'throw error' state
        /// </summary>
        /// <value></value>
        public State FallbackState { get; set; }

        /// <summary>
        /// Creates a new state machine with the given initial state and fallback state.
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="fallbackState"></param>
        public StateMachine(State initialState, State fallbackState, string data = "")
        {
            FallbackState = fallbackState;
            CurrentState = initialState;
            Data = data;
        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state"></param>
        public void AddState(State state)
        {
            States.Add(state);
        }

        /// <summary>
        /// Evaluates the transitions of the current state.
        /// </summary>
        public void Evaluate()
        {
            Console.WriteLine($"Evaluating {CurrentState.Name}");
            CurrentState.EvaluateTransitions();
            Console.WriteLine("------------------\t------------------");
            Console.ReadKey();
        }

        /// <summary>
        /// Evaluates the transitions of the current state. (Note that commas cannot be used in the data)
        /// </summary>
        /// <param name="col">the column</param>
        /// <param name="row">the row</param>
        /// <returns>returns the value from the csv</returns>
        public string ReadKey(int col, int row)
        {
            var lines = Data.Split("\n");
            var line = lines[row];
            var cells = line.Split(",");
            var cell = cells[col];

            return cell;
        }
    }
}