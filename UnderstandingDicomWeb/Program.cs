//-----------------------------------------------------------------------
// Tutorial: DICOMweb Services (WADO-RS, QIDO-RS, STOW-RS)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to use DICOMweb RESTful services for retrieving,
//   querying, and storing DICOM objects over HTTP/HTTPS.
//
// Key Concepts:
//   - DICOMweb: RESTful web services for DICOM (replaces traditional DIMSE)
//   - WADO-RS: Web Access to DICOM Objects - Retrieve
//   - QIDO-RS: Query based on ID for DICOM Objects - Search
//   - STOW-RS: Store Over the Web - Upload
//
// DICOMweb Endpoints:
//   | Service | Method | Endpoint                                           |
//   |---------|--------|-----------------------------------------------------|
//   | QIDO-RS | GET    | /studies?PatientName=...                           |
//   | WADO-RS | GET    | /studies/{studyUID}                                |
//   | WADO-RS | GET    | /studies/{studyUID}/series/{seriesUID}/instances/  |
//   | STOW-RS | POST   | /studies                                           |
//
// Content Types:
//   - application/dicom+json: JSON metadata
//   - multipart/related; type="application/dicom": DICOM objects
//   - image/jpeg, image/png: Rendered images
//
// Orthanc DICOMweb Configuration:
//   - Enable DicomWeb plugin in Orthanc
//   - Default endpoint: http://localhost:8042/dicom-web/
//
// Requirements:
//   - DICOMweb-capable server:
//     - Orthanc with DICOMweb plugin (https://www.orthanc-server.com/)
//     - DCM4CHEE, OHIF, or commercial PACS with DICOMweb support
//   - HTTP client library for REST calls
//
// fo-dicom References:
//   - DicomWebClient class for DICOMweb operations
//   - NOTE: fo-dicom 4.x has limited DICOMweb support
//           Consider using fo-dicom 5.x or direct HTTP client
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Com.SaravananSubramanian.UnderstandingDicomWeb
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: DICOMweb Server Settings
        // NOTE: Orthanc must have DICOMweb plugin enabled
        //-----------------------------------------------------------------------
        private static readonly string DicomWebBaseUrl = "http://localhost:8042/dicom-web";

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOMweb Services Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- DICOMweb Overview ---");
                LogToDebugConsole("DICOMweb provides RESTful access to DICOM objects:");
                LogToDebugConsole("  QIDO-RS: Query for studies/series/instances");
                LogToDebugConsole("  WADO-RS: Retrieve DICOM objects");
                LogToDebugConsole("  STOW-RS: Store DICOM objects");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  Base URL: {DicomWebBaseUrl}");
                LogToDebugConsole("");
                LogToDebugConsole("--- Requirements ---");
                LogToDebugConsole("  - Orthanc with DICOMweb plugin enabled");
                LogToDebugConsole("  - Or other DICOMweb-capable server");
                LogToDebugConsole("");

                // Run async demo
                RunDicomWebDemoAsync().GetAwaiter().GetResult();

                LogToDebugConsole("");
                LogToDebugConsole("DICOMweb tutorial completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrates DICOMweb operations using HttpClient.
        /// </summary>
        private static async Task RunDicomWebDemoAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(DicomWebBaseUrl);

                //-----------------------------------------------------------------------
                // QIDO-RS: Query for Studies
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- QIDO-RS: Querying for Studies ---");
                LogToDebugConsole("");

                try
                {
                    // Query for all studies (limit to 10)
                    var qidoUrl = "/studies?limit=10";
                    LogToDebugConsole($"  GET {DicomWebBaseUrl}{qidoUrl}");

                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/dicom+json"));

                    var response = await httpClient.GetAsync(qidoUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
                        LogToDebugConsole($"  Status: {response.StatusCode}");
                        LogToDebugConsole($"  Content-Type: {response.Content.Headers.ContentType}");
                        LogToDebugConsole($"  Response (truncated): {jsonContent.Substring(0, Math.Min(500, jsonContent.Length))}...");
                    }
                    else
                    {
                        LogToDebugConsole($"  Query failed: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    LogToDebugConsole($"  Connection failed: {ex.Message}");
                    LogToDebugConsole("  Ensure Orthanc with DICOMweb plugin is running.");
                }

                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // WADO-RS: Retrieve Study Metadata
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- WADO-RS Example URLs ---");
                LogToDebugConsole("");
                LogToDebugConsole("  Retrieve study:");
                LogToDebugConsole($"    GET {DicomWebBaseUrl}/studies/{{studyUID}}");
                LogToDebugConsole("");
                LogToDebugConsole("  Retrieve series:");
                LogToDebugConsole($"    GET {DicomWebBaseUrl}/studies/{{studyUID}}/series/{{seriesUID}}");
                LogToDebugConsole("");
                LogToDebugConsole("  Retrieve instance:");
                LogToDebugConsole($"    GET {DicomWebBaseUrl}/studies/{{studyUID}}/series/{{seriesUID}}/instances/{{instanceUID}}");
                LogToDebugConsole("");
                LogToDebugConsole("  Retrieve metadata only (JSON):");
                LogToDebugConsole($"    GET {DicomWebBaseUrl}/studies/{{studyUID}}/metadata");
                LogToDebugConsole("");
                LogToDebugConsole("  Retrieve rendered image:");
                LogToDebugConsole($"    GET {DicomWebBaseUrl}/studies/{{studyUID}}/series/{{seriesUID}}/instances/{{instanceUID}}/rendered");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // STOW-RS: Store Example
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- STOW-RS Example ---");
                LogToDebugConsole("");
                LogToDebugConsole("  Store DICOM file:");
                LogToDebugConsole($"    POST {DicomWebBaseUrl}/studies");
                LogToDebugConsole("    Content-Type: multipart/related; type=\"application/dicom\"");
                LogToDebugConsole("");
                LogToDebugConsole("  Example code:");
                LogToDebugConsole(@"
    using var content = new MultipartContent(""related"");
    content.Headers.ContentType.Parameters.Add(
        new NameValueHeaderValue(""type"", ""\""application/dicom\""""));

    var dicomBytes = File.ReadAllBytes(""image.dcm"");
    var dicomContent = new ByteArrayContent(dicomBytes);
    dicomContent.Headers.ContentType = new MediaTypeHeaderValue(""application/dicom"");
    content.Add(dicomContent);

    var response = await httpClient.PostAsync(""/studies"", content);
");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
