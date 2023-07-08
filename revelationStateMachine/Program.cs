
using System;
using System.Collections.Generic;
using revelationStateMachine;

Console.WriteLine("Start");

State initialState = new State("state");

StateMachine stateMachine = new StateMachine(initialState);