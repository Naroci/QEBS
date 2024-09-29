namespace QEBS.Base
{
    public enum GameStateActionState 
    {
        Uncompleted,
        Completed,
        ForeignRequest,
        Killed
    }

    public enum SendState
    {
        None,
        Requested,
        Sending,
        Sent,
        Failed,
    }
}