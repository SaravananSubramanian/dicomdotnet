using System;
using System.Diagnostics;
using System.IO;
using Dicom.Media;

namespace UnderstandingDicomDirectoryPart2
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

                var dicomDirectoryHelper = new OurDicomDirectoryHelper(LogToDebugConsole);

                dicomDirectoryHelper.ShowDicomDirectoryMetaInformation(dicomDirectory);

                foreach (var patientRecord in dicomDirectory.RootDirectoryRecordCollection)
                {
                    dicomDirectoryHelper.Display(patientRecord);

                    foreach (var studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
                    {
                        dicomDirectoryHelper.Display(studyRecord);

                        foreach (var seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
                        {
                            dicomDirectoryHelper.Display(seriesRecord);

                            foreach (var imageRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
                            {
                                dicomDirectoryHelper.Display(imageRecord);
                            }
                        }
                    }
                }

                
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
