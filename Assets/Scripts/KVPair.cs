// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using UnityEngine;

[System.Serializable]
public class KVPair<TKey, TValue>
{
    public KVPair()
    {
    }

    public KVPair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    [field: SerializeField] public TKey Key { set; get; }
    [field: SerializeField] public TValue Value { set; get; }
}
