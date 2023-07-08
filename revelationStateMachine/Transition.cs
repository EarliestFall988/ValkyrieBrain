using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public struct Transition
    {
        /// <summary>
        /// The state that the transition is coming from
        /// </summary>
        /// <value></value>
        public State From { get; set; }

        /// <summary>
        /// The state that the transition is going to
        /// </summary>
        /// <value></value>
        public State To { get; set; }

        /// <summary>
        /// The name of the transition
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The condition that must be met for the transition to occur
        /// </summary>
        /// <value></value>
        public Func<bool> Condition { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="from">
        ///   The state that the transition is coming from
        /// </param>
        /// <param name="to">
        ///  The state that the transition is going to
        /// </param>
        public Transition(State from, State to, string name, Func<bool> condition)
        {
            From = from;
            To = to;
            Name = name;
            Condition = condition;
        }
    }
}