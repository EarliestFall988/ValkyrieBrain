using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public class State
    {
        public List<Transition> Transitions = new List<Transition>();
        public string Name { get; set; }

        public State(string name)
        {
            Name = name;
        }
    }
}