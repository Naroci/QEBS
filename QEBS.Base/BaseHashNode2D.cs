using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QEBS.Base;

public abstract class BaseHashNode2D 
{
    public string HashIdentifier;

	public void GenerateAndSetHashIdentifier()
	{
		this.HashIdentifier = this.NativeInstance.ToString();
	}

    public override void _Ready()
    {
        try
		{
			if (string.IsNullOrEmpty(this.HashIdentifier))
				GenerateAndSetHashIdentifier();
				
			this._GameManager = Instance.GetCurrentGameEventManager();
			/*if (this._GameManager == null)
			{
					var EventManager = GetNode<NodeGameEventManager>("/root/NodeEventManager");
					Instance.SetGameEventManager(EventManager);
					if (EventManager != null)
						this._GameManager = GameInstance.GetCurrentGameEventManager();
			}*/
			this._GameManager.GameStateChanged += GameStateChanged; 
		}
		catch (Exception e)
		{
			GD.PrintErr(string.Format("[EXCEPTION] Exception at _Ready (in BaseHashNode2D) Occured! : {0}", e.Message));
		}
    }


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

	public override void _ExitTree()
	{
		if (this._GameManager != null)
			this._GameManager.GameStateChanged -= GameStateChanged;
	}

	public void GameStateChanged (object sender, GameStateEventArgs e)
	{
		try{
			if (e != null 
			
			&& BaseGameEventListener.GetIfEventIsRelevant(this,e,this.ForeignClassListeners)
			&& !e.Handled && e.GetType() == typeof(GameStateEventArgs) 
			&& ((GameStateEventArgs)e).TargetClass == this.GetType().Name)
			{
				//e.Handled = true;
				var calledValue = (GameStateEventArgs)e;
				if (this.ReceivedEvents == null)
					this.ReceivedEvents = new List<GameStateEventArgs>();

				calledValue.Completed = false;
				this.ReceivedEvents.Add(calledValue);
				if (calledValue.MethodName != null)
				{
					Type thisType = this.GetType();
					MethodInfo theMethod = thisType.GetMethod(calledValue.MethodName);
					var args = calledValue.MethodArguments.TypedMethodArguments;
					theMethod.Invoke(this, args);
				}
			}
		}
		catch(Exception err)
		{
			GD.Print(string.Format("Error occured in BashHashNode2D.GameStateChanged"));
			GD.Print(string.Format(err.Message));
		}
		
	}

	public void FinishedEventCall(string MethodName)
	{
		var start = DateTime.UtcNow.Second;
		if (this.ReceivedEvents != null && this.ReceivedEvents.Count > 0 && this.ReceivedEvents.Exists(x=>x.MethodName == MethodName && !x.Completed ))
		{
			var eve = this.ReceivedEvents.First(x=>x.MethodName == MethodName && !x.Completed);
			//eve.Handled = true;
			Instance.GetCurrentGameEventManager().EventArgsCompleted(this,eve);
			//eve.Completed = true;
			//if (RelistenAfterEvent)
			//		GameInstance.GetTCPConnection().Listen();
		}
		var end = DateTime.UtcNow.Second;
		var delt = end - start;
		this._Process(delt);
	}



}
