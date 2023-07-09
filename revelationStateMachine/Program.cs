
using System;
using System.Collections.Generic;
using revelationStateMachine;

Console.WriteLine("\t>Starting State Machine...\n\n");
Dictionary<string, Func<int>> functions = new Dictionary<string, Func<int>>();
Dictionary<string, State> States = new Dictionary<string, State>();
Dictionary<string, Transition> Transitions = new Dictionary<string, Transition>();

string store =
"""
5,6
4,2
"""; // CSV data


string structure =

"""
define
    
    //define the start and fallback states
    start s : Guess
    fallback exit

    //define intermediary states
    //state middle : Next
    state exitQuestion : ExitQuestion

end define


connect

    success = s -> exitQuestion : 1
    // return = middle -> s: 0
    exitQuestionSuccess = exitQuestion -> exit : 1
    exitQuestionFail = exitQuestion -> s : 0
    fail = s -> s : 0
    exit = s -> exit : -1

end connect

""";

StateMachine stateMachine = new StateMachine();
stateMachine.Store = store;
StateMachineConstructor constructor = new StateMachineConstructor();
constructor.functions = new Functions(stateMachine).functions;
constructor.ParseInstructions(structure, stateMachine);



// Console.WriteLine("data" + " " + stateMachine.Store);
// Console.WriteLine("data " + stateMachine.ReadKey(1, 1));

// var f = new Functions(stateMachine).functions;
// stateMachine.Store = store;

// State newState = new State("newState", f["Next"]);


// State initialState = new State("state", f["CheckCondition"]);
// State fallbackState = new State("fallback", f["Next"]);

// newState.Transitions.Add(new Transition(newState, initialState, "return", 0));

// initialState.Transitions.Add(new Transition(initialState, newState, "1", 1));
// initialState.Transitions.Add(new Transition(initialState, fallbackState, "exit", -1));

// stateMachine.InitialState = initialState;
// stateMachine.FallbackState = fallbackState;

// stateMachine.AddState(newState);

stateMachine.Start();

Console.WriteLine("\n\n\t>Exiting...");