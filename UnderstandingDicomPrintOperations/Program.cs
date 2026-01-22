//-----------------------------------------------------------------------
// Tutorial: DICOM Print Operations
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates DICOM Print services for printing medical images
//   to film printers or digital print destinations.
//
// Key Concepts:
//   - DICOM Print: Standardized printing of medical images
//   - Film Session: Represents one print job
//   - Film Box: One or more sheets of film
//   - Image Box: One image position on a film
//
// Print Workflow:
//   1. Create Film Session (N-CREATE)
//   2. Create Film Box with layout (N-CREATE)
//   3. Set Image Box content (N-SET)
//   4. Print the Film Box (N-ACTION)
//   5. Delete Film Session (N-DELETE)
//
// SOP Classes:
//   - Basic Film Session (1.2.840.10008.5.1.1.1)
//   - Basic Film Box (1.2.840.10008.5.1.1.2)
//   - Basic Grayscale Image Box (1.2.840.10008.5.1.1.4)
//   - Basic Color Image Box (1.2.840.10008.5.1.1.4.1)
//   - Basic Grayscale Print Meta SOP Class (1.2.840.10008.5.1.1.9)
//
// Film Layouts:
//   | Format    | Description                    |
//   |-----------|--------------------------------|
//   | STANDARD\1,1 | 1 image per sheet          |
//   | STANDARD\2,2 | 4 images (2x2 grid)        |
//   | STANDARD\2,3 | 6 images (2x3 grid)        |
//   | STANDARD\3,4 | 12 images (3x4 grid)       |
//
// Film Sizes:
//   - 14INX17IN (standard chest film)
//   - 8INX10IN, 10INX12IN, 11INX14IN
//   - A4, A3 (paper sizes)
//
// Requirements:
//   - DICOM Print SCP: Film printer or print server
//   - Most radiology workstations support DICOM Print
//   - fo-dicom PrintManagement sample for reference
//
// fo-dicom References:
//   - DicomClient for network communication
//   - N-CREATE, N-SET, N-ACTION, N-DELETE requests
//   - NOTE: DICOM Print is less commonly used now due to PACS viewers
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Com.SaravananSubramanian.UnderstandingDicomPrintOperations
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: DICOM Print Server Settings
        // NOTE: Requires a DICOM Print SCP (film printer or print server)
        //-----------------------------------------------------------------------
        private static readonly string PrintServerHost = "localhost";
        private static readonly int PrintServerPort = 11112;
        private static readonly string RemoteAeTitle = "PRINT_SCP";
        private static readonly string LocalAeTitle = "FODICOM_PRINT";

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Print Operations Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Overview ---");
                LogToDebugConsole("DICOM Print enables standardized printing of medical images");
                LogToDebugConsole("to film printers or digital destinations.");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  Print Server: {PrintServerHost}:{PrintServerPort}");
                LogToDebugConsole($"  Remote AE:    {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:     {LocalAeTitle}");
                LogToDebugConsole("");
                LogToDebugConsole("--- Requirements ---");
                LogToDebugConsole("  - DICOM Print SCP (film printer or print server)");
                LogToDebugConsole("  - DICOM images to print");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // DICOM Print Workflow Description
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- DICOM Print Workflow ---");
                LogToDebugConsole("");
                LogToDebugConsole("1. Create Film Session (N-CREATE)");
                LogToDebugConsole("   - Number of copies");
                LogToDebugConsole("   - Print priority (HIGH, MED, LOW)");
                LogToDebugConsole("   - Medium type (PAPER, FILM)");
                LogToDebugConsole("");
                LogToDebugConsole("2. Create Film Box (N-CREATE)");
                LogToDebugConsole("   - Film size (14INX17IN, A4, etc.)");
                LogToDebugConsole("   - Film orientation (PORTRAIT, LANDSCAPE)");
                LogToDebugConsole("   - Layout format (STANDARD\\1,1 or STANDARD\\2,2)");
                LogToDebugConsole("");
                LogToDebugConsole("3. Set Image Box Content (N-SET)");
                LogToDebugConsole("   - Pixel data for each image position");
                LogToDebugConsole("   - Preformatted grayscale or color");
                LogToDebugConsole("   - Polarity (NORMAL, REVERSE)");
                LogToDebugConsole("");
                LogToDebugConsole("4. Print Film Box (N-ACTION)");
                LogToDebugConsole("   - Triggers actual print operation");
                LogToDebugConsole("");
                LogToDebugConsole("5. Delete Film Session (N-DELETE)");
                LogToDebugConsole("   - Cleanup after printing");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Pseudo-code Example
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Pseudo-code Example ---");
                LogToDebugConsole("");
                LogToDebugConsole(@"
// 1. Create Film Session
var filmSession = new DicomNCreateRequest(DicomUID.BasicFilmSessionSOPClass);
filmSession.Dataset = new DicomDataset();
filmSession.Dataset.Add(DicomTag.NumberOfCopies, ""1"");
filmSession.Dataset.Add(DicomTag.PrintPriority, ""MED"");
filmSession.Dataset.Add(DicomTag.MediumType, ""PAPER"");
filmSession.OnResponseReceived += (req, res) => {
    filmSessionUid = res.SOPInstanceUID;
};

// 2. Create Film Box
var filmBox = new DicomNCreateRequest(DicomUID.BasicFilmBoxSOPClass);
filmBox.Dataset = new DicomDataset();
filmBox.Dataset.Add(DicomTag.ImageDisplayFormat, ""STANDARD\\1,1"");
filmBox.Dataset.Add(DicomTag.FilmSizeID, ""14INX17IN"");
filmBox.Dataset.Add(DicomTag.FilmOrientation, ""PORTRAIT"");
filmBox.Dataset.Add(DicomTag.MagnificationType, ""CUBIC"");
// Link to Film Session
filmBox.Dataset.Add(DicomTag.ReferencedFilmSessionSequence, ...);

// 3. Set Image Box Content
var imageBox = new DicomNSetRequest(DicomUID.BasicGrayscaleImageBoxSOPClass, imageBoxUid);
imageBox.Dataset = new DicomDataset();
imageBox.Dataset.Add(DicomTag.ImageBoxPosition, ""1"");
// Add preformatted grayscale pixel data

// 4. Print
var print = new DicomNActionRequest(DicomUID.BasicFilmBoxSOPClass, filmBoxUid, 1);

// 5. Delete Film Session
var delete = new DicomNDeleteRequest(DicomUID.BasicFilmSessionSOPClass, filmSessionUid);
");

                LogToDebugConsole("");
                LogToDebugConsole("--- Notes ---");
                LogToDebugConsole("DICOM Print is less commonly used today as PACS viewers");
                LogToDebugConsole("provide softcopy viewing. However, it's still used for:");
                LogToDebugConsole("  - Patient CDs with printed reports");
                LogToDebugConsole("  - Operating room displays");
                LogToDebugConsole("  - Legal/archival purposes");
                LogToDebugConsole("");
                LogToDebugConsole("DICOM Print tutorial completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
