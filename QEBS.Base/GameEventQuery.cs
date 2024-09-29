using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QEBS.Base
{
    public class GameEventQuery
    {

        //GameEventArgs CurrentEvent = null;
        //private bool CurretnTaskRunning = false;

        private BlockingCollection<GameEventArgs> EventQueue;

        public GameEventQuery() {
            this.EventQueue = new BlockingCollection<GameEventArgs>();
        }

        private GameEventArgs currentItem
        {
            get
            {
                 if (this.EventQueue != null && EventQueue.Count > 0
                    && this.EventQueue.ToList().Exists(x=>x?.Completed == false))
                 {
                    var temp = this.EventQueue.Where(x=>x?.Completed == false).ToList();
                    var itm = temp.OrderBy(x=>x?.TimeStamp.Ticks)?.First();   

                   /*  if (itm != null && itm.GetType() == typeof(AnimationEventArgs))
                    {
                            AnimationEventArgs CurrEvent = (AnimationEventArgs)itm;
                            GD.Print(string.Format("[CLIENT] - CURRENT ITEM : ANIMNAME {0}, ANIMPLAYER {1}", CurrEvent.AnimationName, CurrEvent.AnimationPlayerName));
                    }
                    if (itm != null && itm.GetType() == typeof(GameStateEventArgs))
                    {
                            GameStateEventArgs CurrEvent =1 (GameStateEventArgs)itm;
                            GD.Print(string.Format("[CLIENT] - CURRENT ITEM : TCLASS {0}, TMETHOD {1}", CurrEvent.TargetClass, CurrEvent.MethodName));
                            if (CurrEvent.MethodArguments != null){
                                foreach (var arg in CurrEvent.MethodArguments)
                                 GD.Print(string.Format("[CLIENT] - ARGUMENT {0}", arg.ToString()));
                            }
                    } */

                    
                    return itm;   
                 }
                 return null;
            }
           
        }

        public GameEventArgs GetCurrentItem()
        {
            return this.currentItem;
        }

        public List<GameEventArgs> GetQuery() 
        {
            return this.EventQueue.ToList();
        }

        public void AddToQuery (object Event)
        {
            try{
                Type defaultType = typeof(GameEventArgs);
                if (this.EventQueue != null &&  this.EventQueue.Count > 100){
                      var temp = this.EventQueue.Where(x=>x?.Completed == false);
                      this.EventQueue = new BlockingCollection<GameEventArgs>();
                        foreach(var item in temp){
                            this.EventQueue.Add(item);  
                        }
                }
                    

                if (Event != null)
                {
                    lock(Event)
                    {
                        var type = Event.GetType();
                        if (type == typeof(List<GameEventArgs>))
                        {
                            foreach (var item in (List<GameEventArgs>)Event)
                                this.EventQueue.Add(item); 
                        }
                        if (type != typeof(List<GameEventArgs>) && Event.GetType().BaseType == typeof(GameEventArgs))
                        {
                            this.EventQueue.Add((GameEventArgs)Event); 
                        }
                        //this.EventQueue = new BlockingCollection<GameEventArgs>( Sort(EventQueue.ToList()));
                    }
                

                /*  if (this.currentItem == null && this.EventQueue[0] != null)
                        this.currentItem = this.EventQueue[0]; */
                }
            }
            catch(Exception e){
                GD.Print("Error while AddToQuery");
                 Console.WriteLine(string.Format("ERROR WHILE ADDTOQUERY REQUEST!\n: {0}",e.Message));
            }
         
        }

        private List<GameEventArgs> Sort(List<GameEventArgs> eventQueue)
        {
            try{
                var newList = this.EventQueue.Where(x=>!x.Completed).ToList();
                newList = newList.OrderBy(x=>x?.TimeStamp.Ticks).ToList();
                return newList;
            }
            catch(Exception e){
                 Console.WriteLine(string.Format("[EXCEPTION] Exception in GameEventQuery Occured! : {0}", e.Message));
                return null;
            }

        }

        public bool ItemExists(GameEventArgs theEvent){
            if (this.EventQueue == null)
                return false;
            
            return this.EventQueue.ToList().Contains(theEvent);
        }

        public bool RemoveItemFromList(GameEventArgs theEvent)
        {
            try{
                lock(theEvent)
                {
                    theEvent.Handled = true;
                    theEvent.Completed =  true;

                    var index = this.EventQueue.ToList().IndexOf(theEvent);
                    if (index < 0)
                        return false;


                    this.EventQueue.ToList()[index] = theEvent;

                    /*  if (theEvent != null && theEvent.GetType() == typeof(AnimationEventArgs))
                    {
                            AnimationEventArgs CurrEvent = (AnimationEventArgs)theEvent;
                            GD.Print(string.Format("[CLIENT] - COMPLETED ITEM : ANIMNAME {0}, ANIMPLAYER {1}", CurrEvent.AnimationName, CurrEvent.AnimationPlayerName));
                    }
                    if (theEvent != null && theEvent.GetType() == typeof(GameStateEventArgs))
                    {
                            GameStateEventArgs CurrEvent = (GameStateEventArgs)theEvent;
                            GD.Print(string.Format("[CLIENT] - COMPLETED ITEM : TCLASS {0}, TMETHOD {1}", CurrEvent.TargetClass, CurrEvent.MethodName));
                            if (CurrEvent.MethodArguments != null){
                                foreach (var arg in CurrEvent.MethodArguments)
                                 GD.Print(string.Format("[CLIENT] - ARGUMENT {0}", arg.ToString()));
                            }
                    } */

                /*     this.EventQueue = Sort(this.EventQueue);
                    if (this.EventQueue.Count > 0)
                        this.currentItem = this.EventQueue[0];
                    else
                        this.currentItem = null; */

                    return true;

                }
             
            }
            catch(Exception e)
            {
                  Console.WriteLine(string.Format("[EXCEPTION] Exception in GameEventQuery (RemoveItemFromList) Occured! : {0}", e.Message));
                return false;
            }
           
        }

       

    }

   
}
