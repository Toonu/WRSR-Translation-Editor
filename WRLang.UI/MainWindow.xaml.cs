using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace WRLang.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ObservableCollection<Translation> _translations = [];
        private readonly DataTable dataTable;
        private string _filePath;
        public string FilePath {
            get => _filePath;
            set {
                if (_filePath == value) return;
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            _filePath = "Translations.xlsx";
            try {
                dataTable = ExcelReader.ReadExcelToDataTable($"{AppContext.BaseDirectory}/{FilePath}");
            } catch (Exception) {
                dataTable = new DataTable();
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConvertCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            try {
                ExcelReader.ReadExcelToDataTable($"{AppContext.BaseDirectory}/{FilePath}");
            } catch (Exception err) {
                MessageBox.Show($"Translation File could not be loaded: {err.Message}");
                return;
            }
            if (dataTable.Rows.Count == 0) {
                MessageBox.Show($"Translation File could not be loaded!");
                return;
            }
            if (_translations.Count == 0) {
                MessageBox.Show($"No translation .btf was loaded!");
                return;
            }

            string failedLog = "";
            int completed = 0;
            Debug.WriteLine("Processing table: " + dataTable.TableName);

            foreach (DataRow row in dataTable.Rows) {
                try {
                    int id = int.Parse(row["ID"]?.ToString() ?? "");
                    string original = row["Original"]?.ToString() ?? "";
                    string translation = row["Translation"]?.ToString() ?? "";

                    bool wasConverted = Converter.UpdateTextById(id, translation, _translations);
                    if (wasConverted) completed++;
                    else failedLog += $"[{id}] {original ?? translation} Was no found!\n";
                } catch (Exception ex) {
                    failedLog += $"Row with Id {row["Id"]}: {ex.Message}\n";
                }
            }

            if (completed == dataTable.Rows.Count) MessageBox.Show("Conversion completed!"); 
            else if (completed == 0) MessageBox.Show("Conversion failed!");
            else MessageBox.Show($"Conversion partially completed! Failed entries:\n{failedLog}");
        }

        private void Convert_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            if (dataTable != null && dataTable.Rows.Count == 0 || _translations.Count == 0) e.CanExecute = false;
            else e.CanExecute = true;
        }

        private ICollectionView? _translationsView;
        public ICollectionView TranslationsView {
            get => _translationsView ??= new CollectionViewSource() {
                Source = _translations
            }.View;
        }

        private string? _currentFile;
        public string? CurrentFile {
            get => _currentFile;
            set {
                _currentFile = value;
                PropertyChanged?.Invoke(this, new(nameof(CurrentFile)));
            }
        }

        private void OpenCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            var fileBrowser = new OpenFileDialog() {
                Filter = ".btf file|*.btf"
            };

            if (fileBrowser.ShowDialog() == true) {
                var mergeResult = _translations.Count > 0 ? MessageBox.Show(
                    owner: this,
                    messageBoxText: "Merge with the existing entries?",
                    caption: "Merge",
                    button: MessageBoxButton.YesNoCancel,
                    icon: MessageBoxImage.Question
                ) : MessageBoxResult.No;

                if (mergeResult != MessageBoxResult.Cancel) {
                    Open(fileBrowser.FileName, mergeResult == MessageBoxResult.Yes);
                }
            }
        }

        private void SaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            Save(CurrentFile!);
        }

        private void SaveAsCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            var saveDialog = new SaveFileDialog() {
                Filter = ".btf file|*.btf"
            };

            if (saveDialog.ShowDialog() == true) {
                Save(saveDialog.FileName);
            }
        }

        private void Save_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
            e.CanExecute = !string.IsNullOrEmpty(CurrentFile);
        }

        private void Open(string path, bool merge = true) {
            Translation[] translations = [];

            try {
                if (merge) {
                    translations = [.. BTF.LoadBtf(path).UnionBy(_translations, t => t.Id)];
                } else {
                    translations = BTF.LoadBtf(path);
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }

            _translations.Clear();
            _translations.AddRange(translations);

            CurrentFile = path;
        }

        private void Save(string path) {
            try {
                BTF.SaveBtf(path, [.. _translations]);
                CurrentFile = path;
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        public string? Filter {
            set {
                TranslationsView.Filter = GetFilter(value!);
            }
        }

        private Predicate<object> GetFilter(string filter) {
            return (obj) => {
                var translation = (obj as Translation)!;

                return translation.Text.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                       (uint.TryParse(filter, out uint filterId) && filterId == translation.Id);
            };
        }
    }
}