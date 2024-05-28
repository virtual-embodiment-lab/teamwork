using Normal.Realtime;

[RealtimeModel]
public partial class DoorModel
{
    [RealtimeProperty(1, true, true)]
    private bool _isOpen;
}
