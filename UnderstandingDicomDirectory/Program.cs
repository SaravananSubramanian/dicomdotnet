//-----------------------------------------------------------------------
// Tutorial: Reading DICOM Directories (DICOMDIR)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to open and read a DICOMDIR file, which is an
//   index file for a collection of DICOM objects (e.g., on a CD/DVD).
//
// Key Concepts:
//   - DICOMDIR: A special DICOM file that indexes other DICOM files
//   - Used for portable media (CD, DVD, USB) and file-based exchanges
//   - Contains a hierarchical structure: Patient > Study > Series > Image
//   - References actual DICOM files via relative file paths
//
// DICOMDIR Structure:
//   DICOMDIR (root)
//   └── PATIENT record
//       ├── Patient Name, ID, Birth Date, Sex
//       └── STUDY record
//           ├── Study Instance UID, Date, Description
//           └── SERIES record
//               ├── Series Instance UID, Modality, Number
//               └── IMAGE record
//                   ├── SOP Instance UID
//                   └── Referenced File ID (path to .dcm file)
//
// Requirements:
//   - DICOMDIR file: Place a DICOMDIR file in the "Test Files" folder
//   - Sample DICOMDIR files available from medical imaging datasets
//   - OsiriX sample data: https://www.osirix-viewer.com/resources/dicom-image-library/
//
// fo-dicom References:
//   - DicomDirectory - Represents a DICOMDIR file
//   - DicomDirectory.Open() - Opens and parses a DICOMDIR
//   - WriteToString() - Dumps directory contents as string
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom.Log;
using Dicom.Media;

namespace UnderstandingDicomDirectory
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
                LogToDebugConsole("=== Reading DICOMDIR Tutorial ===");
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

                LogToDebugConsole("--- DICOMDIR Contents ---");
                LogToDebugConsole("");

                // WriteToString() provides a formatted dump of all directory records
                LogToDebugConsole(dicomDirectory.WriteToString());

                LogToDebugConsole("");
                LogToDebugConsole("DICOMDIR read successfully!");
            }
            catch (Exception ex)
            {
                LogToDebugConsole($"Error reading DICOMDIR: {ex.Message}");
                LogToDebugConsole($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
