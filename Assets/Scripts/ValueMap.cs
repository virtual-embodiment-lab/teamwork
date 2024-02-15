using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ValueMap<TKey, TValue>
{
    [SerializeField]
    protected List<KVPair<TKey, TValue>> Attributes = null;
    private Dictionary<TKey, int> Mapper = null;

    public ValueMap()
    {
        Attributes = new List<KVPair<TKey, TValue>>();
        Mapper = new Dictionary<TKey, int>();
    }

    /* Update or Create an item in the attributes array. Whenever the value
     * changes this will be called with the new keyvaluepair to update the
     * attribute. This can also be used to create a new value. */
    public void UpdateOrCreate(KVPair<TKey, TValue> input)
    {
        if (Mapper.ContainsKey(input.Key))
        {
            int index = Mapper[input.Key];
            Attributes[index] = input;

        }
        else
        {
            Attributes.Add(input);
            Mapper.Add(input.Key, Attributes.IndexOf(input));
        }
    }

    /* Returns the list of attributes */
    public List<KVPair<TKey, TValue>> GetAttributes()
    {
        return Attributes;
    }

    public override string ToString()
    {
        return string.Join(",", Attributes.Select(t => string.Format("{0}, {1}", t.Key, t.Value)));
    }

    public string ToLine(char delimiter)
    {
        return string.Join(delimiter, Attributes.Select(t => string.Format("{0}:{1}", t.Key, t.Value)));
    }
    
}
