using Normal.Realtime;

[RealtimeModel]
public partial class GameModel
{
    [RealtimeProperty(1, true, true)]
    private int _coinsCollected;

    [RealtimeProperty(2, true, true)]
    private float _gameTime;
}