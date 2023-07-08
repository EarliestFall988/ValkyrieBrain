using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public class State
    {
        /// <summary>
        /// The list of transitions that this state can make.
        /// </summary>
        /// <typeparam name="Transition">The Transition</typeparam>
        /// <returns></returns>
        public List<Transition> Transitions = new List<Transition>();

        /// <summary>
        /// The name of the state
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The job performed by the state
        /// </summary>
        /// <value></value>
        public Func<int> Job { get; set; }

        /// <summary>
        /// Is the state active?
        /// </summary>
        /// <value></value>
        public bool active { get; set; }

        /// <summary>
        /// Creates a new state with the given name.
        /// </summary>
        /// <param name="name"></param>
        public State(string name, Func<int> stateJob)
        {
            Name = name;
            active = false;
            Job = stateJob;
        }

        public State? EvaluateTransitions(int outcome)
        {
            foreach (var x in Transitions)
            {
                var result = x.EvaluateCondition(outcome);
                if (result)
                {
                    return x.To;
                }
            }

            return null;
        }

        public override string ToString()
        {
            string isActive = active == true ? "Active" : "Inactive";

            return $"{Name} {isActive} Transitions: {Transitions.Count}";
        }
    }
}