

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avalon
{
    /// <summary>
    /// The State Machine Constructor handles the parsing of the program file and the creation of the state machine.
    /// </summary>
    public class StateMachineBuilder
    {
        private Dictionary<string, FunctionDefinition> functions;
        private Dictionary<string, State> States;
        private Dictionary<string, Transition> Transitions;
        private Dictionary<string, KeyTypeDefinition> Variables;
        private FunctionLibrary FunctionLibrary;
        StateMachine StateMachine;

        string FilePath = "";

        public StateMachineBuilder()
        {

            StateMachine = new StateMachine();
            FunctionLibrary = new FunctionLibrary();
            functions = new Dictionary<string, FunctionDefinition>();
            States = new Dictionary<string, State>();
            Transitions = new Dictionary<string, Transition>();
            Variables = new Dictionary<string, KeyTypeDefinition>();
        }

        public Func<int> GetDefaultFunction()
        {
            Func<int> Next = () => 0;
            return Next;
        }

        public void AddState(string name, string jobName)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = GetDefaultFunction();

            if (trimmedJobName != string.Empty)
                stateJob = functions[trimmedJobName].Function;

            AddState(new State(trimmedName, stateJob));
            if (trimmedJobName != null && trimmedJobName != string.Empty)
                Console.WriteLine("adding state " + trimmedName + " : " + trimmedJobName);
            else
                Console.WriteLine("adding state " + trimmedName + " : " + "default");
        }

        public void AddState(State state)
        {
            States.Add(state.Name, state);

        }

        public void AddStartState(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = functions[trimmedJobName];

            State state = new State(trimmedName, stateJob.Function);

            machine.InitialState = state;
            Console.WriteLine("adding initial " + trimmedName + " : " + trimmedJobName);

            AddState(state);

        }

        public void AddFallBackState(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = GetDefaultFunction();

            if (trimmedJobName != string.Empty)
                stateJob = functions[trimmedJobName].Function;

            // States.Add(name, new State(name, stateJob));
            var state = new State(trimmedName, stateJob);

            machine.FallbackState = state;
            if (trimmedJobName != null && trimmedJobName != string.Empty)
                Console.WriteLine("adding fallback " + trimmedName + " : " + trimmedJobName);
            else
                Console.WriteLine("adding fallback " + trimmedName + " : " + "default");


            AddState(state);
        }

        public void AddTransition(string name, string from, string to, int outcome, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedFrom = from.Trim();
            string trimmedTo = to.Trim();

            Transition t = new Transition(States[trimmedFrom], States[trimmedTo], trimmedName, outcome);

            Transitions.Add(trimmedName, t);


            if (States[trimmedFrom].FallbackState)
            {
                if (machine.FallbackState != null)
                {
                    machine.FallbackState.Transitions.Add(Transitions[trimmedName]);
                    // Console.WriteLine("Added fallback state transition.");
                }
                else
                {
                    Console.WriteLine("fallback state not set in the state machine.");
                }
            }

            if (States[trimmedFrom].InitialState)
            {
                if (machine.InitialState != null)
                {
                    machine.InitialState.Transitions.Add(Transitions[trimmedName]);
                    // Console.WriteLine("Added initial state transition");
                }
                else
                {
                    Console.WriteLine("initial state not set in the state machine.");
                }
            }

            if (!States[trimmedFrom].InitialState && !States[trimmedFrom].FallbackState)
            {
                States[trimmedFrom].Transitions.Add(t);
            }

            Console.WriteLine("adding transition " + trimmedName + " (" + trimmedFrom + " -> " + trimmedTo + " && " + outcome + ")");
        }

        private bool GetType(string type, out StateMachineVariableType result)
        {
            string trimmedType = type.Trim().ToLower();

            switch (trimmedType)
            {
                case "text":
                    result = StateMachineVariableType.Text;
                    return true;
                case "integer":
                    result = StateMachineVariableType.Integer;
                    return true;

                case "decimal":
                    result = StateMachineVariableType.Decimal;
                    return true;

                case "yesno":
                    result = StateMachineVariableType.YesNo;
                    return true;

                default:
                    result = StateMachineVariableType.Text;
                    return false;
            }
        }

        private void InjectParameters(FunctionDefinition function, Dictionary<string, KeyTypeDefinition> parameters, int lineNumber)
        {
            if (parameters.Count > 0 && function != null)
            {
                bool success = function.TryInjectParameters(parameters, out var result);
                if (!success)
                {
                    throw new Exception($"{result} (at line {lineNumber})");
                }

                parameters.Clear();
            }
        }

        public async Task<string> GetProgramFile(string path)
        {
            string structure = "";
            using (StreamReader reader = File.OpenText(path))
            {
                structure = await reader.ReadToEndAsync();
            }

            return structure;
        }

        public StateMachine ParseInstructionsJSON(string json)
        {
            var jsonDoc = JsonDocument.Parse(json);

            var root = jsonDoc.RootElement;

            var variables = root.GetProperty("variables");
            var functionsJson = root.GetProperty("functions");
            var states = root.GetProperty("states");
            var transitions = root.GetProperty("transitions");

            bool createdStart = false;
            bool createdFallback = false;

            bool createdVariables = false;
            bool createdFunctions = false;
            bool createdStates = false;
            bool createdTransitions = false;

            foreach (var x in variables.EnumerateArray())
            {
                var name = x.GetProperty("name").GetString();
                var type = x.GetProperty("type").GetString();
                var value = x.GetProperty("value").GetString();

                // Console.WriteLine($"adding variable {name} ({type}) =  {value}.");

                if (type == null)
                    throw new Exception($"Invalid variable definition. A valid type must be given after the name. ({type})");

                if (value == null)
                    throw new Exception($"Invalid variable definition. A valid value must be given after the name. ({value})");

                bool result = GetType(type, out var variableType);

                if (!result)
                    throw new Exception($"Invalid variable definition. A valid type must be given after the name. ({type})");

                if (name == null)
                    throw new Exception($"Invalid variable definition. A valid name must be given after the definition.");

                if (name == "")
                    throw new Exception($"Invalid variable definition. A valid name must be given after the definition.");

                if (Variables.ContainsKey(name))
                    throw new Exception($"Invalid variable definition. The variable {name} already exists.");

                if (variableType == StateMachineVariableType.Text)
                {
                    Console.WriteLine($"adding variable {name} ({variableType}) = \"{value}\".");
                }
                else
                {
                    Console.WriteLine($"adding variable {name} ({variableType}) = {value}.");
                }

                Variables.Add(name, new KeyTypeDefinition(name, variableType, value));

                createdVariables = true;
            }


            foreach (var x in functionsJson.EnumerateArray())
            {
                var name = x.GetProperty("name").GetString();
                var parameters = x.GetProperty("parameters");
                // var code = x.GetProperty("code").GetString();

                Dictionary<string, (StateMachineVariableType type, bool added)> parameterList = new Dictionary<string, (StateMachineVariableType type, bool added)>();

                Dictionary<string, KeyTypeDefinition> injectionVariables = new Dictionary<string, KeyTypeDefinition>();

                if (name == null)
                    throw new Exception($"Invalid function definition. A valid name must be given after the definition.");

                if (!FunctionLibrary.TryGetFunction(name, out var function))
                    throw new Exception($"Invalid function definition. The function {name} does not exist.");

                if (function == null)
                    throw new Exception($"Invalid function definition. The function {name} does not exist.");

                foreach (var y in parameters.EnumerateArray())
                {
                    var paramName = y.GetProperty("name").GetString();
                    var paramType = y.GetProperty("type").GetString();
                    var varToConnectName = y.GetProperty("connectVar").ToString();

                    if (paramName == null)
                        throw new Exception($"Invalid function parameter definition. A valid name must be given after the definition.");

                    if (paramName == "")
                        throw new Exception($"Invalid function parameter definition. A valid name must be given after the definition.");

                    if (paramType == null)
                        throw new Exception($"Invalid function parameter definition. A valid type must be given after the name.");

                    bool result = GetType(paramType, out var variableType);

                    if (!result)
                        throw new Exception($"Invalid function parameter definition. A valid type must be given after the name. ({paramType})");

                    parameterList.Add(paramName, (variableType, true));

                    if (!Variables.TryGetValue(varToConnectName, out var variable))
                        throw new Exception($"Invalid function parameter definition. The connection variable {varToConnectName} does not exist.");

                    if (variable == null)
                        throw new Exception($"Invalid function parameter definition. The connection variable {varToConnectName} cannot be generated.");

                    if (variable.Type != variableType)
                        throw new Exception($"Invalid function parameter definition. The connection variable {varToConnectName} is not of type {variableType}.");

                    injectionVariables.Add(paramName, variable);
                }

                function.ExpectedParameters = parameterList;

                bool injectionResult = function.TryInjectParameters(injectionVariables, out var result2);

                if (!injectionResult)
                    throw new Exception($"{result2}");

                if (functions.ContainsKey(name))
                    throw new Exception($"Invalid function definition. The function {name} already exists.");

                functions.Add(name, function);

                createdFunctions = true;
            }


            foreach (var x in functions)
            {
                Console.WriteLine($"Function added {x.Key} {string.Join(',', x.Value.ExpectedParameters.Keys)} .");
            }


            foreach (var s in states.EnumerateArray())
            {

                var name = s.GetProperty("name").GetString();
                var isStart = false;
                var isFallback = false;

                var type = s.GetProperty("type").GetString();

                if (name == null || name == "")
                    throw new Exception($"Invalid state definition. A valid name must be given after the definition.");

                if (type == "start")
                {
                    isStart = true;
                }
                else if (type == "fallback")
                {
                    isFallback = true;
                }
                else if (type != "state")
                {
                    throw new Exception($"Invalid state definition. A state must be of type 'state, fallback, or start'.");
                }

                if (isStart && createdStart)
                    throw new Exception($"Invalid state definition. A start state has already been defined.");

                if (isFallback && createdFallback)
                    throw new Exception($"Invalid state definition. A fallback state has already been defined.");

                if (isStart)
                {
                    createdStart = true;
                }

                if (isFallback)
                {
                    createdFallback = true;
                }

                if (States.ContainsKey(name))
                    throw new Exception($"Invalid state definition. The state {name} already exists.");

                var functionName = s.GetProperty("function").GetString();

                if (functionName == null)
                    throw new Exception($"Invalid state definition. A valid function must be given after the definition. (Make sure the function is declared in the functions section of the json file.)");


                if (isStart)
                {
                    AddStartState(name, functionName, StateMachine);
                }
                else if (isFallback)
                {
                    AddFallBackState(name, functionName, StateMachine);
                }
                else
                {
                    AddState(name, functionName);
                }

                createdStates = true;
            }


            foreach (var x in States)
            {
                Console.WriteLine($"State added {x.Key}.");
            }

            foreach (var t in transitions.EnumerateArray())
            {
                var from = t.GetProperty("from").GetString();
                var to = t.GetProperty("to").GetString();
                var outcome = t.GetProperty("outcome").GetInt32();

                string name = $"transition {from} -> {to}";

                if (from == null || from == "")
                    throw new Exception($"Invalid transition definition. A valid from state must be given after the definition.");

                if (to == null || to == "")
                    throw new Exception($"Invalid transition definition. A valid to state must be given after the definition.");

                if (name == null || name == "")
                    throw new Exception($"Invalid transition definition. A valid name must be given after the definition.");

                AddTransition(name, from, to, outcome, StateMachine);

                createdTransitions = true;
            }


            Console.WriteLine("\n\t>Building State Machine...\n\n");


            if (!createdVariables)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No variables defined.");
                Console.ResetColor();
            }

            if (!createdFunctions || functions.Count < 1)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No functions defined.");
                Console.ResetColor();
            }

            if (!createdStart)
            {
                throw new Exception($"No start state defined.");
            }

            if (!createdFallback)
            {
                throw new Exception($"No fallback state defined.");
            }

            if (!createdStates)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No states defined.");
                Console.ResetColor();
            }

            if (!createdTransitions)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No transitions defined.");
                Console.ResetColor();
            }

            Console.WriteLine("\t>Result:\n\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Imported Functions:");
            Console.ResetColor();
            foreach (var x in functions.Values)
            {

                string pmtrs = "";

                foreach (var y in x.ExpectedParameters)
                {
                    pmtrs += y.Key + ": " + y.Value.type + ", ";
                }

                pmtrs = pmtrs.Trim().TrimEnd(',');

                Console.WriteLine("|" + x.Name + " [" + pmtrs + "]");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nDefined States:");
            Console.ResetColor();

            foreach (var x in States.Values)
            {
                Console.WriteLine("|" + x.Name + " [" + String.Join(',', x.Transitions) + "]");
            }

            Console.WriteLine("\n\t>Program Loaded.\n");


            StateMachine.States.AddRange(States.Values);

            return StateMachine;
        }


        public async Task<StateMachine> ParseInstructions(string filePath)
        {

            string structure = await GetProgramFile(filePath);

            string[] lines = structure.Split("\n");
            var defineStates = false;
            var hasDefinedStates = false;

            var createVariables = false;
            var hasCreatedVariables = false;

            var importFunctions = false;
            FunctionDefinition? currentFunction = null;// default function
            Dictionary<string, KeyTypeDefinition> parameters = new Dictionary<string, KeyTypeDefinition>();
            var hasImportedFunctions = false;

            var defineTransitions = false;

            var createdStart = false;
            var createdFallback = false;

            int lineNumber = 1;

            Console.WriteLine("\t>Starting Parser and Interpreter...\n\n");

            foreach (var line in lines)
            {
                var x = line.Trim();
                if (x.StartsWith("//") || x == "")
                    continue;

                if (x.ToLower().Trim() == "create")
                {
                    createVariables = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end create")
                {
                    createVariables = false;
                    hasCreatedVariables = true;
                    Console.WriteLine("");
                    continue;
                }


                if (x.ToLower().Trim() == "end create")
                {
                    createVariables = false;
                    hasCreatedVariables = true;
                    Console.WriteLine("");
                    continue;
                }

                if (x.ToLower().Trim() == "import")
                {
                    importFunctions = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end import")
                {
                    if (currentFunction != null)
                        InjectParameters(currentFunction, parameters, lineNumber);

                    importFunctions = false;
                    hasImportedFunctions = true;
                    Console.WriteLine("");
                    continue;
                }

                if (x.ToLower().Trim() == "define")
                {
                    defineStates = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end define")
                {
                    defineStates = false;
                    hasDefinedStates = true;
                    Console.WriteLine("");
                    continue;
                }
                if (x.ToLower().Trim() == "connect")
                {
                    if (!hasDefinedStates)
                        throw new Exception($"Cannot define transitions without defining states first. (at line {lineNumber})");

                    defineTransitions = true;
                    continue;
                }

                if (x.ToLower().Trim() == "end connect")
                {
                    defineTransitions = false;
                    hasCreatedVariables = true;
                    Console.WriteLine("");
                    continue;
                }

                if (createVariables)
                {
                    var split = x.Split(" ");
                    var name = split[0];

                    var type = split[1];


                    if (name == null || name == "")
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");

                    if (type == null || type == "")
                        throw new Exception($"Invalid variable definition. A valid type must be given after the name. Currently it does not exist. (at line {lineNumber})");

                    var captureValue = x.Split("=");
                    var value = "";

                    if (captureValue.Length < 2 || captureValue[1] == null || captureValue[1] == string.Empty)
                    {
                        throw new Exception($"Invalid variable definition. A valid variable must be given after the \'=\' sign. (at line {lineNumber})");
                    }


                    bool result = GetType(type, out var variableType);

                    if (!result)
                        throw new Exception($"Invalid variable definition. A valid type must be given after the name. ({type}) (at line {lineNumber})");

                    if (variableType == StateMachineVariableType.Text)
                    {

                        var text = captureValue[1].Trim();


                        for (int i = 0; i < text.Length; i++)
                        {

                            if (text[i] == '\"')
                            {
                                continue;
                            }

                            if (text[i] == '\\')
                            {
                                if (text.Length > i + 1)
                                {
                                    if (text[i + 1] == '\"')
                                    {
                                        value += '\"';
                                        i++;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                value += text[i];
                            }
                        }
                    }
                    else
                    {
                        value = captureValue[1].Trim();
                    }

                    // Console.WriteLine($"adding variable {name} ({variableType}) =  {value}.");

                    // KeyTypeDefinition def = new KeyTypeDefinition(name, variableType, value);
                    // Console.WriteLine($"adding variable {def.Key} ({def.Type}) =  {def.Value}.");

                    if (name == null)
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");

                    if (name == "")
                        throw new Exception($"Invalid variable definition. A valid name must be given after the definition. (at line {lineNumber})");


                    if (Variables.ContainsKey(name))
                        throw new Exception($"Invalid variable definition. The variable {name} already exists. (at line {lineNumber})");

                    if (variableType == StateMachineVariableType.Text)
                    {
                        Console.WriteLine($"adding variable {name} ({variableType}) = \"{value}\".");
                    }
                    else
                    {
                        Console.WriteLine($"adding variable {name} ({variableType}) = {value}.");
                    }

                    Variables.Add(name, new KeyTypeDefinition(name, variableType, value));

                    continue;
                }

                if (importFunctions)
                {
                    var split = x.Split(" ");
                    var isUsing = split[0].Trim().ToLower() == "using";
                    var functionName = split[1].Trim();

                    var insertFunParamName = "";
                    var paramName = "";

                    if (x.Contains("="))
                    {
                        insertFunParamName = x.Split("=")[0].Trim();
                        paramName = x.Split("=")[1].Trim();
                    }


                    if (functionName.Length < 1)
                        throw new Exception($"Invalid function definition. A valid function name must be given after the definition. (at line {lineNumber})");





                    if (isUsing)
                    {
                        Console.WriteLine($"adding function {functionName}.");

                        if (parameters.Count > 0 && currentFunction != null)
                        {
                            InjectParameters(currentFunction, parameters, lineNumber);
                        }

                        if (!FunctionLibrary.TryGetFunction(functionName, out var function))
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        if (function == null)
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        currentFunction = function;
                        functions.Add(functionName, currentFunction);
                        continue;
                    }
                    else
                    {

                        Console.WriteLine($"adding parameter '{paramName}' to function insert '{insertFunParamName}' in function {currentFunction?.Name ?? "unknown"}.");

                        if (currentFunction == null)
                            throw new Exception($"Invalid function definition. The function {functionName} does not exist. (at line {lineNumber})");

                        if (functions.ContainsKey(functionName))
                            throw new Exception($"Invalid function definition. The function {functionName} already exists. (at line {lineNumber})");

                        string[] splitStr = x.Split("=");
                        string insertName = splitStr[0];
                        string variableName = "";

                        if (splitStr.Length > 1)
                            variableName = splitStr[1];
                        else
                            throw new Exception($"Invalid function parameter definition. A valid variable name must be given after the parameter insert name. (at line {lineNumber})");

                        variableName = variableName.Trim();
                        insertName = insertName.Trim();

                        if (insertName == null || insertName == "")
                            throw new Exception($"Invalid function parameter definition. A valid function insert name must be given. (at line {lineNumber})");

                        if (variableName == null || variableName == "")
                            throw new Exception($"Invalid function parameter definition. A valid variable name must be given after the parameter insert name. (at line {lineNumber})");

                        if (!Variables.ContainsKey(variableName))
                            throw new Exception($"Invalid function parameter definition. The variable {variableName} does not exist. (at line {lineNumber})");

                        var variable = Variables[variableName];

                        currentFunction.TryGetVariableType(insertName, out var variableType);

                        if (variable.Type != variableType)
                            throw new Exception($"Invalid function parameter definition. The variable {variableName} is not of type {variableType}. (at line {lineNumber})");

                        parameters.Add(insertName, variable);

                        continue;
                    }
                }

                if (defineStates)
                {
                    if (x.ToLower().StartsWith("state"))
                    {
                        if (x.Split(" ").Length < 3 || !x.Contains(":"))
                            throw new Exception($"Invalid state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var splitCheck = x.Split(":");
                        var stateFunctionName = "";
                        if (splitCheck != null && splitCheck.Length > 1) stateFunctionName = x.Split(":")[1];

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");


                        AddState(stateName, stateFunctionName);


                        continue;
                    }

                    if (x.ToLower().StartsWith("start"))
                    {
                        if (x.Split(" ").Length < 3 || !x.Contains(":"))
                            throw new Exception($"Invalid start state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var stateFunctionName = x.Split(":")[1] ?? throw new Exception($"Invalid start state definition. A valid function must be given after the definition and name. (at line {lineNumber})");

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");

                        // AddState(stateName, stateFunctionName);
                        AddStartState(stateName, stateFunctionName, StateMachine);

                        States[stateName].InitialState = true;

                        createdStart = true;

                        continue;
                    }

                    if (x.ToLower().StartsWith("fallback"))
                    {
                        if (x.Split(" ").Length < 2)
                            throw new Exception($"Invalid fallback state definition. (at line {lineNumber})");

                        var stateName = x.Split(" ")[1];
                        var splitCheck = x.Split(":");
                        var stateFunctionName = "";
                        if (splitCheck != null && splitCheck.Length > 1) stateFunctionName = x.Split(":")[1];

                        if (stateName == null || stateName == "")
                            throw new Exception($"Invalid fallback state definition. A valid name must be given after the definition. (at line {lineNumber})");

                        // Console.WriteLine("creating fallback");

                        // AddState(stateName, stateFunctionName);
                        AddFallBackState(stateName, stateFunctionName, StateMachine);

                        States[stateName].FallbackState = true;

                        createdFallback = true;

                        continue;
                    }
                }

                if (defineTransitions)
                {
                    if (!x.Contains("->"))
                    {
                        throw new Exception($"Invalid transition definition. (at line {lineNumber})");
                    }

                    var separator = x.Split("=");
                    var name = separator[0].Trim();
                    var transition = separator[1].Trim();

                    var fromState = transition.Split("->")[0];
                    var toState = transition.Split("->")[1].Split(":")[0];
                    var outcome = int.Parse(transition.Split(":")[1].Trim());


                    AddTransition(name, fromState, toState, outcome, StateMachine);

                    continue;
                }


                lineNumber++;
            }

            Console.WriteLine("\t>Building State Machine...\n\n");

            if (!hasCreatedVariables)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No variables defined.");
                Console.ResetColor();
            }

            if (!hasImportedFunctions || functions.Count < 1)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No functions defined.");
                Console.ResetColor();
            }

            if (!createdStart)
            {
                throw new Exception($"No start state defined.");
            }

            if (!createdFallback)
            {
                throw new Exception($"No fallback state defined.");
            }

            Console.WriteLine("\t>Result:\n\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Imported Functions:");
            Console.ResetColor();
            foreach (var x in functions.Values)
            {

                string pmtrs = "";

                foreach (var y in x.ExpectedParameters)
                {
                    pmtrs += y.Key + ": " + y.Value.type + ", ";
                }

                pmtrs = pmtrs.Trim().TrimEnd(',');

                Console.WriteLine("|" + x.Name + " [" + pmtrs + "]");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\nDefined States:");
            Console.ResetColor();

            foreach (var x in States.Values)
            {
                Console.WriteLine("|" + x.Name + " [" + String.Join(',', x.Transitions) + "]");
            }

            Console.WriteLine("\n\t>Program Loaded.\n");

            StateMachine.States.AddRange(States.Values);

            return StateMachine;
        }
    }
}