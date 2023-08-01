
using System;
using System.Collections.Generic;
using revelationStateMachine;

string programFilePath = @"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\program.rstm";
// StateMachineConstructor constructor = new StateMachineConstructor(programFilePath);

// StateMachine machine = await constructor.ParseInstructions();
// StateMachineConstructor.BootStateMachine(machine);

string json = System.IO.File.ReadAllText(@"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\stateMachine.json");

StateMachineConstructor constructor = new StateMachineConstructor(programFilePath);
constructor.ParseInstructionsJSON(json);