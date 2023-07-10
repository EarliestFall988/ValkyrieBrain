
using System;
using System.Collections.Generic;
using revelationStateMachine;




string programFilePath = @"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\program.lg";
StateMachineConstructor constructor = new StateMachineConstructor(programFilePath);

StateMachine machine = await constructor.ParseInstructions();
StateMachineConstructor.BootStateMachine(machine);




// 
// Dictionary<string, Func<int>> functions = new Dictionary<string, Func<int>>();
// Dictionary<string, State> States = new Dictionary<string, State>();
// Dictionary<string, Transition> Transitions = new Dictionary<string, Transition>();

// string store =
// """
// 5,6
// 4,2
// """; // CSV data


// string structure = "";
// using (StreamReader reader = File.OpenText(@"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\program.lg"))
// {
//     structure = await reader.ReadToEndAsync();
// }

// Console.WriteLine("read file: \n" + structure);

// Console.WriteLine("\t>Launching State Machine...\n");

// StateMachine stateMachine = new StateMachine();
// // stateMachine.Store = store;
// string filePath = @"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\program.lg";
// StateMachineConstructor constructor = new StateMachineConstructor(filePath);
// // constructor.functions = new FunctionLibrary().ImportedFunctions;
// // string structure = await constructor.GetProgramFile();
// StateMachine machine = await constructor.ParseInstructions();
// StateMachineConstructor.Boot(stateMachine);



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