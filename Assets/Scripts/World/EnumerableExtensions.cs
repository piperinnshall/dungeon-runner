using System.Collections.Generic;
using System.Linq;

// Extension method: adds indexed enumeration to any IEnumerable
public static class EnumerableExtensions {
  public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> source)
    => source.Select((item, i) => (item, i));
}
