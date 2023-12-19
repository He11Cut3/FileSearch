using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using Xceed.Document.NET;
public class Program
{
    public static void Main()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Console.WriteLine("Поддерживает файлы, форматов - docx, xlsx и txt.");
        Console.Write("Введите путь: ");
        string directoryPath = Console.ReadLine(); // Ввод пути.

        Console.Write("Напишите, что необходимо найти: ");
        string searchString = Console.ReadLine(); // Ввод фразы, необходимую найти.

        if (!Directory.Exists(directoryPath)) // Проверка на существование пути.
        {
            Console.WriteLine($"Путь '{directoryPath}' не найден.");
            return;
        }

        List<string> resultLines = new List<string>(); // Создания списка

        SearchTextInFiles(directoryPath, searchString, resultLines); // "SearchTextInFiles" - функция, которая реализует поиск.
                                                                     // ( где все параметры: directoryPath - путь,  searchString - "слово", resultLines - список из всех найденный "совпадений".

        if (resultLines.Count > 0) // Если есть совпадения
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // Скачивает файл на рабочий стол.
            string resultFilePath = Path.Combine(desktopPath, "Результат.txt"); //Сочетает путь и наименование файла.

            File.WriteAllLines(resultFilePath, resultLines); //Записывание файла.
            Console.WriteLine($"Результат сохранён '{resultFilePath}'."); //Вывод информации.
        }
        else
        {
            Console.WriteLine("Таких строк не найдено."); //Если нету такой фразы в файлах.
        }
    }

    public static void SearchTextInFiles(string directoryPath,  string searchString, List<string> resultLines)
    {
        foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)) // Поиск пути ко всем файлам в каталоге и его подкаталогах.
        {
            if (filePath.EndsWith(".txt")) //Проверка на файл - .txt
            {
                SearchInTxtFile(filePath, directoryPath, searchString, resultLines); //Функция - Txt
            }
            else if (filePath.EndsWith(".xlsx"))//Проверка на файл - .xlsx
            {
                SearchInExcelFile(filePath, directoryPath, searchString, resultLines); //Функция - Excel
            }
            else if (filePath.EndsWith(".docx"))//Проверка на файл - .docx
            {
                SearchInWordFile(filePath, directoryPath, searchString, resultLines); //Функция - Word
            }
        }
    }

    public static void SearchInTxtFile(string filePath, string directoryPath, string searchString, List<string> resultLines)
    {
        int lineNumber = 0;
        foreach (string line in File.ReadLines(filePath)) // После поиска файла в каталогах - чтение файла.
        {
            lineNumber++; // Подсчёт строк.
            if (line.Contains(searchString)) // Содержит ли в себе строку, которую изначально искали?
            {
                string result = $"{GetRelativePath(directoryPath, filePath)}({lineNumber}): {line}";
                resultLines.Add(result); // Если да, то записывает в результат данную строку.
            }
        }
    }

    public static void SearchInExcelFile(string filePath,string directoryPath, string searchString, List<string> resultLines)
    {
        try
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                {
                    for (int row = 1; row <= worksheet.Dimension.Rows; row++) // Два цикоа для проверки стобцов и строк в таблице.
                    {
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            string cellValue = worksheet.Cells[row, col].Text;
                            if (cellValue.Contains(searchString)) //Если содержит в себе нужную строку - записываем в файл.
                            {
                                string result = $"{GetRelativePath(directoryPath, filePath)}({row}, {col}): {cellValue}";
                                resultLines.Add(result);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения Excel файла '{filePath}': {ex.Message}"); //Проверка на загрузку файла
        }
    }

    public static void SearchInWordFile(string filePath, string directoryPath, string searchString, List<string> resultLines)
    {
        try //Всё тоже самое, но с Word файлом. Только вместо столбцов - раздел.
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                XWPFDocument document = new XWPFDocument(fileStream);

                int paragraphNumber = 0;
                foreach (var paragraph in document.Paragraphs)
                {
                    paragraphNumber++;

                    if (paragraph.Text.Contains(searchString))
                    {
                        string result = $"{GetRelativePath(directoryPath, filePath)}(Раздел: {paragraphNumber}): {paragraph.Text}";
                        resultLines.Add(result);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения Word файла '{filePath}': {ex.Message}"); //Проверка на загрузку файла.
        }
    }


    public static string GetRelativePath(string basePath, string fullPath) // Возращает относительный путь от basePath к fullPath.
    {
        Uri baseUri = new Uri(basePath);
        Uri fullUri = new Uri(fullPath);
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }
}
