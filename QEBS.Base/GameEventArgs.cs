using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QEBS.Base
{
    public abstract class GameEventArgs : EventArgs
    {
        private bool isIstantEvent;

        public bool GetIfIsInstantEvent()
        {
            return this.isIstantEvent;
        }

        public void SetIfIsInstantEvent(bool IsInstant)
        {
            this.isIstantEvent = IsInstant;
        }

        public string SourceGameInstruction;
        private DateTime _createTimeStamp;

        public int OrderNum;

        public GameEventArgs() 
        {
            this._createTimeStamp = DateTime.UtcNow;
        }

        public GameEventArgs(bool InstantFireEvent = false) 
        {
            this._createTimeStamp = DateTime.UtcNow;
            isIstantEvent = InstantFireEvent;
        }

        public string TargetClass;

        private bool _initiated;
        public bool Initiated
        {
            get
            {
                return _initiated;
            }
            set{
                if (value == true)
                {
                    this.initiationTime = DateTime.UtcNow;
                }
                this._initiated = value;
            }

        }

        public bool Handled;

        public bool Completed;

        public DateTime TimeStamp 
        { 
            get => this._createTimeStamp; 
        }

        private DateTime initiationTime = DateTime.MinValue;
        public DateTime GetInitiationTime(){
            return initiationTime;
        }

        public void OverrideTimeStamp(DateTime timeStamp){
            this._createTimeStamp = timeStamp;
        }

        //public abstract EventInstruction GetEventInstruction();
    }

    [Serializable]
    public class EventInstruction
    {
        public object EventInformation;

        public EventType TypeOfEvent;

        public EventInstruction (GameEventArgs eventBase)
        {
            if (eventBase.GetType() == typeof(GameStateEventArgs))
            {
                 this.TypeOfEvent = EventType.GameEvent;
                 this.EventInformation = GameEventInfo.ParseEventInfoFromEventArgs((GameStateEventArgs)eventBase);
            }
               
            if (eventBase.GetType() == typeof(AnimationEventArgs)){
                this.TypeOfEvent = EventType.Animation;
                this.EventInformation = AnimationEventInfo.ParseEventInfoFromEventArgs((AnimationEventArgs)eventBase);
            }
        }

        public static List<EventInstruction> GetInstructionsFromList(List<GameEventArgs> lsit)
        {
            
            if (lsit == null)
                return null;

            List<EventInstruction> result = new List<EventInstruction>();

            foreach ( var item in lsit){
                result.Add(new EventInstruction(item));
            }

            return result;
        }

        public static List<GameEventArgs> GetGameEventArgsListFromEventInstructions(List<EventInstruction> eventList)
        {
            if (eventList == null)
                return null;

            List<GameEventArgs> eventArgs = new List<GameEventArgs>();
            foreach ( var eventInstr in eventList)
            {
                        
                if (eventInstr.TypeOfEvent == EventType.Animation)
                {
                    var eventInfo = eventInstr.EventInformation;
                    if (eventInfo.GetType() == typeof(JObject))
                        eventInfo = ((JObject)eventInfo).ToObject<AnimationEventInfo>();

                    var item = eventInstr.EventInformation.GetType();
                    var eventAnim = AnimationEventInfo.ParseEventArgsFromAnimationEventInfo((AnimationEventInfo)eventInfo);
                    eventArgs.Add(eventAnim);
                }
                if (eventInstr.TypeOfEvent == EventType.GameEvent)
                {
                    var eventInfo = eventInstr.EventInformation;
                    if (eventInfo.GetType() == typeof(JObject))
                    eventInfo = ((JObject)eventInfo).ToObject<GameEventInfo>();

                    var item = eventInstr.EventInformation.GetType();
                    var eventAnim = GameEventInfo.ParseEventArgsFromAnimationEventInfo((GameEventInfo)eventInfo);
                    List<object> objs = new List<object>();
                    eventArgs.Add(eventAnim);
                }    
            }

            return eventArgs;

        }

        public EventInstruction(){}
    }

    public class AnimationEventInfo
    {
        public string AnimationPlayerName;

        public string AnimationName;

        public float AnimationLength;

        public int AnimationSpeed;

        public List<Argument> AnimationArguments; 

        public string TargetClass;

        public DateTime TimeStamp;

        public bool PlayBackwards;

        public static AnimationEventArgs ParseEventArgsFromAnimationEventInfo(AnimationEventInfo eventInfo)
        {
            return new AnimationEventArgs(eventInfo.TimeStamp)
            {
                AnimationPlayerName =   eventInfo.AnimationPlayerName,
                AnimationName       =   eventInfo.AnimationName,
                AnimationLength     =   eventInfo.AnimationLength,
                AnimationSpeed      =   eventInfo.AnimationSpeed,
                AnimationArguments  =   eventInfo.AnimationArguments,
                TargetClass         =   eventInfo.TargetClass,
                PlayBackwards       =   eventInfo.PlayBackwards
            };
        }

        public static AnimationEventInfo ParseEventInfoFromEventArgs (AnimationEventArgs eventBase)
        {
            return new AnimationEventInfo()
            {
                AnimationPlayerName =   eventBase.AnimationPlayerName,
                AnimationName       =   eventBase.AnimationName,
                AnimationLength     =   eventBase.AnimationLength,
                AnimationSpeed      =   eventBase.AnimationSpeed,
                AnimationArguments  =   eventBase.AnimationArguments,
                TargetClass         =   eventBase.TargetClass,
                TimeStamp           =   eventBase.TimeStamp,
                PlayBackwards       =   eventBase.PlayBackwards
            };
        }
    }

    public class GameEventInfo
    {
        public string TargetClass;

        public EventMethodInformation MethodInformation;

        public string TypeID;

        public string PropertyName;

        public List<Argument> Arguments;

        public DateTime TimeStamp;

        public static GameStateEventArgs ParseEventArgsFromAnimationEventInfo(GameEventInfo eventInfo)
        {
            return new GameStateEventArgs(eventInfo.TimeStamp)
            {
                TargetClass = eventInfo.TargetClass,
                MethodName = eventInfo.MethodInformation?.MethodName,
                MethodArguments = new GameStateEventArgs.MethodArgumenter(eventInfo.MethodInformation?.MethodArguments),
                TypeID = eventInfo.TypeID, 
                PropertyName = eventInfo.PropertyName,
                Arguments = eventInfo.Arguments
            };
        }

        public static GameEventInfo ParseEventInfoFromEventArgs (GameStateEventArgs eventBase)
        {
            return new GameEventInfo()
            {
                TargetClass = eventBase.TargetClass,
                MethodInformation = new EventMethodInformation()
                {
                    MethodName = eventBase.MethodName,
                    MethodArguments = eventBase.MethodArguments.ToArray()
                },
                TypeID = eventBase.TypeID, 
                PropertyName = eventBase.PropertyName,
                Arguments = eventBase.Arguments,
                TimeStamp = eventBase.TimeStamp
            };
        }
    }

    public class EventMethodInformation
    {
        public string MethodName;

        public GameStateEventArgs.MethodArgument[] MethodArguments;
    }

    public enum EventType{
        Animation,
        GameEvent
    }
}
