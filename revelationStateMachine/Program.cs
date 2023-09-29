
using Avalon;


// Console.WriteLine("Welcome to Avalon - connect to the web app? (y/n)");
// var input = Console.ReadLine();


// if (input == "y")
// {

// Console.WriteLine("What is you user name?");
// var userName = Console.ReadLine();

// if (userName == null || userName.Length == 0)
// {
//     Console.WriteLine("Invalid user name");
//     return;
// }

// Console.WriteLine("What is the 6 digit pin?");
// var pin = Console.ReadLine();

// if (pin == null || pin.Length != 6)
// {
//     Console.WriteLine("Invalid pin length");
//     return;
// }

// Console.WriteLine("Connecting to web app...");
var webController = new ValkyrieWebConnectionController("123123", "howelltaylor195@gmail.com");

await webController.Connect();
// await webController.Authenticate();
await webController.SubscribeToChannel();

// Console.WriteLine("Success! Connected to web app.");
Console.WriteLine("Done.");

Task.Delay(50000).Wait();

webController.Disconnect();
// }
// else
// {
//     Console.WriteLine("Not connecting to web app.");
// }

//boot the state machine

// string baseDomain = Environment.CurrentDirectory;

// string json = System.IO.File.ReadAllText($@"{baseDomain}\stateMachine.json"); //get the json file

// StateMachineBuilder builder = new StateMachineBuilder(); // create a state machine builder
// var stateMachine = builder.ParseInstructionsJSON(json); // parse the json file and build the state machine


// stateMachine.Boot(); // boot the state machine