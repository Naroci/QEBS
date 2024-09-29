using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QEBS.Base
{
	public abstract class BaseEventNodeSubcriber
	{
		public bool RelistenAfterEvent = false;

		public virtual string[] ForeignClassListeners 
		{
			get 
			{
				return null;
			}
		}

		public virtual string[] ValuesNeeded
		{
			get 
			{
				return null;
			}
		}
		
		private NodeGameEventManager _GameManager = null;
		
		public List<GameStateEventArgs> ReceivedEvents = null;

		// Called when the node enters the scene tree for the first time.
		// vielleicht lieber als constructor call oder so...
		public void _Ready()
		{
			try
			{
				this._GameManager = Instance.GetCurrentGameEventManager();
				this._GameManager.GameStateChanged += GameStateChanged; 
			}
			catch (Exception e)
			{
				Console.WriteLine($"[EXCEPTION] Exception in _Ready(BaseEventNoteSubscriber) Occured! : {e.Message}");
				//GD.PrintErr(string.Format("[EXCEPTION] Exception in _Ready(BaseEventNoteSubscriber) Occured! : {0}", e.Message));
			}
		}

		// Ambesten als disposeable als dispose oder so callen, sollte bei c# universal (hoffentlich) funktionieren.
		public void _ExitTree()
		{
			this._GameManager.GameStateChanged -= GameStateChanged;
		}

		public void GameStateChanged (object sender, GameStateEventArgs e)
		{
			try{
					if (e != null 
				
					&& BaseGameEventListener.GetIfEventIsRelevant(this,e,this.ForeignClassListeners)
					&& e.Handled == false 
					&& e.GetType() == typeof(GameStateEventArgs) 
					&& ((GameStateEventArgs)e).TargetClass == this.GetType().Name)
				{
					var calledValue = (GameStateEventArgs)e;
					e.Handled = true;
					calledValue.Completed = false;

					if (calledValue.MethodName != null)
					{
						Type thisType = this.GetType();
						MethodInfo theMethod = thisType.GetMethod(calledValue.MethodName);
						if (theMethod != null)
						{
							if (this.ReceivedEvents == null)
								this.ReceivedEvents = new List<GameStateEventArgs>();
							try{
								this.ReceivedEvents.Add(calledValue);
								var args = calledValue.MethodArguments?.TypedMethodArguments;
								theMethod.Invoke(this, args);
							}
							catch(Exception ex){
								Console.WriteLine(ex.Message);
							}
						}
					}
				}
			}
			catch(Exception err)
			{
				Console.WriteLine("Error occured in BaseEventNodeSubcriber.GameStateChanged");
				Console.WriteLine(err.Message);
				
				//GD.Print(string.Format("Error occured in BaseEventNodeSubcriber.GameStateChanged"));
				//GD.Print(string.Format(err.Message));
			}
			
		
		}

		public void FinishedEventCall(string MethodName)
		{
			var start = DateTime.UtcNow.Second;
			try
			{
				if (this.ReceivedEvents == null)
					return;

				if (this.ReceivedEvents.Exists(x=>x.MethodName.ToLower().Equals(MethodName.ToLower()) && x.Completed == false))
				{
					var containingEvents = this.ReceivedEvents.Where(x=>x.MethodName.ToLower().Equals(MethodName.ToLower()))?.ToList();
					var results = containingEvents != null ? this.ReceivedEvents.Where(x=>x.Completed == false)?.ToList() : null;
					if (results != null)
					{
						var eve = results.First();
						//eve.Handled = true;
						Instance.GetCurrentGameEventManager().EventArgsCompleted(this,eve);
						//this.ReceivedEvents = this.ReceivedEvents.Where(x=>x.Completed == false).ToList();
						//eve.Completed = true;
						
						Console.WriteLine($"[CLIENT] - Finished Call {MethodName}");
						
						//GD.Print(string.Format("[CLIENT] - Finished Call {0}",MethodName));
						//Console.WriteLine(string.Format("Finished Call {0}",MethodName));
						
						/* if (RelistenAfterEvent)
								await GameInstance.GetHTTPConnection().Listen(); */
					}
				/* 		else
					{
						Console.WriteLine(string.Format("[CLIENT] - ({0}) Call for {1} not found",this.GetType().Name,MethodName));
						if (this.ReceivedEvents != null && this.ReceivedEvents.Count > 0 && this.ReceivedEvents.Exists(x=>x.Completed == false))
						Console.WriteLine(string.Format("[CLIENT] - CURRENT CALL {0}",this.ReceivedEvents.First(x=>x.Completed == false).MethodName));
					} */
					
				}
			
			}
			catch(Exception e)
			{
				//Console.WriteLine("[CLIENT] - Error while Calling Finish Event!");
				Console.WriteLine($"[EXCEPTION] Exception in FinishedEventCall Occured! : {e.Message}");
				//GD.PrintErr(string.Format("[EXCEPTION] Exception in FinishedEventCall Occured! : {0}", e.Message));
			}
			var end = DateTime.UtcNow.Second;
			// -- >  DEF need to change this! this._Process((float)(end - start));
		
		}


		//  // Called every frame. 'delta' is the elapsed time since the previous frame.
		//  public override void _Process(float delta)
		//  {
		//      
		//  }
	}
}


