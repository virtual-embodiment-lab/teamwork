using Normal.Realtime;

[RealtimeModel]
public partial class SpawnPointModel
{
    [RealtimeProperty(1, true, true)]
    private bool _isOccupied;

    [RealtimeProperty(2, true, true)]
    private int _occupyingClientID;
}
