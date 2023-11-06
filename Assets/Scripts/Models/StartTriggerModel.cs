using Normal.Realtime;

[RealtimeModel]
public partial class StartTriggerModel
{
    [RealtimeProperty(1, true, true)]
    private bool _started;
}
