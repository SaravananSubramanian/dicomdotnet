//-----------------------------------------------------------------------
// Tutorial: Creating DICOMDIR Files
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to create a DICOMDIR file from a collection of
//   DICOM image files programmatically.
//
// Key Concepts:
//   - DICOMDIR creation indexes DICOM files for portable media
//   - File paths must follow DICOM Media Application Profile rules:
//     - Max 8 characters per path component
//     - Uppercase letters, digits, underscore only
//     - Use backslash as separator
//   - fo-dicom extracts necessary attributes from each file
//
// File Naming Rules:
//   - Path components: max 8 characters each
//   - Allowed characters: A-Z, 0-9, _
//   - Example: DICOM\STUDY1\SERIES1\IMAGE001
//
// Use Cases:
//   - Creating DICOM CDs/DVDs for patient records
//   - Packaging DICOM files for file-based exchange
//   - Organizing DICOM collections for archive
//
// Requirements:
//   - DICOM image files: Place .dcm files in "Test DICOM Images" folder
//   - Sample files available from: https://www.dicomlibrary.com/
//
// fo-dicom References:
//   - DicomDirectory - Create and manage DICOMDIR
//   - DicomDirectory.AddFile() - Add a DICOM file to the directory
//   - DicomDirectory.Save() - Write DICOMDIR to disk
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom;
using Dicom.Log;
using Dicom.Media;

namespace Com.SaravananSubramanian.UnderstandingDicomDirectoryPart3
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to folder containing DICOM images
        // NOTE: Place .dcm files in this folder before running
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomImages =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test DICOM Images");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Creating DICOMDIR Tutorial ===");
                LogToDebugConsole($"Source folder: {PathToDicomImages}");
                LogToDebugConsole("");

                // Verify source folder exists
                if (!Directory.Exists(PathToDicomImages))
                {
                    LogToDebugConsole("ERROR: Source folder not found!");
                    LogToDebugConsole("Please create the 'Test DICOM Images' folder and add .dcm files.");
                    return;
                }

                // Output DICOMDIR path
                var dicomdirPath = Path.Combine(PathToDicomImages, "DICOMDIR");

                // Remove existing DICOMDIR if present
                if (File.Exists(dicomdirPath))
                {
                    LogToDebugConsole($"Removing existing DICOMDIR: {dicomdirPath}");
                    File.Delete(dicomdirPath);
                }

                //-----------------------------------------------------------------------
                // Create new DICOMDIR and add files
                //-----------------------------------------------------------------------
                LogToDebugConsole("");
                LogToDebugConsole("--- Creating DICOMDIR ---");

                var dicomDir = new DicomDirectory();
                var directoryInfo = new DirectoryInfo(PathToDicomImages);
                var files = directoryInfo.GetFiles("*.dcm", SearchOption.AllDirectories);

                LogToDebugConsole($"Found {files.Length} DICOM files to index");
                LogToDebugConsole("");

                int fileCount = 0;
                foreach (var file in files)
                {
                    try
                    {
                        // Open each DICOM file
                        var dicomFile = DicomFile.Open(file.FullName);

                        // Create a relative path for the DICOMDIR reference
                        // Path format: FOLDER\FILENAME (max 8 chars each, uppercase)
                        var relativePath = $@"IMAGES\{file.Name.Substring(0, Math.Min(8, file.Name.Length)).ToUpper()}";

                        // Add file to DICOMDIR
                        dicomDir.AddFile(dicomFile, relativePath);
                        fileCount++;

                        LogToDebugConsole($"  Added: {file.Name} -> {relativePath}");
                    }
                    catch (Exception ex)
                    {
                        LogToDebugConsole($"  Skipped: {file.Name} - {ex.Message}");
                    }
                }

                //-----------------------------------------------------------------------
                // Save the DICOMDIR
                //-----------------------------------------------------------------------
                LogToDebugConsole("");
                LogToDebugConsole("Saving DICOMDIR...");
                dicomDir.Save(dicomdirPath);

                LogToDebugConsole($"DICOMDIR created successfully!");
                LogToDebugConsole($"Output: {dicomdirPath}");
                LogToDebugConsole($"Files indexed: {fileCount}");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Verify by reading back the created DICOMDIR
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Verifying Created DICOMDIR ---");
                LogToDebugConsole("");

                var createdDir = DicomDirectory.Open(dicomdirPath);
                LogToDebugConsole(createdDir.WriteToString());
            }
            catch (Exception ex)
            {
                LogToDebugConsole($"Error creating DICOMDIR: {ex.Message}");
                LogToDebugConsole($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
