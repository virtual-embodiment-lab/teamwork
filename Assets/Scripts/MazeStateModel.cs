using Normal.Realtime;

[RealtimeModel]
public partial class MazeStateModel
{
    [RealtimeProperty(1, true, true)]
    private int _activeMaze;
}
