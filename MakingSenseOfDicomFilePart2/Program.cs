using System;
using System.Diagnostics;
using System.IO;
using Dicom;

namespace MakingSenseOfDicomFilePart2
{
    public class Program
    {
        private static readonly string PathToDicomTestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole($"Attempting to extract information from DICOM file:{PathToDicomTestFile}...");

                var file = DicomFile.Open(PathToDicomTestFile);

                foreach (var tag in file.Dataset)
                {
                    LogToDebugConsole($" {tag} '{file.Dataset.GetValueOrDefault(tag.Tag,0,"")}'");
                }

                LogToDebugConsole($"Extract operation from DICOM file successful");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error occured during DICOM file dump operation -> {e.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
    }
}
