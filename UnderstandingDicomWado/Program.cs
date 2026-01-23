//-----------------------------------------------------------------------
// Tutorial: WADO (Web Access to DICOM Objects)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to retrieve DICOM objects via HTTP using the
//   original WADO standard (WADO-URI), the predecessor to DICOMweb.
//
// Key Concepts:
//   - WADO: HTTP-based access to DICOM objects
//   - Uses query parameters to specify study/series/instance
//   - Can return DICOM, JPEG, or other formats
//   - Simpler than DIMSE for web-based viewing
//
// WADO vs DICOMweb:
//   | Feature        | WADO-URI (Original) | WADO-RS (DICOMweb) |
//   |----------------|---------------------|---------------------|
//   | Standard       | DICOM Supplement 103| DICOM Part 18       |
//   | Style          | Query parameters    | RESTful paths       |
//   | Metadata       | Limited             | Full JSON/XML       |
//   | Adoption       | Legacy systems      | Modern systems      |
//
// WADO-URI URL Format:
//   http://server/wado?requestType=WADO
//     &studyUID={studyUID}
//     &seriesUID={seriesUID}
//     &objectUID={sopInstanceUID}
//     &contentType={mime-type}
//
// Content Types:
//   - application/dicom: Full DICOM object
//   - image/jpeg: Rendered JPEG image
//   - image/png: Rendered PNG image
//
// Orthanc WADO Configuration:
//   - Built-in support via /wado endpoint
//   - Example: http://localhost:8042/wado
//
// Requirements:
//   - WADO-capable server:
//     - Orthanc (built-in WADO support)
//     - DCM4CHEE or other PACS with WADO
//   - HTTP client for requests
//
// fo-dicom References:
//   - Use standard HttpClient for WADO requests
//   - DicomFile.Open() can parse retrieved DICOM bytes
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.SaravananSubramanian.UnderstandingDicomWado
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: WADO Server Settings
        // NOTE: Orthanc provides built-in WADO support
        //-----------------------------------------------------------------------
        private static readonly string WadoBaseUrl = "http://localhost:8042/wado";

        // Example UIDs - replace with actual UIDs from your server
        private static readonly string StudyUid = "1.2.3.4.5.6.7.8.9";
        private static readonly string SeriesUid = "1.2.3.4.5.6.7.8.9.1";
        private static readonly string InstanceUid = "1.2.3.4.5.6.7.8.9.1.1";

        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");

        public static async Task Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== WADO (Web Access to DICOM Objects) Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Overview ---");
                LogToDebugConsole("WADO enables HTTP-based retrieval of DICOM objects.");
                LogToDebugConsole("It's simpler than DIMSE for web-based image viewing.");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  WADO URL: {WadoBaseUrl}");
                LogToDebugConsole("");
                LogToDebugConsole("--- Requirements ---");
                LogToDebugConsole("  - Orthanc or other WADO-capable server");
                LogToDebugConsole("  - DICOM data with known UIDs");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Run async demo
                await RunWadoDemoAsync();

                LogToDebugConsole("");
                LogToDebugConsole("WADO tutorial completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrates WADO operations using HttpClient.
        /// </summary>
        private static async Task RunWadoDemoAsync()
        {
            using (var httpClient = new HttpClient())
            {
                //-----------------------------------------------------------------------
                // Build WADO URL examples
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- WADO URL Examples ---");
                LogToDebugConsole("");

                // Retrieve as DICOM
                var dicomUrl = $"{WadoBaseUrl}?requestType=WADO" +
                    $"&studyUID={StudyUid}" +
                    $"&seriesUID={SeriesUid}" +
                    $"&objectUID={InstanceUid}" +
                    $"&contentType=application/dicom";

                LogToDebugConsole("Retrieve as DICOM:");
                LogToDebugConsole($"  {dicomUrl}");
                LogToDebugConsole("");

                // Retrieve as JPEG
                var jpegUrl = $"{WadoBaseUrl}?requestType=WADO" +
                    $"&studyUID={StudyUid}" +
                    $"&seriesUID={SeriesUid}" +
                    $"&objectUID={InstanceUid}" +
                    $"&contentType=image/jpeg";

                LogToDebugConsole("Retrieve as JPEG:");
                LogToDebugConsole($"  {jpegUrl}");
                LogToDebugConsole("");

                // Retrieve as PNG
                var pngUrl = $"{WadoBaseUrl}?requestType=WADO" +
                    $"&studyUID={StudyUid}" +
                    $"&seriesUID={SeriesUid}" +
                    $"&objectUID={InstanceUid}" +
                    $"&contentType=image/png";

                LogToDebugConsole("Retrieve as PNG:");
                LogToDebugConsole($"  {pngUrl}");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Optional Parameters
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Optional WADO Parameters ---");
                LogToDebugConsole("");
                LogToDebugConsole("  rows=512          : Limit image height");
                LogToDebugConsole("  columns=512       : Limit image width");
                LogToDebugConsole("  windowCenter=40   : Window center for CT");
                LogToDebugConsole("  windowWidth=400   : Window width for CT");
                LogToDebugConsole("  frameNumber=1     : Frame for multi-frame");
                LogToDebugConsole("  imageQuality=90   : JPEG quality (1-100)");
                LogToDebugConsole("  anonymize=yes     : Remove PHI");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Attempt to retrieve from Orthanc
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Attempting WADO Request ---");
                LogToDebugConsole("");

                try
                {
                    // Try a simple request to see if server is available
                    var response = await httpClient.GetAsync(WadoBaseUrl + "?requestType=WADO");

                    LogToDebugConsole($"  Server response: {response.StatusCode}");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        // Expected - we didn't provide valid UIDs
                        LogToDebugConsole("  (This is expected - no valid UIDs provided)");
                        LogToDebugConsole("  WADO server is accessible!");
                    }
                }
                catch (HttpRequestException ex)
                {
                    LogToDebugConsole($"  Connection failed: {ex.Message}");
                    LogToDebugConsole("  Ensure Orthanc is running at http://localhost:8042");
                }

                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Code Example
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Code Example: Retrieve DICOM via WADO ---");
                LogToDebugConsole("");
                LogToDebugConsole(@"
using System.Net.Http;
using FellowOakDicom;

// Build WADO URL
var wadoUrl = $""{WadoBaseUrl}?requestType=WADO"" +
    $""&studyUID={studyUid}"" +
    $""&seriesUID={seriesUid}"" +
    $""&objectUID={instanceUid}"" +
    $""&contentType=application/dicom"";

// Retrieve DICOM bytes
using var httpClient = new HttpClient();
var dicomBytes = await httpClient.GetByteArrayAsync(wadoUrl);

// Parse with fo-dicom
using var memoryStream = new MemoryStream(dicomBytes);
var dicomFile = DicomFile.Open(memoryStream);
var patientName = dicomFile.Dataset.GetSingleValue<string>(DicomTag.PatientName);
");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
