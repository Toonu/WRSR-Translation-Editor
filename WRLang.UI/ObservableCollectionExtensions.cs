using System.Collections.ObjectModel;

namespace WRLang.UI {
    static class ObservableCollectionExtension {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items) {
            foreach (var item in items) {
                collection.Add(item);
            }
        }
    }
}
