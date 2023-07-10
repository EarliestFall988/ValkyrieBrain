using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace revelationStateMachine
{
    /// <summary>
    /// The function metadata definition
    /// </summary>
    public abstract class FunctionDefinition
    {
        public Func<int> Function { get; set; } = () => { return 0; }; //default function

        /// <summary>
        /// The name of the function
        /// </summary>
        /// <value></value>
        public string Name { get; set; } = "";

        /// <summary>
        /// The dictionary of expected parameters and types
        /// </summary>
        /// <typeparam name="StateMachineVariableType"></typeparam>
        /// <returns></returns>
        protected Dictionary<string, (StateMachineVariableType type, bool applied)> ExpectedParameters { get; set; } = new Dictionary<string, (StateMachineVariableType type, bool applied)>();

        /// <summary>
        /// The dictionary of parameters
        /// </summary>
        /// <value></value>
        protected Dictionary<string, KeyTypeDefinition> Parameters { get; set; } = new Dictionary<string, KeyTypeDefinition>();

        /// <summary>
        /// Inject the parameters into the function definition so the function knows what to call
        /// </summary>
        /// <param name="parameters">the list of parameters</param>
        /// <param name="result">the result of the injection, outputs error messages</param>
        /// <returns></returns>
        public bool TryInjectParameters(Dictionary<string, KeyTypeDefinition> parameters, out string result)
        {
            if (parameters.Count == ExpectedParameters.Count)
            {

                foreach (var x in parameters)
                {
                    if (ExpectedParameters.ContainsKey(x.Key))
                    {
                        if (ExpectedParameters[x.Key].type == x.Value.Type)
                        {
                            Parameters.Add(x.Key, x.Value);
                        }
                        else
                        {
                            result = "Parameter type mismatch. The expected type of parameter " + x.Value.Key + " is " + ExpectedParameters[x.Key].type + " but " + x.Value.Type + " was provided.";
                            return false;
                        }
                    }
                    else
                    {
                        result = "Parameter mismatch. The expected parameter " + x.Key + " was not provided.";
                        return false;
                    }
                }

                result = "Success!";
                return true;
            }
            else
            {
                result = "Parameter count mismatch. The expected number of parameters is " + ExpectedParameters.Count + " but " + parameters.Count + " were provided.";
                return false;
            }
        }

        protected abstract void DefineFunction();

        public bool TryGetVariableType(string name, out StateMachineVariableType result)
        {
            if (ExpectedParameters.ContainsKey(name))
            {
                result = ExpectedParameters[name].type;
                return true;
            }

            result = StateMachineVariableType.Text;
            return false;
        }
    }
}