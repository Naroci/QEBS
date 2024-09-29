using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QEBS.Base
{
    // Eine Klasse zum ermitteln von Hashwerten
    public class InstructionCreator
    {
        public GameInstruction CreateGameInstruction (string ExecutionClass, List<Argument> InstructionArguments = null, bool TargetsAllPlayers = false)
        {
            var inst = new GameInstruction();
            inst.ExecutionClassName = ExecutionClass;
            inst.Arguments = InstructionArguments;
            inst.SourceTimeStamp = DateTime.UtcNow;
            inst.TargetsAllPlayers = TargetsAllPlayers;
            return inst;
        }

        private static InstructionCreator _instance = null;

        public static InstructionCreator GetInstance() 
        {
            if (_instance == null)
                _instance = new InstructionCreator();

            return _instance;
        }

        public static string CreateInstructionString(Source TargetSource, string CommandName){
            switch (TargetSource){
                case Source.Client:
                    return string.Format("GameQueueBase.Base.Commanding.Commands.Client.{0}",CommandName);
                case  Source.Server:
                    return string.Format("GameQueueBase.Base.Commanding.Commands.Server.{0}",CommandName);
                default:
                    return null;
            }
        }
    

    }
}
