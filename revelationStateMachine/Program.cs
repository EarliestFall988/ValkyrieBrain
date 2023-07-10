
using System;
using System.Collections.Generic;
using revelationStateMachine;


string programFilePath = @"C:\Users\howel\Documents\Software Development\RevelationStateMachine\revelationStateMachine\program.lg";
StateMachineConstructor constructor = new StateMachineConstructor(programFilePath);

StateMachine machine = await constructor.ParseInstructions();
StateMachineConstructor.BootStateMachine(machine);