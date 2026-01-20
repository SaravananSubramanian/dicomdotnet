//-----------------------------------------------------------------------
// Tutorial: DICOM Storage (C-STORE)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to send DICOM files to a remote DICOM server
//   using the C-STORE operation.
//
// Key Concepts:
//   - C-STORE: Send DICOM objects from SCU to SCP for storage
//   - Storage SCU: Client that sends images
//   - Storage SCP: Server that receives and stores images (PACS, Orthanc)
//   - SOP Class negotiation: Must match the type of DICOM object being sent
//
// C-STORE Workflow:
//   1. Establish association with Storage SCP
//   2. Negotiate presentation contexts for each SOP Class to be stored
//   3. For each file:
//      a. Send C-STORE request with DICOM data
//      b. Receive C-STORE response with status
//   4. Release association
//
// Status Codes:
//   | Status  | Meaning                              |
//   |---------|--------------------------------------|
//   | 0x0000  | Success                              |
//   | 0xA7xx  | Refused: Out of resources            |
//   | 0xA9xx  | Error: Data set does not match       |
//   | 0xC0xx  | Error: Cannot understand             |
//
// Requirements:
//   - DICOM file to send: Place a .dcm file in "Test Files" folder
//   - Running DICOM Storage SCP server:
//     - Local: Install Orthanc (https://www.orthanc-server.com/)
//       Default: localhost:4242, AE Title: ORTHANC
//     - Verify Orthanc is running at http://localhost:8042
//
// fo-dicom References:
//   - DicomClient - DICOM network client
//   - DicomCStoreRequest - C-STORE request message
//   - DicomCStoreResponse - Response with storage status
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom.Network;

namespace DICOMEchoVerificationWithOrthancServer
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to DICOM file to send
        // NOTE: Ensure a valid DICOM file exists at this path before running
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomTestFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");

        //-----------------------------------------------------------------------
        // Configuration: Orthanc DICOM Server Settings
        // NOTE: You must have Orthanc running locally before running this tutorial
        //       Download from: https://www.orthanc-server.com/download.php
        //       Verify running at: http://localhost:8042
        //-----------------------------------------------------------------------
        private static readonly string DicomServerHost = "localhost";
        private static readonly int DicomServerPort = 4242;
        private static readonly string RemoteAeTitle = "ORTHANC";
        private static readonly string LocalAeTitle = "FODICOM_SCU";
        private static readonly bool UseTls = false;

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM C-STORE Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  File to send:  {PathToDicomTestFile}");
                LogToDebugConsole($"  Remote Host:   {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE:     {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:      {LocalAeTitle}");
                LogToDebugConsole("");

                // Verify the file exists before attempting to send
                if (!File.Exists(PathToDicomTestFile))
                {
                    LogToDebugConsole($"ERROR: DICOM file not found: {PathToDicomTestFile}");
                    LogToDebugConsole("Please place a .dcm file in the 'Test Files' folder.");
                    return;
                }

                // Create DICOM store client with event handlers
                var client = CreateDicomStoreClient(PathToDicomTestFile);

                LogToDebugConsole("Sending C-STORE request...");
                LogToDebugConsole("");

                // Send the file to the remote DICOM server
                client.Send(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

                LogToDebugConsole("");
                LogToDebugConsole("C-STORE operation completed successfully!");
                LogToDebugConsole("Check Orthanc web interface at http://localhost:8042 to verify.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error during C-STORE operation: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
                LogToDebugConsole("");
                LogToDebugConsole("Troubleshooting:");
                LogToDebugConsole("  1. Ensure Orthanc server is running");
                LogToDebugConsole("  2. Check firewall settings for port 4242");
                LogToDebugConsole("  3. Verify the DICOM file is valid");
            }
        }

        /// <summary>
        /// Creates a DICOM client configured for C-STORE operation.
        /// </summary>
        private static DicomClient CreateDicomStoreClient(string fileToTransmit)
        {
            var client = new DicomClient();

            // Create C-STORE request with the file to send
            // fo-dicom automatically determines the SOP Class from the file
            var cStoreRequest = new DicomCStoreRequest(fileToTransmit);

            // Attach event handler for store response
            cStoreRequest.OnResponseReceived += OnStoreResponseReceived;
            client.AddRequest(cStoreRequest);

            // Add association event handlers for status monitoring
            client.AssociationAccepted += OnAssociationAccepted;
            client.AssociationRejected += OnAssociationRejected;
            client.AssociationReleased += OnAssociationReleased;

            return client;
        }

        /// <summary>
        /// Called when the C-STORE response is received from the remote SCP.
        /// </summary>
        private static void OnStoreResponseReceived(DicomCStoreRequest request, DicomCStoreResponse response)
        {
            LogToDebugConsole("--- C-STORE Response Received ---");
            LogToDebugConsole($"  SOP Instance UID: {request.SOPInstanceUID}");
            LogToDebugConsole($"  Status:           {response.Status}");

            // Check for success (status 0x0000)
            if (response.Status == DicomStatus.Success)
            {
                LogToDebugConsole("  Result: Image stored successfully on remote server!");
            }
            else
            {
                LogToDebugConsole($"  Result: Storage failed with status {response.Status}");
            }
        }

        /// <summary>
        /// Called when the association is accepted by the remote SCP.
        /// </summary>
        private static void OnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            LogToDebugConsole("--- Association Accepted ---");
            LogToDebugConsole($"  Remote Host: {e.Association.RemoteHost}");
        }

        /// <summary>
        /// Called when the association is rejected by the remote SCP.
        /// </summary>
        private static void OnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole("--- Association Rejected ---");
            LogToDebugConsole($"  Reason: {e.Reason}");
        }

        /// <summary>
        /// Called when the association is released.
        /// </summary>
        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("--- Association Released ---");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}