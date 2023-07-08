using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace revelationStateMachine
{
    public class StateMachine
    {
        public List<State> States = new List<State>();
        public State CurrentState { get; set; }

        public StateMachine(State initialState)
        {
            CurrentState = initialState;
        }

        public void AddState(State state)
        {
            States.Add(state);
        }
    }
}