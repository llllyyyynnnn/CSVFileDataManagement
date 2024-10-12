using CSVFileDataManagement;

CSVManager.Initialize();
Console.WriteLine("Enter the name of the csv file you would like to read (*.csv)");
CSVManager.SetActiveFile(Console.ReadLine());
CSVManager.PrintData();