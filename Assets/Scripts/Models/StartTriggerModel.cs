// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Normal.Realtime;

[RealtimeModel]
public partial class StartTriggerModel
{
    [RealtimeProperty(1, true, true)]
    private bool _started;
}
