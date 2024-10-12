using CSVFileDataManagement;

CSVManager.Initialize(); // maybe make it a drag & drop
Console.WriteLine("Enter the name of the csv file you would like to read (*.csv)");
CSVManager.SetActiveFile(Console.ReadLine());
CSVManager.PrintData();

while (true)
{

}