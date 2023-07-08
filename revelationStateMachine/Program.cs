
using System;
using System.Collections.Generic;
using revelationStateMachine;

Console.WriteLine("\t>Starting State Machine...\n\n");



string store =
"""
5,6
4,2
"""; // CSV data



// Console.WriteLine("data" + " " + stateMachine.Store);
// Console.WriteLine("data " + stateMachine.ReadKey(1, 1));
StateMachine stateMachine = new StateMachine();
stateMachine.Store = store;

Func<string, bool> Exit = (x) =>
{
    if (x == "Exit")
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

State newState = new State("newState", Next);


State initialState = new State("state", CheckCondition);
State fallbackState = new State("fallback", Next);

newState.Transitions.Add(new Transition(newState, initialState, "return", 0));

initialState.Transitions.Add(new Transition(initialState, newState, "1", 1));
initialState.Transitions.Add(new Transition(initialState, fallbackState, "exit", -1));

stateMachine.InitialState = initialState;
stateMachine.FallbackState = fallbackState;

stateMachine.AddState(newState);

stateMachine.Start();

Console.WriteLine("\n\n\t>Exiting...");