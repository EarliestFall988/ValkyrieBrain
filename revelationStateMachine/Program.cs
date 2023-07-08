
using System;
using System.Collections.Generic;
using revelationStateMachine;

Console.WriteLine("\t>Start");

State initialState = new State("state");
State fallbackState = new State("fallback");

string data =
"""
5,6
4,2
"""; // CSV data



StateMachine stateMachine = new StateMachine(initialState, fallbackState, data);

Console.WriteLine("data" + " " + stateMachine.Data);
Console.WriteLine("data " + stateMachine.ReadKey(1, 1));

State newState = new State("newState");

stateMachine.AddState(newState);

Func<bool> condition = () =>
{

    Console.WriteLine("Type in a number");
    string? number = Console.ReadLine();

    if (number == null || number.Trim() == "")
        return false;

    Console.WriteLine("Checking Data @ 1,1");
    bool result = stateMachine.ReadKey(1, 1) == number.Trim();
    Console.WriteLine("Result: " + result);
    return result;
};

Transition transition = new Transition(initialState, newState, "test", condition);
initialState.Transitions.Add(transition);

stateMachine.Evaluate();

Console.WriteLine("\t>End");