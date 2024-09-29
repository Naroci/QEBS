using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QEBS.Base
{
    [Serializable]
    public class GameInstruction
    {
        public static GameInstruction GenerateFromGameEventArgs(Source TargetSource, string Command,List<GameEventArgs> EventArgs, string InstructionIdentifier) 
        {
            GameInstruction reply = null;
            var instrCreator = InstructionCreator.GetInstance();        
            List<EventInstruction>  Instructions = new List<EventInstruction>();
            if (EventArgs != null && EventArgs.Count > 0){
                    foreach(var eventIns in EventArgs)
                    {
                        var instruction = new EventInstruction(eventIns);
                        Instructions.Add(instruction);
                    }
            }
            
            reply = instrCreator.CreateGameInstruction(InstructionCreator.CreateInstructionString(TargetSource,Command), new List<Argument>() 
            { 
                new Argument (InstructionIdentifier, Instructions)
            });

            return reply;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [JsonIgnore]
        public long Tick; 

        // Auszuführende Klasse
        public string ExecutionClassName;

        // Angehängte Argumente
        public List<Argument> Arguments;

        // Zeitstempel
        public DateTime SourceTimeStamp;

        public bool TargetsAllPlayers;

        public bool RequestedInstruction;

        public GameStateActionState InstructionState;

        public string ClientExecutedGameInstructionName;

        public SendState State;

        public bool IsHumanPlayer;

        public string TargetPlayerID;

        public int ExecutionTryCount = 0;
    }

    [Serializable]
    public class Argument
    {
        public Argument (string entry, object value = null)
        {
            this.Entry = entry;
            this.Value = value;
        }

        public Argument()
        {

        }

        public string Entry;
        public object Value;

        public static Argument CreateArgument(string ArgEntry, object ArgValue = null)
        {
            return new Argument(ArgEntry,ArgValue);
        }
    }
}
