using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GPSTCPClient
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            items.ToList().ForEach(collection.Add);
        }
    }
}
