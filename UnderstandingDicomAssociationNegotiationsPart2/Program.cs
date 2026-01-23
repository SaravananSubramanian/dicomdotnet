//-----------------------------------------------------------------------
// Tutorial: DICOM Association Rejection Handling
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to handle association rejection scenarios when
//   the remote SCP rejects the proposed presentation contexts.
//
// Key Concepts:
//   - Association rejection occurs when SCP cannot accept the request
//   - Common rejection reasons:
//     - Abstract syntax not supported (SOP Class not recognized)
//     - Transfer syntax not supported
//     - No reason given (server policy)
//     - User rejection (authentication failure)
//
// Rejection Reason Codes:
//   | Code | Reason                           |
//   |------|----------------------------------|
//   | 0    | No reason given                  |
//   | 1    | User rejection                   |
//   | 2    | No reason (provider rejection)   |
//   | 3    | Abstract syntax not supported    |
//   | 4    | Transfer syntax not supported    |
//
// Requirements:
//   - Network access to a DICOM server
//   - Public test server: www.dicomserver.co.uk:11112
//   - Local test: Orthanc (localhost:4242, AE: ORTHANC)
//
// fo-dicom References:
//   - DicomClient.AssociationRejected - Event for rejection handling
//   - AssociationRejectedEventArgs - Contains rejection details
//   - DicomPresentationContext - Custom presentation context
//   - DicomUID - For creating custom abstract syntaxes
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using FellowOakDicom;
using System.Threading;
using System.Threading.Tasks;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Network.Client.EventArguments;

namespace Com.SaravananSubramanian.UnderstandingDicomAssociationNegotiationsPart2
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

        private static readonly string LocalAeTitle = "FODICOM_SCU";
        private static readonly bool UseTls = false;

        public static async Task Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Association Rejection Handling Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("This tutorial demonstrates what happens when the SCP");
                LogToDebugConsole("rejects an association due to an unsupported SOP Class.");
                LogToDebugConsole("");
                LogToDebugConsole("--- Connection Settings ---");
                LogToDebugConsole($"  Remote Host: {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE:   {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:    {LocalAeTitle}");
                LogToDebugConsole("");

                // Create DICOM client
                var client = DicomClientFactory.Create(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

                //-----------------------------------------------------------------------
                // Create an invalid/unsupported Abstract Syntax (SOP Class UID)
                // This will cause the SCP to reject the presentation context
                //-----------------------------------------------------------------------
                LogToDebugConsole("Creating an invalid presentation context...");
                LogToDebugConsole("  Using fake Abstract Syntax UID: 1.2.3.4.5.6.7.8.9");
                LogToDebugConsole("");

                var invalidAbstractSyntax = new DicomUID(
                    "1.2.3.4.5.6.7.8.9",
                    "Fake SOP Class",
                    DicomUidType.SOPClass);

                // Add the invalid presentation context to the client
                // Presentation Context ID must be an odd number (1-255)
                var invalidPresentationContext = new DicomPresentationContext(1, invalidAbstractSyntax);
                client.AdditionalPresentationContexts.Add(invalidPresentationContext);

                // Add event handlers for association events
                client.AssociationAccepted += OnAssociationAccepted;
                client.AssociationRejected += OnAssociationRejected;
                client.AssociationReleased += OnAssociationReleased;

                LogToDebugConsole("Sending invalid request to trigger rejection...");
                LogToDebugConsole("");

                // Send the request - this should trigger a rejection
                await client.SendAsync(CancellationToken.None);

                LogToDebugConsole("");
                LogToDebugConsole("Rejection handling demonstration completed.");
            }
            catch (Exception e)
            {
                // Note: fo-dicom may throw an exception when no valid
                // presentation contexts are negotiated
                LogToDebugConsole("--- Exception Caught ---");
                LogToDebugConsole($"  Type: {e.GetType().Name}");
                LogToDebugConsole($"  Message: {e.Message}");
                LogToDebugConsole("");
                LogToDebugConsole("This is expected when all presentation contexts are rejected.");
            }
        }

        /// <summary>
        /// Called when the association is accepted (unlikely in this demo).
        /// </summary>
        private static void OnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            LogToDebugConsole("--- Association Accepted ---");
            LogToDebugConsole("  (Unexpected in this rejection demonstration)");

            // Check each presentation context for individual rejections
            foreach (var pc in e.Association.PresentationContexts)
            {
                var status = pc.Result == DicomPresentationContextResult.Accept ? "Accepted" : "Rejected";
                LogToDebugConsole($"  Context {pc.ID}: {status}");
                LogToDebugConsole($"    Reason: {pc.GetResultDescription()}");
            }
        }

        /// <summary>
        /// Called when the association is rejected by the remote host.
        /// </summary>
        private static void OnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole("--- Association Rejected ---");
            LogToDebugConsole($"  Rejection Reason: {e.Reason}");
            LogToDebugConsole("");
            LogToDebugConsole("This is the expected behavior when proposing an");
            LogToDebugConsole("unsupported Abstract Syntax (SOP Class).");
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