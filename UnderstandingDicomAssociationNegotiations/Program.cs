//-----------------------------------------------------------------------
// Tutorial: DICOM Association Negotiation
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how DICOM association negotiation works, including
//   presentation context negotiation between SCU and SCP.
//
// Key Concepts:
//   - Association: A DICOM network connection session
//   - Presentation Context: Combination of Abstract Syntax + Transfer Syntax
//   - Abstract Syntax: What service/SOP Class to use (e.g., Verification, CT Storage)
//   - Transfer Syntax: How data is encoded (implicit/explicit VR, byte order)
//
// Negotiation Process:
//   1. SCU proposes presentation contexts (Abstract Syntax + Transfer Syntaxes)
//   2. Each context has a Presentation Context ID (odd number 1-255)
//   3. SCP responds with Accept or Reject for each context
//   4. If accepted, SCP chooses one of the proposed Transfer Syntaxes
//
// Common Transfer Syntaxes:
//   - 1.2.840.10008.1.2   : Implicit VR Little Endian (default)
//   - 1.2.840.10008.1.2.1 : Explicit VR Little Endian
//   - 1.2.840.10008.1.2.2 : Explicit VR Big Endian (retired)
//
// Requirements:
//   - Network access to a DICOM server
//   - Public test server: www.dicomserver.co.uk:11112
//   - Local test: Orthanc (localhost:4242, AE: ORTHANC)
//
// fo-dicom References:
//   - DicomClient - Network client with association events
//   - DicomAssociation - Contains negotiated presentation contexts
//   - DicomPresentationContext - Represents a single presentation context
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Dicom.Network;

namespace UnderstandingDicomAssociationNegotiations
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

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Association Negotiation Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Connection Settings ---");
                LogToDebugConsole($"  Remote Host: {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE:   {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:    {LocalAeTitle}");
                LogToDebugConsole("");

                // Create DICOM client with association event handlers
                var client = CreateDicomClientWithAssociationHandlers();

                LogToDebugConsole("Initiating association...");
                LogToDebugConsole("");

                // Send request - this triggers association negotiation
                client.Send(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

                LogToDebugConsole("");
                LogToDebugConsole("Association demonstration completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error during association: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Creates a DICOM client with handlers for all association events.
        /// </summary>
        private static DicomClient CreateDicomClientWithAssociationHandlers()
        {
            var client = new DicomClient();

            // Create C-ECHO request to trigger association
            var cEchoRequest = new DicomCEchoRequest();
            cEchoRequest.OnResponseReceived += OnEchoResponseReceived;
            client.AddRequest(cEchoRequest);

            // Add event handlers for association lifecycle events
            client.AssociationAccepted += OnAssociationAccepted;
            client.AssociationRejected += OnAssociationRejected;
            client.AssociationReleased += OnAssociationReleased;

            return client;
        }

        /// <summary>
        /// Called when the association is successfully established.
        /// This is where we can inspect the negotiated presentation contexts.
        /// </summary>
        private static void OnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            var association = e.Association;

            LogToDebugConsole("--- Association Accepted ---");
            LogToDebugConsole($"  Remote Host: {association.RemoteHost}");
            LogToDebugConsole($"  Remote Port: {association.RemotePort}");
            LogToDebugConsole($"  Max PDU:     {association.MaximumPDULength} bytes");
            LogToDebugConsole("");

            // Display each negotiated presentation context
            LogToDebugConsole("--- Presentation Contexts ---");
            foreach (var pc in association.PresentationContexts)
            {
                LogToDebugConsole($"  Context ID: {pc.ID}");
                LogToDebugConsole($"    Abstract Syntax: {pc.AbstractSyntax}");
                LogToDebugConsole($"    Result: {pc.GetResultDescription()}");

                if (pc.Result == DicomPresentationContextResult.Accept)
                {
                    LogToDebugConsole($"    Accepted Transfer Syntax: {pc.AcceptedTransferSyntax}");
                }
                LogToDebugConsole("");
            }
        }

        /// <summary>
        /// Called when the association is rejected by the remote host.
        /// </summary>
        private static void OnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole("--- Association Rejected ---");
            LogToDebugConsole($"  Reason: {e.Reason}");
        }

        /// <summary>
        /// Called when the association is released (connection closed normally).
        /// </summary>
        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("--- Association Released ---");
            LogToDebugConsole("  Connection closed gracefully.");
        }

        /// <summary>
        /// Called when C-ECHO response is received.
        /// </summary>
        private static void OnEchoResponseReceived(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LogToDebugConsole("--- C-ECHO Response ---");
            LogToDebugConsole($"  Status: {response.Status}");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}