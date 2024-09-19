// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

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
