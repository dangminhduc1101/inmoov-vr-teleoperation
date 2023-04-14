using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the path of the folder containing the broken JSON files:");
        string folderPath = Console.ReadLine();

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("Folder does not exist!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.json");

        foreach (string filePath in files)
        {
            string jsonString = File.ReadAllText(filePath);
            jsonString = "[" + jsonString.Replace("}{", "},{") + "]";
            File.WriteAllText(filePath, jsonString);
        }

        Console.WriteLine("All JSON files in the folder have been fixed!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
