using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KnockOff.Helpers;

public static class ExtensionMethods
{
    public static void Replace<T>(this ObservableCollection<T> source, IEnumerable<T> items)
    {
        source.Clear();
        foreach (var item in items) { source.Add(item); }
    }
}