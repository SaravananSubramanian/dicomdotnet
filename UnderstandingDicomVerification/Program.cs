//-----------------------------------------------------------------------
// Tutorial: DICOM Network Verification (C-ECHO)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to verify DICOM network connectivity using the
//   C-ECHO command, also known as "DICOM Ping".
//
// Key Concepts:
//   - SCU (Service Class User): Client initiating the request
//   - SCP (Service Class Provider): Server responding to requests
//   - AE Title: Application Entity identifier (max 16 characters)
//   - Association: DICOM connection session between two AEs
//   - C-ECHO: Verification service to test connectivity
//
// C-ECHO Workflow:
//   1. SCU opens TCP connection to SCP
//   2. SCU sends A-ASSOCIATE request
//   3. SCP responds with A-ASSOCIATE accept/reject
//   4. SCU sends C-ECHO request
//   5. SCP responds with C-ECHO response (success/failure)
//   6. SCU sends A-RELEASE request
//   7. SCP confirms release
//
// Requirements:
//   - Network access to a DICOM server
//   - Public test server: www.dicomserver.co.uk:11112 (Dr. Dave Harvey's server)
//   - Local test: Install Orthanc (https://www.orthanc-server.com/)
//     - Default: localhost:4242, AE Title: ORTHANC
//
// SOP Class: Verification SOP Class (1.2.840.10008.1.1)
//
// fo-dicom References (5.x async API):
//   - DicomClientFactory.Create() - Creates async DICOM client
//   - DicomCEchoRequest - C-ECHO request message
//   - DicomCEchoResponse - C-ECHO response with status
//   - await client.AddRequestAsync() - Async request queueing
//   - await client.SendAsync() - Async network operation
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;

namespace Com.SaravananSubramanian.UnderstandingDicomVerification
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Remote DICOM Server Settings
        //-----------------------------------------------------------------------
        // Option 1: Public Test Server (Dr. Dave Harvey's server)
        // WARNING: Do not send any confidential/real patient data!
        private static readonly string DicomServerHost = "www.dicomserver.co.uk";
        private static readonly int DicomServerPort = 11112;
        private static readonly string RemoteAeTitle = "STORESCP";

        // Option 2: Local Orthanc Server (uncomment to use)
        // private static readonly string DicomServerHost = "localhost";
        // private static readonly int DicomServerPort = 4242;
        // private static readonly string RemoteAeTitle = "ORTHANC";

        // Our client's AE Title (can be any string up to 16 characters)
        private static readonly string LocalAeTitle = "FODICOM_SCU";

        // Use TLS encryption (false for most test servers)
        private static readonly bool UseTls = false;

        public static async Task Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM C-ECHO (Verification) Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Connection Settings ---");
                LogToDebugConsole($"  Remote Host:    {DicomServerHost}");
                LogToDebugConsole($"  Remote Port:    {DicomServerPort}");
                LogToDebugConsole($"  Remote AE:      {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:       {LocalAeTitle}");
                LogToDebugConsole($"  Use TLS:        {UseTls}");
                LogToDebugConsole("");

                // Create the DICOM client using factory pattern (fo-dicom 5.x)
                // Connection parameters are specified at creation time
                var client = DicomClientFactory.Create(
                    DicomServerHost,
                    DicomServerPort,
                    UseTls,
                    LocalAeTitle,
                    RemoteAeTitle);

                // Create C-ECHO request - uses Verification SOP Class (1.2.840.10008.1.1)
                var cEchoRequest = new DicomCEchoRequest();

                // Attach event handler for when the remote peer responds
                cEchoRequest.OnResponseReceived += OnEchoResponseReceived;

                // Add the request to the client asynchronously
                await client.AddRequestAsync(cEchoRequest);

                LogToDebugConsole("Sending C-ECHO request...");
                LogToDebugConsole("");

                // Send the verification request asynchronously
                await client.SendAsync(CancellationToken.None);

                LogToDebugConsole("");
                LogToDebugConsole("C-ECHO verification completed successfully!");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error during C-ECHO verification: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Event handler called when C-ECHO response is received from remote host.
        /// </summary>
        private static void OnEchoResponseReceived(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LogToDebugConsole("--- C-ECHO Response Received ---");
            LogToDebugConsole($"  Status: {response.Status}");

            // Check if the response indicates success
            // Status 0x0000 = Success
            if (response.Status == DicomStatus.Success)
            {
                LogToDebugConsole("  Result: DICOM server is alive and responding!");
            }
            else
            {
                LogToDebugConsole($"  Result: Verification failed with status {response.Status}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
