

namespace revelationStateMachine
{
    public class StateMachineConstructor
    {
        public Dictionary<string, Func<int>> functions = new Dictionary<string, Func<int>>();
        public Dictionary<string, State> States = new Dictionary<string, State>();

        public Dictionary<string, Transition> Transitions = new Dictionary<string, Transition>();


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
                stateJob = functions[trimmedJobName];

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

        public void AddStartStateMachine(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = functions[trimmedJobName];

            State state = new State(trimmedName, stateJob);

            machine.InitialState = state;
            Console.WriteLine("adding initial " + trimmedName + " : " + trimmedJobName);

            AddState(state);

        }

        public void AddFallBackStateMachine(string name, string jobName, StateMachine machine)
        {

            string trimmedName = name.Trim();
            string trimmedJobName = jobName.Trim();

            var stateJob = GetDefaultFunction();

            if (trimmedJobName != string.Empty)
                stateJob = functions[trimmedJobName];

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

        public void ParseInstructions(string structure, StateMachine sm)
        {
            string[] lines = structure.Split("\n");
            var defineStates = false;
            var hasDefinedStates = false;
            var defineTransitions = false;

            var createdStart = false;
            var createdFallback = false;

            int lineNumber = 1;

            foreach (var line in lines)
            {
                var x = line.Trim();
                if (x.StartsWith("//") || x == "")
                    continue;

                if (x.ToLower() == "define")
                {
                    defineStates = true;
                    continue;
                }

                if (x.ToLower() == "end define")
                {
                    defineStates = false;
                    hasDefinedStates = true;
                    Console.WriteLine("\n");
                    continue;
                }
                if (x.ToLower() == "connect")
                {
                    if (!hasDefinedStates)
                        throw new Exception($"Cannot define transitions without defining states first. (at line {lineNumber})");

                    defineTransitions = true;
                    continue;
                }

                if (x.ToLower() == "end connect")
                {
                    defineTransitions = false;
                    Console.WriteLine("\n");
                    continue;
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
                        AddStartStateMachine(stateName, stateFunctionName, sm);

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
                        AddFallBackStateMachine(stateName, stateFunctionName, sm);

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


                    AddTransition(name, fromState, toState, outcome, sm);

                    continue;
                }




                lineNumber++;
            }



            if (!createdStart)
            {
                throw new Exception($"No start state defined.");
            }

            if (!createdFallback)
            {
                throw new Exception($"No fallback state defined.");
            }

            Console.WriteLine("\tResult:");
            foreach (var x in States.Values)
            {
                Console.WriteLine("|" + x.Name + " -> " + String.Join(',', x.Transitions));
            }

            Console.WriteLine("\n\n");

            sm.States.AddRange(States.Values);
        }
    }
}