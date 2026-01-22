//-----------------------------------------------------------------------
// Tutorial: Navigating DICOMDIR Hierarchical Structure
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to programmatically navigate the hierarchical
//   structure of a DICOMDIR file (Patient > Study > Series > Image).
//
// Key Concepts:
//   - DICOMDIR uses Directory Record Sequence (0004,1220) for structure
//   - Each record type contains specific attributes
//   - Navigation is done via LowerLevelDirectoryRecordCollection
//   - Record types: PATIENT, STUDY, SERIES, IMAGE (and others)
//
// Directory Record Hierarchy:
//   RootDirectoryRecordCollection (PATIENT records)
//   └── LowerLevelDirectoryRecordCollection (STUDY records)
//       └── LowerLevelDirectoryRecordCollection (SERIES records)
//           └── LowerLevelDirectoryRecordCollection (IMAGE records)
//
// Key Attributes per Record Type:
//   PATIENT: Patient Name, Patient ID, Birth Date, Sex
//   STUDY:   Study Instance UID, Study Date, Study ID, Description
//   SERIES:  Series Instance UID, Modality, Series Number
//   IMAGE:   SOP Instance UID, Instance Number, Referenced File ID
//
// Requirements:
//   - DICOMDIR file: Place a DICOMDIR file in the "Test Files" folder
//   - Sample DICOMDIR files available from medical imaging datasets
//
// fo-dicom References:
//   - DicomDirectory - Represents a DICOMDIR file
//   - RootDirectoryRecordCollection - Top-level patient records
//   - LowerLevelDirectoryRecordCollection - Child records
//   - DicomDirectoryRecord - Individual record with attributes
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom.Media;

namespace Com.SaravananSubramanian.UnderstandingDicomDirectoryPart2
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to DICOMDIR file
        // NOTE: Ensure a valid DICOMDIR file exists at this path before running
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomDirectoryFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "DICOMDIR");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Navigating DICOMDIR Structure Tutorial ===");
                LogToDebugConsole($"Opening DICOMDIR: {PathToDicomDirectoryFile}");
                LogToDebugConsole("");

                // Verify DICOMDIR exists
                if (!File.Exists(PathToDicomDirectoryFile))
                {
                    LogToDebugConsole("ERROR: DICOMDIR file not found!");
                    LogToDebugConsole("Please place a DICOMDIR file in the 'Test Files' folder.");
                    return;
                }

                // Open and parse the DICOMDIR file
                var dicomDirectory = DicomDirectory.Open(PathToDicomDirectoryFile);

                // Create helper for formatted display
                var helper = new OurDicomDirectoryHelper(LogToDebugConsole);

                // Display DICOMDIR meta information
                helper.ShowDicomDirectoryMetaInformation(dicomDirectory);

                //-----------------------------------------------------------------------
                // Navigate the hierarchical structure
                // PATIENT > STUDY > SERIES > IMAGE
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Directory Structure ---");
                LogToDebugConsole("");

                // Iterate through PATIENT records (root level)
                foreach (var patientRecord in dicomDirectory.RootDirectoryRecordCollection)
                {
                    helper.Display(patientRecord);

                    // Iterate through STUDY records under each patient
                    foreach (var studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
                    {
                        helper.Display(studyRecord);

                        // Iterate through SERIES records under each study
                        foreach (var seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
                        {
                            helper.Display(seriesRecord);

                            // Iterate through IMAGE records under each series
                            foreach (var imageRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
                            {
                                helper.Display(imageRecord);
                            }
                        }
                    }
                }

                LogToDebugConsole("");
                LogToDebugConsole("DICOMDIR navigation completed successfully!");
            }
            catch (Exception ex)
            {
                LogToDebugConsole($"Error navigating DICOMDIR: {ex.Message}");
                LogToDebugConsole($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
