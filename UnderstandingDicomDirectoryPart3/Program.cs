using System;
using System.Diagnostics;
using System.IO;
using Dicom.Log;
using Dicom.Media;

namespace UnderstandingDicomDirectoryPart3
{
    public class Program
    {
        private static readonly string PathToDicomImages =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test DICOM Images");

        static void Main(string[] args)
        {
            LogToDebugConsole("Creating Dicom directory...");

            try
            {
                //location where we will create the DICOMDIR file from the images
                var pathToOutputDicomDirectoryFile = Path.Combine(PathToDicomImages, "DICOMDIR");

                if (File.Exists(pathToOutputDicomDirectoryFile))
                {
                    LogToDebugConsole($"Dicom directory file already exists at '{pathToOutputDicomDirectoryFile}'. Deleting...");
                    File.Delete(pathToOutputDicomDirectoryFile);
                }

                var directoryInfoForDicomImagesFolder = new DirectoryInfo(PathToDicomImages);

                var dicomDir = new DicomDirectory();

                foreach (var file in directoryInfoForDicomImagesFolder.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    var dicomFile = Dicom.DicomFile.Open(file.FullName);

                    dicomDir.AddFile(dicomFile, $@"000001\{file.Name}");
                }

                dicomDir.Save(pathToOutputDicomDirectoryFile);

                LogToDebugConsole($"Dicom directory creation was successful. DICOMDIR file created at '{pathToOutputDicomDirectoryFile}'");

                var dicomDirectory = DicomDirectory.Open(pathToOutputDicomDirectoryFile);

                LogToDebugConsole("Outputing the newly created DICOM directory information to console");

                LogToDebugConsole(dicomDirectory.WriteToString());
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
