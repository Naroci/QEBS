using System;
using System.ComponentModel;
using System.Linq;

namespace QEBS.Base;
public static class BaseGameEventListener 
{
	public static bool GetIfEventIsRelevant(object sender, GameEventArgs EventArgsItem, string[] TargetClassToListen = null)
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
}
