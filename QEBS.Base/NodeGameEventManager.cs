using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace QEBS.Base;

    public class NodeGameEventManager
    {
        bool waiting = false;
        private const int TimerTickRate = 30;
        
        public float PullRate;

        private float Ticks = 0;

        //GameEventArgs CurrentEvent = null;
        //private bool CurretnTaskRunning = false;

        System.Threading.Thread EventTimerM = null;

        //System.Timers.Timer EventTimer;

        private GameEventQuery queryListManager = null;

        //GameEventArgs currentItem = null;

        public override void _Ready()
        {
            base._Ready();
        }

        public NodeGameEventManager()
        {
            SetEventRegisterAllowed(true);
            this.queryListManager = new GameEventQuery();
           /*  

            this.EventTimerM = new System.Threading.Thread(Run);
            this.EventTimerM.IsBackground = true;
            this.EventTimerM.Start(); */
            //this.EventTimer = new System.Timers.Timer(TimerTickRate);
           
            //this.EventTimer.Elapsed += OnTimedEvent;
            //this.EventTimer.Enabled = true;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            this.Ticks += delta;
            if (this.Ticks > this.PullRate)
            {
                this.Ticks = 0;
                 CheckCurrentQueue();
            }
           
        }

    // Hier Animationen anmelden, die von einem AnimationsManager abgehört werden und je nach Animationsbeschreibung ausgelöst werden.


        // Hier Animationen anmelden, die von einem AnimationsManager abgehört werden und je nach Animationsbeschreibung ausgelöst werden.
        public event EventHandler<AnimationEventArgs> AnimationEventReceived;

        // Innerhalb des Eventargs das betroffende Object / bzw. eine Objekt ID oder ähnliches angeben, damit das entsprechende Objekt lauschen und Reagieren kann.
        public event EventHandler<GameStateEventArgs> GameStateChanged;

        public List<GameEventArgs> GetRemainingEvents()
        {
            return this.queryListManager.GetQuery();
        }

        private void FireGameState(object sender, GameStateEventArgs e)
        {
            lock(e)
            {
                EventHandler<GameStateEventArgs> eventHandler = GameStateChanged;
                eventHandler?.Invoke(this, e);
            }
        }

        private void FireAnimationEvent(object sender, AnimationEventArgs e)
        {
            lock(e)
            {
                EventHandler<AnimationEventArgs> eventHandler = AnimationEventReceived;
                eventHandler?.Invoke(this, e);
            }
      
        }

        private bool _EventRegisteringAllowed = false;

        public void SetEventRegisterAllowed(bool Allow){
            this._EventRegisteringAllowed = Allow;
        }

        public bool GetIfEventRegisterIsAllowed(){
            return this._EventRegisteringAllowed;
        }

        public void Register(object sender, object Event)
        {
            Type defaultType = typeof(GameEventArgs);
            if (Event != null && GetIfEventRegisterIsAllowed() == true)
            {
                if (Event.GetType() == typeof(AnimationEventArgs) && ((AnimationEventArgs)Event).GetIfIsInstantEvent() == true 
                            || Event.GetType() == typeof(GameStateEventArgs) && ((GameStateEventArgs)Event).GetIfIsInstantEvent() == true)
                {
                        FireInstantEvent(Event);
                }
                else if (Event.GetType() != defaultType)
                {
                    this.queryListManager.AddToQuery(Event);

                    /* if(this.EventTimer == null || this.EventTimer != null && !this.EventTimer.Enabled)
                        ResetEventTimer(); */
                }
            }
        }

        private void FireInstantEvent(object currentItem)
        {
            if (currentItem != null && currentItem.GetType() == typeof(AnimationEventArgs))
            {
                AnimationEventArgs newEvent = (AnimationEventArgs)currentItem;
                newEvent.Completed = true;
                FireAnimationEvent(this, newEvent);
            }
            if (currentItem != null && currentItem.GetType() == typeof(GameStateEventArgs))
            {
                GameStateEventArgs newEvent = (GameStateEventArgs)currentItem;
                newEvent.Completed = true;
                FireGameState(this,newEvent);
            }
        }

        private void ResetEventTimer()
        {
          /*   this.EventTimer = new System.Timers.Timer(TimerTickRate);
            this.EventTimer.Elapsed += OnTimedEvent;
            this.EventTimer.Start(); */
        }

      /*   private  void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckCurrentQueue();
        }

        private void Run()
        {
            double framTime = 1000 / Godot.Engine.TargetFps;
            while(!GameInstance.QuitGame)
            {
                double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                CheckCurrentQueue();
                double endTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                
                System.Threading.Thread.Sleep(TimerTickRate);
            }
        }  */

        private void PrintEventInfo(object item, string Text) 
        {
            if (item.GetType() == typeof(AnimationEventArgs))
            {
                GD.Print(
                            string.Format(
                            "({0}) - {1}, AnimationName: {2}, AnimationPlayer: {3}",
                            item.GetType().ToString(),
                            Text,
                            ((AnimationEventArgs)item).AnimationName,
                            ((AnimationEventArgs)item).AnimationPlayerName)
                        );
            }
            if (item.GetType() == typeof(GameStateEventArgs))
            {
                GD.Print(
                            string.Format(
                            item.GetType().ToString(),
                            "({0}) - {1}, TargetClass: {3}",
                            Text,
                            //((GameStateEventArgs)item).ObjectsName,
                            ((GameStateEventArgs)item).TargetClass)
                        );
            }
        }

        private void CheckCurrentQueue() 
        {

            try
            {
                var currentItem = queryListManager.GetCurrentItem();
                bool fired = false;
                //GD.Print("Checking Queue...");
                if ( currentItem != null && currentItem.Initiated  == false && currentItem.Completed ==false) 
                {
                    //CurretnTaskRunning = true;
                    currentItem.Initiated = true;
                    waiting = true;
                    //this.currentItem.Completed = false;

                        if (currentItem != null && currentItem.GetType() == typeof(AnimationEventArgs))
                        {
                            AnimationEventArgs newEvent = (AnimationEventArgs)currentItem;
                            newEvent.Completed = false;
                            FireAnimationEvent(this, newEvent);
                
                            fired = true;
                        }
                        if (currentItem != null && currentItem.GetType() == typeof(GameStateEventArgs))
                        {
                            GameStateEventArgs newEvent = (GameStateEventArgs)currentItem;
                            newEvent.Completed = false;
                            FireGameState(this,newEvent);
                            
                            fired = true;
                        }
                        if (!fired){
                            GD.Print("COULD NOT IDENTIFY GAME EVENT ARGS ! - EVENT NOT FIRED!!!");
                        } 
                }
                if (currentItem != null && currentItem.Initiated == true && currentItem.Completed == false)
                {
                    float AnimTime = 0;
                    if (currentItem is AnimationEventArgs)
                        AnimTime = ((AnimationEventArgs)currentItem).AnimationLength;

                    var TimeDiff = (DateTime.UtcNow - currentItem.GetInitiationTime());
                    if (TimeDiff.Seconds > 2 && (float)TimeDiff.Seconds > Math.Round(AnimTime +1)){
                            GD.Print(string.Format("Throwing unfinished Event! ({0} Seconds) , (Targetclassname: {1})",TimeDiff.Seconds.ToString(), currentItem.TargetClass?.ToString()));
                            EventArgsCompleted(this,currentItem);
                    }
                   

                }
                else if (currentItem == null && waiting == true || currentItem != null && currentItem.Completed && waiting == true)
                    waiting = false;
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("[EXCEPTION] Exception in GameEventManager Occured! : {0}", e.Message));
            }
           
        }

        public void EventArgsCompleted (object sender,GameEventArgs theEvent)
        {
            lock(theEvent)
            {
                if (theEvent != null && this.queryListManager != null && this.queryListManager.ItemExists(theEvent))
                {
                    //GD.Print("Removing Item from Queue");
                    if (this.queryListManager.RemoveItemFromList(theEvent))
                    {
                        waiting = false;

                        if(this.EventTimerM == null || this.EventTimerM != null && !this.EventTimerM.IsAlive)
                            ResetEventTimer();
                        
                        //CheckCurrentQueue();
                    }         
                }
                else{
                    waiting = false;
                    GD.Print("MISSMATCHING EVENT ORDER FOUND!!!");
                }
            }
            
               
          /*   if (this.EventQueue?.Count > -1){
               
                GD.Print(string.Format("Item Count afterwards: {0}",this.EventQueue.Count));
                // PrintEventInfo(theEvent,"Item: ");
            } */
        }

      /*   private bool removeItemFromList(GameEventArgs theEvent)
        {
            try{
                var index = this.EventQueue.IndexOf(theEvent);
                if (index < 0)
                    return false;
                
                this.EventQueue[index].Completed = true;

                var newList = this.EventQueue.Where(x=>!x.Completed).ToList();
                newList = newList.OrderBy(x=>x?.TimeStamp.Ticks).ToList();

                this.EventQueue = newList;
                return true;
            }
            catch(Exception e){
                return false;
            }
           
        } */

       

    }

   

