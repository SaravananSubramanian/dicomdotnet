using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Dicom;

namespace MakingSenseOfDicomFile
{
    public class Program
    {
        private static readonly string PathToDicomTestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");  
        
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole($"Attempting to extract information from DICOM file:{PathToDicomTestFile}...");

                var file = DicomFile.Open(PathToDicomTestFile,readOption:FileReadOption.ReadAll);
                var studyInstanceUid = file.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                var seriesInstanceUid = file.Dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                var sopClassUid = file.Dataset.GetSingleValue<string>(DicomTag.SOPClassUID);
                var sopInstanceUid = file.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                var transferSyntaxUid = file.FileMetaInfo.TransferSyntax;

                LogToDebugConsole($" StudyInstanceUid - {studyInstanceUid}");
                LogToDebugConsole($" SeriesInstanceUid - {seriesInstanceUid}");
                LogToDebugConsole($" SopClassUid - {sopClassUid}");
                LogToDebugConsole($" SopInstanceUid - {sopInstanceUid}");
                LogToDebugConsole($" TransferSyntaxUid - {transferSyntaxUid}");

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
