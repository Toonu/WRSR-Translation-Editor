using ExcelDataReader;
using System.Data;
using System.Text;

namespace WRLang {
    public static class ExcelReader {
        public static DataTable ReadExcelToDataTable(string filePath, string? sheetName = "media_soviet") {
            try {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                var config = new ExcelReaderConfiguration {
                    // 1252 = Western European (Windows) – common fallback for old Western files
                    FallbackEncoding = Encoding.GetEncoding(1252),
                };

                using var reader = ExcelReaderFactory.CreateReader(stream, config);

                var datasetConfig = new ExcelDataSetConfiguration {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration {
                        UseHeaderRow = true
                    }
                };

                var dataSet = reader.AsDataSet(datasetConfig);

                // Safely get the requested sheet or fall back to first sheet
                if (string.IsNullOrEmpty(sheetName)) {
                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
                }

                var table = dataSet.Tables[sheetName];
                if (table != null) {
                    return table;
                }

                // Sheet not found → return first sheet or empty
                return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
            } catch (Exception ex) {
                throw new InvalidOperationException($"Failed to read Excel file '{filePath}': {ex.Message}", ex);
            }
        }
    }
}