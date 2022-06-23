using System.Collections.Generic;

public static class Extensions
{
    public static void Set(this List<PropAdd> list, EPropAddSrcType srcType, PropAdd propAdd)
    {
        propAdd.srcType = srcType;
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].srcType == srcType)
            {
                list[i] = propAdd;
                return;
            }
        }
        list.Add(propAdd);
    }
    public static void Remove(this List<PropAdd> list, EPropAddSrcType srcType)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].srcType == srcType)
            {
                list.RemoveAt(i);
                return;
            }
        }
    }
}