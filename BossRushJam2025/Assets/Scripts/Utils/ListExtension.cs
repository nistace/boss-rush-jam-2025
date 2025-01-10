using System.Collections.Generic;

namespace Utils {
   public static class ListExtension {
      public static int IndexOf<T>(this IReadOnlyList<T> list, T item) {
         for (var i = 0; i < list.Count; ++i) {
            if (list[i] is null && item is null || list[i].Equals(item)) return i;
         }
         return -1;
      }
   }
}