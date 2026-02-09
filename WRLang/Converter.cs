using System.Collections.ObjectModel;

namespace WRLang {
    public class Converter {
        public static bool UpdateTextById(int id, string newText, ObservableCollection<Translation> translations) {
            if (translations.FirstOrDefault(t => t?.Id == id) is not { } target) return false;
            target.Original = target.Text;
            target.Text = newText;
            return true;
        }
    }
}
