using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace revelationStateMachine
{
    public class KeyTypeDefinition
    {
        public string Key { get; set; } = "";
        public StateMachineVariableType Type { get; set; } = StateMachineVariableType.Text;
        public string Value { get; private set; } = "";

        public KeyTypeDefinition(string key, StateMachineVariableType type, string value)
        {
            Key = key;
            Type = type;
            Value = value;
        }

        public string GetText()
        {
            return Value;
        }

        public Int32 GetInt()
        {
            return Int32.Parse(Value);
        }

        public Decimal GetDecimal()
        {
            return Decimal.Parse(Value);
        }

        public bool GetYesNo()
        {
            if (Value.ToLower() == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetValue(string value)
        {
            this.Value = value;
        }
    }

    public enum StateMachineVariableType
    {
        Text, //text
        Decimal, //decimal
        Integer, //int
        YesNo, //boolean
    }
}