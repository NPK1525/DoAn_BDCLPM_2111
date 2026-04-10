using ClosedXML.Excel;
using System;
using System.IO;

namespace TestProject1.Utilities
{
    public static partial class ExcelReportHelper
    {
        private static readonly string ExcelFilePath = FindExcelFile();

        private static string FindExcelFile()
        {
            // Tìm file Excel từ các đường dẫn có thể
            var candidates = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "2111.xlsx"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "2111.xlsx"),
                Path.Combine(@"D:\BDCLPM_LT\TestProject1", "2111.xlsx"),
            };

            foreach (var c in candidates)
            {
                var full = Path.GetFullPath(c);
                if (File.Exists(full)) return full;
            }

            // Không tìm thấy — trả về path mặc định để log
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "2111.xlsx");
        }

        private static readonly object _lock = new();

        [System.Text.RegularExpressions.GeneratedRegex(@"TC_F\d+_\d+_\d+")]
        private static partial System.Text.RegularExpressions.Regex TestCaseIdRegex();

        private static string ExtractTestCaseId(string testMethodName)
        {
            var match = TestCaseIdRegex().Match(testMethodName);
            return match.Success ? match.Value : testMethodName;
        }

        public static void WriteResult(string testMethodName, string status, string screenshotPath)
        {
            lock (_lock)
            {
                if (!File.Exists(ExcelFilePath))
                {
                    Console.WriteLine($"[ExcelReport] File không tồn tại: {ExcelFilePath}");
                    return;
                }

                var testCaseId = ExtractTestCaseId(testMethodName);

                using var workbook = new XLWorkbook(ExcelFilePath);
                foreach (var ws in workbook.Worksheets)
                {
                    if (TryWriteToSheet(ws, testCaseId, testMethodName, status, screenshotPath))
                    {
                        workbook.Save();
                        Console.WriteLine($"[ExcelReport] Đã ghi '{testCaseId}' vào sheet '{ws.Name}'");
                        return;
                    }
                }

                Console.WriteLine($"[ExcelReport] Không tìm thấy '{testCaseId}' trong bất kỳ sheet nào.");
                workbook.Save();
            }
        }

        private static bool TryWriteToSheet(IXLWorksheet ws, string testCaseId, string testMethodName, string status, string screenshotPath)
        {
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            int lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            if (lastRow == 0 || lastCol == 0) return false;

            int headerRow = FindHeaderRow(ws, lastRow, lastCol);
            if (headerRow == 0)
            {
                Console.WriteLine($"[ExcelReport] Sheet '{ws.Name}': không tìm thấy header row.");
                return false;
            }

            int colTestCaseId = FindColumnExact(ws, headerRow, lastCol, "Test Case ID");
            int colTestscripts = FindColumnContains(ws, headerRow, lastCol, "Testscripts");
            int colScreenshot = FindColumnContains(ws, headerRow, lastCol, "Hình ảnh");
            int colActual = FindColumnExact(ws, headerRow, lastCol, "Actual Result");
            
            // Debug: In ra tất cả tên cột
            Console.WriteLine($"[Debug] Các cột trong sheet '{ws.Name}':");
            for (int c = 1; c <= lastCol; c++)
            {
                var colName = ws.Cell(headerRow, c).GetString().Trim();
                Console.WriteLine($"  Cột {c}: '{colName}'");
            }
            
            // Tìm cột Result - cột 12 có tên "Result\n(Passed/Failed)"
            int colResult = 0;
            for (int c = 1; c <= lastCol; c++)
            {
                var val = ws.Cell(headerRow, c).GetString().Trim();
                // Tìm cột có chứa "Result" và "(Passed/Failed)" nhưng không phải "Expected" hoặc "Actual"
                if (val.Contains("Result", StringComparison.OrdinalIgnoreCase) &&
                    val.Contains("Passed", StringComparison.OrdinalIgnoreCase) &&
                    !val.Contains("Expected", StringComparison.OrdinalIgnoreCase) &&
                    !val.Contains("Actual", StringComparison.OrdinalIgnoreCase))
                {
                    colResult = c;
                    break;
                }
            }
            
            Console.WriteLine($"[Debug] colResult = {colResult}");

            if (colTestCaseId == 0 || colTestscripts == 0 || colResult == 0)
            {
                Console.WriteLine($"[ExcelReport] Sheet '{ws.Name}': thiếu cột bắt buộc. " +
                    $"TestCaseId={colTestCaseId}, Testscripts={colTestscripts}, Result={colResult}");
                return false;
            }

            int targetRow = FindTestCaseRow(ws, testCaseId, colTestCaseId, headerRow + 1, lastRow);
            if (targetRow == 0) return false;

            // Ghi tên test method vào cột Testscripts
            ws.Cell(targetRow, colTestscripts).Value = testMethodName;

            // Ghi Passed/Failed vào cột Result
            bool isPassed = status.Equals("Pass", StringComparison.OrdinalIgnoreCase);
            var resultCell = ws.Cell(targetRow, colResult);
            resultCell.Value = isPassed ? "Passed" : "Failed";
            resultCell.Style.Font.Bold = true;
            resultCell.Style.Font.FontColor = isPassed ? XLColor.Green : XLColor.Red;

            // Ghi Actual Result cho cả Pass lẫn Fail
            if (colActual > 0)
            {
                var actualCell = ws.Cell(targetRow, colActual);
                if (isPassed)
                {
                    actualCell.Value = "Test passed successfully.";
                    actualCell.Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    var message = TestContextHelper.GetTestMessage();
                    actualCell.Value = !string.IsNullOrEmpty(message)
                        ? message
                        : "Test failed (no error message).";
                    actualCell.Style.Font.FontColor = XLColor.Red;
                }
            }

            if (colScreenshot > 0)
                ws.Cell(targetRow, colScreenshot).Value = screenshotPath ?? "";

            return true;
        }

        private static int FindHeaderRow(IXLWorksheet ws, int lastRow, int lastCol)
        {
            int searchUntil = Math.Min(lastRow, 10);
            for (int r = 1; r <= searchUntil; r++)
            {
                bool hasActual = false, hasTestscripts = false;
                for (int c = 1; c <= lastCol; c++)
                {
                    var val = ws.Cell(r, c).GetString();
                    if (val.Contains("Actual Result", StringComparison.OrdinalIgnoreCase)) hasActual = true;
                    if (val.Contains("Testscripts", StringComparison.OrdinalIgnoreCase)) hasTestscripts = true;
                }
                if (hasActual && hasTestscripts) return r;
            }
            return 0;
        }

        private static int FindColumnExact(IXLWorksheet ws, int row, int lastCol, string keyword)
        {
            // Exact match only
            for (int c = 1; c <= lastCol; c++)
            {
                var val = ws.Cell(row, c).GetString().Trim();
                if (val.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                    return c;
            }
            return 0;
        }

        private static int FindColumnContains(IXLWorksheet ws, int row, int lastCol, string keyword)
        {
            // Tìm cột có chứa keyword, nhưng loại trừ "Expected"
            for (int c = 1; c <= lastCol; c++)
            {
                var val = ws.Cell(row, c).GetString().Trim();
                if (val.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
                    !val.Contains("Expected", StringComparison.OrdinalIgnoreCase))
                    return c;
            }
            return 0;
        }

        private static int FindTestCaseRow(IXLWorksheet ws, string testCaseId, int colTestCaseId, int startRow, int lastRow)
        {
            // Chuẩn hóa testCaseId để so sánh (bỏ dấu chấm, gạch dưới, khoảng trắng)
            string normalizedTestCaseId = testCaseId.Replace("_", "").Replace(".", "").Replace(" ", "").ToUpper();
            
            for (int r = startRow; r <= lastRow; r++)
            {
                var cellVal = ws.Cell(r, colTestCaseId).GetString().Trim();
                string normalizedCellVal = cellVal.Replace("_", "").Replace(".", "").Replace(" ", "").ToUpper();
                
                if (normalizedCellVal.Equals(normalizedTestCaseId, StringComparison.OrdinalIgnoreCase))
                    return r;
            }
            return 0;
        }
    }
}
