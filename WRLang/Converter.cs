using System.Collections.ObjectModel;

namespace WRLang {
    public class Converter {
        public static bool UpdateTextById(int id, string newText, ObservableCollection<Translation> translations) {
            if (translations.FirstOrDefault(t => t?.Id == id) is not { } target) {
                target = new Translation {
                    Text = newText,
                    Id = id
                };
                translations.Add(target);
                return true;
            }
            target.Original = target.Text;
            target.Text = newText;
            
            return true;
        }
    }
}
