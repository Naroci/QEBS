using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace QEBS.Base
{
    [Serializable]
    public class GameStateEventArgs : GameEventArgs
    {
        public GameStateEventArgs()
        {
             this.MethodArguments = new MethodArgumenter();
        }

        public GameStateEventArgs(DateTime timestampOverride)
        {
            OverrideTimeStamp(timestampOverride);
            if (this.MethodArguments == null)
                this.MethodArguments = new MethodArgumenter();
        }

        public GameStateEventArgs(string _TargetClass = null, List<Argument> _Arguments = null)
        {
            //this.ObjectsName = _TargetObjectsName;
            this.TargetClass = _TargetClass;
            this.Arguments =_Arguments;
            this.MethodArguments = new MethodArgumenter();
        }

        public GameStateEventArgs(string _TargetClass = null, string MethodInstructionName = null, object[] MethodInstructionArguments = null)
        {
            this.TargetClass = _TargetClass;
            this.MethodName = MethodInstructionName;
            this.MethodArguments = new MethodArgumenter();
            if (MethodInstructionArguments != null){
                
                foreach (var itm in MethodInstructionArguments){
                    this.MethodArguments.Add(new MethodArgument(itm));
                }
            }
        }

        public GameStateEventArgs(string _TargetClass = null, List<Argument> _Arguments = null, string MethodInstructionName = null, object[] MethodInstructionArguments = null)
        {
            this.TargetClass = _TargetClass;
            this.Arguments =_Arguments;
            this.MethodName = MethodInstructionName;
            this.MethodArguments = new MethodArgumenter();
            if (MethodInstructionArguments != null){
                foreach (var itm in MethodInstructionArguments){
                    this.MethodArguments.Add(new MethodArgument(itm));
                }
            }
        }

        //public string ObjectsName;

        public string MethodName;

        public MethodArgumenter MethodArguments;

        public string TypeID;

        public string PropertyName;

        public List<Argument> Arguments;

        public static GameStateEventArgs GetWrappedFromEventArgs (GameEventArgs eventArgs)
        {
            if (eventArgs.GetType() == typeof(GameStateEventArgs)){
                return (GameStateEventArgs)eventArgs;
            }

            return null;
        }

        public class MethodArgumenter : List<MethodArgument>
        {

            public MethodArgumenter(){}

            public MethodArgumenter(MethodArgument[] items)
            {
               foreach (var itm in items){
                    if (itm != null){
                        this.Add(itm);
                    }
               }
            }

            public object [] TypedMethodArguments
            {
                get 
                {
                    object[] returnValue = new object[this.Count];
                    for (int i = 0; i < this.Count; i++)
                    {
                        var parseResult = parse(this[i]);
                        if (parseResult != null)
                            returnValue[i] = parseResult;
                    }
                    return returnValue;
                }
            }

            private object parse(MethodArgument obj)
            {
                object returnVal = null;
                if (obj == null || obj?.Value == null && obj?.GetValueType() == null)
                    return null;

                try
                {
                    Type ValueType = obj.Value.GetType();
                    
                    if (ValueType == typeof(JObject)){
                        returnVal = ((JObject)obj.Value).ToObject(obj.GetValueType());
                    }
                    else if (ValueType == typeof(JArray)){
                        returnVal = ((JArray)obj.Value).ToObject(obj.GetValueType());
                    }
                    else if (ValueType == typeof(JValue)){
                        returnVal = ((JValue)obj.Value).ToObject(obj.GetValueType());
                    }
                    else{
                        if (!ValueType.IsEnum)
                            returnVal =  obj.Value;
                        else
                            returnVal =  Enum.Parse(ValueType,obj.Value.ToString());
                    }       
                }
                catch(Exception e)
                {
                            Console.WriteLine(e.Message);
                }
                return returnVal;
            }
        }

        public class MethodArgument
        {
            public object Value {get;set;}

            public string TypeValue {get;set;}

            public MethodArgument()
            {

            }

            public MethodArgument(object Value)
            {
                if (Value != null){
                    this.TypeValue = Value.GetType().FullName;
                    this.Value = Value;
                }
            }

            public Type GetValueType(){
                if (this.TypeValue == null)
                    return null;
                    
                return Type.GetType(this.TypeValue);
            }

        }


    }
}
