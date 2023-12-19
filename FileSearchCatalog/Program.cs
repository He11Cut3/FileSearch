using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    public static void Main()
    {
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

    public static void SearchTextInFiles(string directoryPath, string searchString, List<string> resultLines)
    {
        foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.txt", SearchOption.AllDirectories)) // Поиск пути ко всем файлам в каталоге и его подкаталогах.
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
    }


    public static string GetRelativePath(string basePath, string fullPath) // Возращает относительный путь от basePath к fullPath.
    {
        Uri baseUri = new Uri(basePath);
        Uri fullUri = new Uri(fullPath);
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }
}
