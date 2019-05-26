using System;
using System.Diagnostics;
using System.IO;
using Dicom.Log;
using Dicom.Media;

namespace UnderstandingDicomDirectory
{
    public class Program
    {
        private static readonly string PathToDicomDirectoryFile = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "DICOMDIR");

        static void Main(string[] args)
        {
            LogToDebugConsole("Performing Dicom directory dump:");

            try
            {
                var dicomDirectory = DicomDirectory.Open(PathToDicomDirectoryFile);

                LogToDebugConsole(dicomDirectory.WriteToString());

                LogToDebugConsole("Dicom directory dump operation was successful");
            }
            catch (Exception ex)
            {
                LogToDebugConsole($"Error occured during Dicom directory dump. Error:{ex.Message}");
            }
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }

    }
}
