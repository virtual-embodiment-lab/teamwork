// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

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
