//-----------------------------------------------------------------------
// Tutorial: Reading DICOM Files - Extracting Key Attributes
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to open a DICOM file and extract essential
//   identifying attributes (UIDs) and transfer syntax information.
//
// Key Concepts:
//   - DICOM files contain metadata (patient/study info) + pixel data
//   - UID Hierarchy: Study Instance UID > Series Instance UID > SOP Instance UID
//   - Transfer Syntax defines how pixel data is encoded (compression, byte order)
//   - SOP Class UID identifies the type of DICOM object (CT, MR, US, etc.)
//
// Requirements:
//   - DICOM test file: Place a .dcm file in the "Test Files" folder
//   - Sample files available from: https://www.dicomlibrary.com/
//
// fo-dicom References:
//   - DicomFile.Open() - Opens and parses a DICOM file
//   - DicomDataset - Contains all DICOM attributes
//   - DicomTag - Standard DICOM tag definitions
//   - FileMetaInfo - Contains transfer syntax and file meta information
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom;

namespace Com.SaravananSubramanian.MakingSenseOfDicomFile
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to DICOM test file
        // NOTE: Ensure a valid DICOM file exists at this path before running
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomTestFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM File Reading Tutorial ===");
                LogToDebugConsole($"Reading DICOM file: {PathToDicomTestFile}");
                LogToDebugConsole("");

                // Open the DICOM file with all data loaded into memory
                var file = DicomFile.Open(PathToDicomTestFile, readOption: FileReadOption.ReadAll);
                var dataset = file.Dataset;

                // Extract the UID hierarchy - these uniquely identify the image in a PACS
                // Study Instance UID: Unique identifier for the entire study (exam)
                var studyInstanceUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);

                // Series Instance UID: Unique identifier for a series within the study
                var seriesInstanceUid = dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);

                // SOP Class UID: Identifies the type of DICOM object (e.g., CT Image, MR Image)
                var sopClassUid = dataset.GetSingleValue<string>(DicomTag.SOPClassUID);

                // SOP Instance UID: Unique identifier for this specific image
                var sopInstanceUid = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);

                // Transfer Syntax: Defines encoding (compression type, byte order)
                var transferSyntaxUid = file.FileMetaInfo.TransferSyntax;

                // Display the extracted information
                LogToDebugConsole("--- UID Hierarchy ---");
                LogToDebugConsole($"  Study Instance UID:  {studyInstanceUid}");
                LogToDebugConsole($"  Series Instance UID: {seriesInstanceUid}");
                LogToDebugConsole($"  SOP Instance UID:    {sopInstanceUid}");
                LogToDebugConsole("");
                LogToDebugConsole("--- Object Information ---");
                LogToDebugConsole($"  SOP Class UID:       {sopClassUid}");
                LogToDebugConsole($"  Transfer Syntax:     {transferSyntaxUid}");
                LogToDebugConsole("");
                LogToDebugConsole("DICOM file read successfully!");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error reading DICOM file: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
