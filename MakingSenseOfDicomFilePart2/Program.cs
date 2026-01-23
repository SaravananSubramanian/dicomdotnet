//-----------------------------------------------------------------------
// Tutorial: Dumping All DICOM Contents to Console
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to read a DICOM file and iterate through all
//   attributes (tags), displaying their values in a human-readable format.
//
// Key Concepts:
//   - DICOM files contain many attributes organized by tag (group,element)
//   - Each tag has a VR (Value Representation) defining its data type
//   - Tags can contain single values, multiple values, or sequences
//   - Common tag categories: Patient, Study, Series, Image modules
//
// Requirements:
//   - DICOM test file: Place a .dcm file in the "Test Files" folder
//   - Sample files available from: https://www.dicomlibrary.com/
//
// fo-dicom References:
//   - DicomFile.Open() - Opens and parses a DICOM file
//   - DicomDataset iteration - Enumerate all tags in the dataset
//   - GetValueOrDefault() - Safely retrieve tag values with fallback
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using FellowOakDicom;

namespace Com.SaravananSubramanian.MakingSenseOfDicomFilePart2
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
                LogToDebugConsole("=== DICOM Content Dump Tutorial ===");
                LogToDebugConsole($"Reading DICOM file: {PathToDicomTestFile}");
                LogToDebugConsole("");

                // Open the DICOM file
                var file = DicomFile.Open(PathToDicomTestFile);

                LogToDebugConsole("--- All DICOM Attributes ---");
                LogToDebugConsole("");

                // Iterate through every tag in the dataset
                // Each item represents a DICOM attribute with its tag, VR, and value
                foreach (var item in file.Dataset)
                {
                    // Get the tag's value - use GetValueOrDefault for safe retrieval
                    // The second parameter (0) is the index for multi-valued attributes
                    // The third parameter ("") is the default if the value is empty/missing
                    var value = file.Dataset.GetValueOrDefault(item.Tag, 0, "");

                    // Display in format: (group,element) VR TagName: Value
                    LogToDebugConsole($"  {item} = '{value}'");
                }

                LogToDebugConsole("");
                LogToDebugConsole("DICOM content dump completed successfully!");
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
