using Normal.Realtime;

[RealtimeModel]
public partial class PlayerModel
{
    [RealtimeProperty(1, true, true)]
    private int _role;
}
