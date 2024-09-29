using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace QEBS.Base
{
	public class GameEventListener 
	{
		private string[] ForeignClassListeners 
		{
			get;set;
		}

		object usedByClass = null;
		public GameEventListener (object CallerClass,bool RelistenAfterEvent, string[] ForeignClassListener)
		{
			this.ForeignClassListeners = ForeignClassListener;
			usedByClass = CallerClass;
			this.RelistenAfterEvent = RelistenAfterEvent;
		}

		private bool RelistenAfterEvent = false;

		public List<GameStateEventArgs> ReceivedEvents = null;

		public bool GetIfEventIsRelevant (GameEventArgs EventArgsItem, string[] TargetClassToListen = null)
		{
			return GetIfEventIsRelevant(this.usedByClass,EventArgsItem, TargetClassToListen);
		}

		public void GameStateChanged (object sender, GameStateEventArgs e)
		{
			if (e != null 
				&& GetIfEventIsRelevant(sender,e,this.ForeignClassListeners)
				&& !e.Handled && e.GetType() == typeof(GameStateEventArgs) 
				&& ((GameStateEventArgs)e).TargetClass == this.GetType().Name)
			{
				e.Handled = true;
				var calledValue = (GameStateEventArgs)e;
				if (this.ReceivedEvents == null)
					this.ReceivedEvents = new List<GameStateEventArgs>();

				this.ReceivedEvents.Add(calledValue);
				if (calledValue.MethodName != null)
				{
					Type thisType = sender.GetType();
					MethodInfo theMethod = thisType.GetMethod(calledValue.MethodName);
					var args = calledValue.MethodArguments.TypedMethodArguments;
					theMethod.Invoke(sender, args);
				}
			}
		}

		public bool GetIfEventIsRelevant(object sender, GameEventArgs EventArgsItem, string[] TargetClassToListen = null)
		{
			var className = TypeDescriptor.GetClassName(sender);
			var senderBaseType = sender.GetType().BaseType;

			if (EventArgsItem.GetType() == typeof(AnimationEventArgs) && 
				senderBaseType == typeof(AnimationPlayer) &&
				((AnimationEventArgs) EventArgsItem).AnimationPlayerName == ((AnimationPlayer)sender).Name)
			{
				return true;
			}

			if (EventArgsItem.GetType() == typeof(GameStateEventArgs) && 
				((GameStateEventArgs) EventArgsItem).TargetClass == className || 
				EventArgsItem.GetType() == typeof(GameStateEventArgs) && 
				TargetClassToListen != null && TargetClassToListen.Contains(((GameStateEventArgs) EventArgsItem).TargetClass))
			{
				return true;
			}
			return false;
		}

		public void FinishedEventCall(string MethodName)
		{
			if (this.ReceivedEvents != null && this.ReceivedEvents.Count > 0 && this.ReceivedEvents.Exists(x=>x.MethodName == MethodName && !x.Completed && !x.Handled))
			{
				var eve = this.ReceivedEvents.First(x=>x.MethodName == MethodName && !x.Completed);
				eve.Handled = true;
				Instance.GetCurrentGameEventManager().EventArgsCompleted(this.usedByClass,eve);

				/* if (RelistenAfterEvent)
				  await GameInstance.GetHTTPConnection().Listen(); */
			}
		}
	}

}

