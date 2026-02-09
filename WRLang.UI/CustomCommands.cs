using System.Windows.Input;

namespace WRLang.UI {
    public static class CustomCommands {
        public static readonly RoutedUICommand Convert = new(
            "Convert",
            "Convert",
            typeof(CustomCommands),
            []
        );
    }
}