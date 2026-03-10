using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SanGuoCharacterEditor.Utils
{
    public static class ExcelDataValidationHelper
    {
        private const string enumSheetName = "Enums";
        private const int startRow = 2;

        /// <summary>
        /// 创建枚举sheet
        /// </summary>
        public static void CreateEnumSheet(SpreadsheetDocument doc, Dictionary<string, string[]> enumData)
        {
            var wbPart = doc.WorkbookPart;

            var oldSheet = wbPart.Workbook.Sheets.Elements<Sheet>()
                .FirstOrDefault(s => s.Name == enumSheetName);

            if (oldSheet != null)
            {
                var part = wbPart.GetPartById(oldSheet.Id.Value);
                wbPart.DeletePart(part);
                oldSheet.Remove();
            }

            var enumSheetPart = wbPart.AddNewPart<WorksheetPart>();
            enumSheetPart.Worksheet = new Worksheet(new SheetData());

            var sheets = wbPart.Workbook.Sheets;

            uint sheetId = sheets.Elements<Sheet>().Any()
                ? sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1
                : 1;

            sheets.Append(new Sheet
            {
                Id = wbPart.GetIdOfPart(enumSheetPart),
                SheetId = sheetId,
                Name = enumSheetName
            });

            var sheetData = enumSheetPart.Worksheet.GetFirstChild<SheetData>();

            int col = 1;

            foreach (var kv in enumData)
            {
                var title = kv.Key;
                var values = kv.Value;

                // 写标题
                var headerRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 1)
                    ?? sheetData.AppendChild(new Row { RowIndex = 1 });

                headerRow.Append(CreateTextCell(GetColumnLetter(col), 1, title));

                // 写枚举
                for (int i = 0; i < values.Length; i++)
                {
                    uint rowIndex = (uint)(i + 2);

                    var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex)
                        ?? sheetData.AppendChild(new Row { RowIndex = rowIndex });

                    row.Append(CreateTextCell(GetColumnLetter(col), rowIndex, values[i]));
                }

                col++;
            }

            enumSheetPart.Worksheet.Save();
        }

        /// <summary>
        /// 批量应用枚举验证
        /// </summary>
        public static void ApplyEnumValidations(
            SpreadsheetDocument doc,
            string sheetName,
            params (string columnHeader, string enumTitle)[] mappings)
        {
            var wbPart = doc.WorkbookPart;

            // 找目标sheet
            var sheet = wbPart.Workbook.Sheets.Elements<Sheet>()
                .FirstOrDefault(s => s.Name == sheetName);

            if (sheet == null)
                throw new Exception($"Sheet '{sheetName}' not found");

            var mainSheetPart = (WorksheetPart)wbPart.GetPartById(sheet.Id);
            var mainSheet = mainSheetPart.Worksheet;
            var sheetData = mainSheet.GetFirstChild<SheetData>();

            // 找枚举sheet
            var enumSheet = wbPart.Workbook.Sheets.Elements<Sheet>()
                .FirstOrDefault(s => s.Name == enumSheetName);

            if (enumSheet == null)
                throw new Exception($"Enum sheet '{enumSheetName}' not found");

            var enumSheetPart = (WorksheetPart)wbPart.GetPartById(enumSheet.Id);
            var enumSheetData = enumSheetPart.Worksheet.GetFirstChild<SheetData>();

            // 解析 header
            var headerRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 1);
            if (headerRow == null) return;

            var headerMap = headerRow.Elements<Cell>()
                .ToDictionary(
                    c => GetCellValue(c, wbPart),
                    c => GetColumnIndexFromCellReference(c.CellReference!)
                );

            // 解析 enum title
            var enumHeaderRow = enumSheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 1);
            if (enumHeaderRow == null) return;

            var enumMap = enumHeaderRow.Elements<Cell>()
                .ToDictionary(
                    c => GetCellValue(c, wbPart),
                    c => GetColumnIndexFromCellReference(c.CellReference!)
                );

            // 数据行
            var rowIndexes = sheetData.Elements<Row>().Select(r => r.RowIndex.Value);
            uint lastRow = rowIndexes.Any() ? rowIndexes.Max() : (uint)startRow;

            var dataValidations = mainSheet.Elements<DataValidations>().FirstOrDefault();

            if (dataValidations == null)
            {
                dataValidations = new DataValidations();
                mainSheet.Append(dataValidations);
            }

            foreach (var (columnHeader, enumTitle) in mappings)
            {
                if (!headerMap.TryGetValue(columnHeader, out int col)) continue;
                if (!enumMap.TryGetValue(enumTitle, out int enumCol)) continue;

                uint enumLastRow = GetEnumLastRow(enumSheetData, enumCol);

                string colLetter = GetColumnLetter(enumCol);
                string formula = $"'{enumSheetName}'!${colLetter}${2}:${colLetter}${enumLastRow}";

                dataValidations.Append(new DataValidation
                {
                    Type = DataValidationValues.List,
                    AllowBlank = false,
                    ShowErrorMessage = true,
                    SequenceOfReferences = new ListValue<StringValue>
                    {
                        InnerText = $"{GetColumnLetter(col)}{startRow}:{GetColumnLetter(col)}{lastRow}"
                    },
                    Formula1 = new Formula1(formula)
                });
            }

            dataValidations.Count = (uint)dataValidations.ChildElements.Count;

            mainSheet.Save();
        }

        static Cell CreateTextCell(string column, uint row, string text)
        {
            return new Cell
            {
                CellReference = column + row,
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
        }

        static string GetCellReference(int colIndex, int rowIndex)
            => $"{GetColumnLetter(colIndex)}{rowIndex}";

        static string GetColumnLetter(int col)
        {
            string letter = "";
            while (col > 0)
            {
                int mod = (col - 1) % 26;
                letter = (char)(65 + mod) + letter;
                col = (col - mod) / 26;
            }
            return letter;
        }

        static int GetColumnIndexFromCellReference(string cellRef)
        {
            string col = new string(cellRef.TakeWhile(char.IsLetter).ToArray());

            int index = 0;

            foreach (var c in col)
                index = index * 26 + (c - 'A' + 1);

            return index;
        }

        static string GetCellValue(Cell cell, WorkbookPart wbPart)
        {
            if (cell.CellValue == null) return "";

            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                int id = int.Parse(cell.CellValue.Text);
                return wbPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(id).InnerText;
            }

            return cell.CellValue.Text;
        }

        static uint GetEnumLastRow(SheetData sheetData, int columnIndex)
        {
            uint lastRow = 2;

            foreach (var row in sheetData.Elements<Row>())
            {
                var cell = row.Elements<Cell>()
                    .FirstOrDefault(c =>
                        GetColumnIndexFromCellReference(c.CellReference!) == columnIndex);

                if (cell != null && cell.CellValue != null && !string.IsNullOrEmpty(cell.CellValue.Text))
                {
                    lastRow = row.RowIndex.Value;
                }
            }

            return lastRow;
        }
    }
}