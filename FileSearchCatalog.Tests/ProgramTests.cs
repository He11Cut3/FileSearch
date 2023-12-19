using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using Xceed.Words.NET;
using Xunit;

public class ProgramTests : IDisposable
{
    private readonly string testDirectory;
    private readonly string testDataDirectory;
    private readonly string searchStringFilePath;

    public ProgramTests()
    {
        // Создаем временную папку на рабочем столе
        testDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestDirectory");
        Directory.CreateDirectory(testDirectory);

        // Создаем подкаталог для тестовых данных
        testDataDirectory = Path.Combine(testDirectory, "TestData");
        Directory.CreateDirectory(testDataDirectory);

        // Создаем файл SearchString.txt и записываем в него "SearchString"
        searchStringFilePath = Path.Combine(testDataDirectory, "SearchString.txt");
        File.WriteAllText(searchStringFilePath, "SearchString");
    }

    [Fact]
    public void SearchTextInFiles_WhenSearchStringFoundInExcel_ShouldAddToResultLines()
    {
        // Arrange
        var directoryPath = Path.Combine(testDirectory, "TestData");
        var searchString = "ExselTest";
        var resultLines = new List<string>();

        // Создаем файл Excel с тестовыми данными
        var excelFilePath = Path.Combine(directoryPath, "TestExcelFile.xlsx");
        using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
            worksheet.Cells["A1"].Value = "ExselTest";

            package.Save();
        }

        // Act
        Program.SearchTextInFiles(directoryPath, searchString, resultLines);

        // Assert
        Assert.NotEmpty(resultLines);
        Assert.All(resultLines, line => Assert.Contains(searchString, line));
    }

    [Fact]
    public void SearchTextInFiles_WhenSearchStringFoundInTxt_ShouldAddToResultLines()
    {
        // Arrange
        var directoryPath = Path.Combine(testDirectory, "TestData");
        var searchString = "TxtTest";
        var resultLines = new List<string>();

        // Создаем файл TXT с тестовыми данными
        var txtFilePath = Path.Combine(directoryPath, "TestTxtFile.txt");
        File.WriteAllText(txtFilePath, "TxtTest");

        // Act
        Program.SearchTextInFiles(directoryPath, searchString, resultLines);

        // Assert
        Assert.NotEmpty(resultLines);
        Assert.All(resultLines, line => Assert.Contains(searchString, line));
    }

    [Fact]
    public void SearchTextInFiles_WhenSearchStringFoundInWord_ShouldAddToResultLines()//Проверка на 
    {
        // Arrange
        var directoryPath = Path.Combine(testDirectory, "TestData");
        var searchString = "WordTest";
        var resultLines = new List<string>();

        // Создаем файл Word с тестовыми данными
        var wordFilePath = Path.Combine(directoryPath, "TestWordFile.docx");
        using (var document = DocX.Create(wordFilePath))
        {
            document.InsertParagraph("WordTest");
            document.Save();
        }

        // Act
        Program.SearchTextInFiles(directoryPath, searchString, resultLines);

        // Assert
        Assert.NotEmpty(resultLines);
        Assert.All(resultLines, line => Assert.Contains(searchString, line));
    }


    [Fact]
    public void SearchTextInFiles_WhenSearchStringNotFound_ShouldNotAddToResultLines() //Проверка поведения при отсутствии искомой строки.
    {
        // Arrange
        var directoryPath = Path.Combine(testDirectory, "TestData");
        var searchString = "NonexistentString";
        var resultLines = new List<string>();

        // Act
        Program.SearchTextInFiles(directoryPath, searchString, resultLines);

        // Assert
        Assert.Empty(resultLines);
    }

    [Fact]
    public void SearchTextInFiles_WhenSearchStringFound_ShouldAddToResultLines() //Проверка корректности добавления строк при наличии искомой строки.
    {
        // Arrange
        var directoryPath = Path.Combine(testDirectory, "TestData");
        var searchString = "SearchString";
        var resultLines = new List<string>();

        // Act
        Program.SearchTextInFiles(directoryPath, searchString, resultLines);

        // Assert
        Assert.NotEmpty(resultLines);
        Assert.All(resultLines, line => Assert.Contains(searchString, line));
    }

    [Fact]
    public void GetRelativePath_ShouldReturnCorrectRelativePath() //Проверка корректности получения относительного пути.
    {
        // Arrange
        var basePath = testDataDirectory;
        var fullPath = searchStringFilePath;

        // Act
        var result = Program.GetRelativePath(basePath, fullPath);

        // Assert
        var expected = Path.Combine("TestData", "SearchString.txt");
        Assert.Equal(expected, result);
    }
    public void Dispose() //Для удаления временной папки после завершения всех тестов.
    {
        Directory.Delete(testDirectory, true);
    }
}
