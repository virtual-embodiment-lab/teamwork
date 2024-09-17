using Normal.Realtime;

[RealtimeModel]
public partial class CoinModel
{
    [RealtimeProperty(1, true, true)]
    private bool _found;

    [RealtimeProperty(2, true, true)]
    private bool _collected;

    [RealtimeProperty(3, true, true)]
    private int _parentID;
}
