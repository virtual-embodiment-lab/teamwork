using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class DoorModel
{
    [RealtimeProperty(1, true, true)]
    private bool _isOpen;

    [RealtimeProperty(2, true, true)]
    private int _opener;
}
