namespace QEBS.Base;

public class Instance
{
    private static NodeGameEventManager _EventManager = null;
    
    /// <summary>
    /// Gibt die aktuelle Instanz des GameEventManagers zurück, der z.B. Animationen und ähnliches auslöst.
    /// </summary>
    /// <returns></returns>
    public static NodeGameEventManager GetCurrentGameEventManager()
    {
        if (_EventManager == null)
        {
            _EventManager = new NodeGameEventManager();
        }
        return _EventManager;
    }
}