//-----------------------------------------------------------------------
// Tutorial: Storage Commitment Service
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates the Storage Commitment service, which confirms that
//   a PACS has safely archived images before the modality deletes them.
//
// Key Concepts:
//   - Storage Commitment: Guarantees images are safely stored
//   - Modality can delete local copies after commitment is confirmed
//   - Uses N-ACTION to request commitment, N-EVENT-REPORT for response
//   - Transaction UID links request to response
//
// Storage Commitment Workflow:
//   1. Modality sends images via C-STORE
//   2. Modality sends N-ACTION requesting commitment for stored images
//   3. PACS verifies images are safely stored
//   4. PACS sends N-EVENT-REPORT with success/failure results
//   5. Modality can safely delete local copies if committed
//
// SOP Class: Storage Commitment Push Model (1.2.840.10008.1.20.1)
//
// N-ACTION Request Attributes:
//   - (0008,1195) Transaction UID - Links request to response
//   - (0008,1199) Referenced SOP Sequence:
//     - Referenced SOP Class UID
//     - Referenced SOP Instance UID
//
// N-EVENT-REPORT Response:
//   - Event Type ID: 1 = Success, 2 = Complete (check failures)
//   - Referenced SOP Sequence (successful commits)
//   - Failed SOP Sequence (with failure reasons)
//
// Failure Reasons:
//   | Code  | Meaning             |
//   |-------|---------------------|
//   | 0110  | Processing failure  |
//   | 0112  | No such object      |
//   | 0213  | Resource limitation |
//
// Requirements:
//   - Storage Commitment SCP: Most enterprise PACS support this
//   - Orthanc does NOT support Storage Commitment natively
//   - DCM4CHEE or commercial PACS required for testing
//
// fo-dicom References:
//   - DicomNActionRequest - N-ACTION request
//   - Storage Commitment SOP Class handling
//   - NOTE: fo-dicom has limited built-in support for Storage Commitment
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace DicomStorageCommitmentService
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Storage Commitment SCP Settings
        // NOTE: Requires a Storage Commitment-capable PACS server
        //       Orthanc does not support Storage Commitment
        //-----------------------------------------------------------------------
        private static readonly string DicomServerHost = "localhost";
        private static readonly int DicomServerPort = 11112;
        private static readonly string RemoteAeTitle = "STORAGECMT_SCP";
        private static readonly string LocalAeTitle = "FODICOM_SCU";

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Storage Commitment Service Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Overview ---");
                LogToDebugConsole("Storage Commitment confirms images are safely archived");
                LogToDebugConsole("before the modality can delete local copies.");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  Server:    {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE: {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:  {LocalAeTitle}");
                LogToDebugConsole("");
                LogToDebugConsole("--- Requirements ---");
                LogToDebugConsole("  - Storage Commitment-capable PACS server");
                LogToDebugConsole("  - Orthanc does NOT support Storage Commitment");
                LogToDebugConsole("  - Use DCM4CHEE or commercial PACS for testing");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Storage Commitment Implementation Notes
                //-----------------------------------------------------------------------
                // fo-dicom provides limited built-in support for Storage Commitment.
                // A full implementation would require:
                //
                // 1. Send images via C-STORE first
                // 2. Create N-ACTION request with:
                //    - Transaction UID (unique identifier for this commitment request)
                //    - Referenced SOP Sequence (list of images to commit)
                // 3. Handle N-EVENT-REPORT callback with commitment results
                //
                // The callback may arrive later (asynchronously) after the
                // association is released, requiring an SCP implementation
                // to receive the event report.
                //-----------------------------------------------------------------------

                LogToDebugConsole("--- Implementation Notes ---");
                LogToDebugConsole("");
                LogToDebugConsole("Storage Commitment requires:");
                LogToDebugConsole("  1. Send images via C-STORE");
                LogToDebugConsole("  2. Send N-ACTION with Transaction UID and SOP references");
                LogToDebugConsole("  3. Implement SCP to receive N-EVENT-REPORT callback");
                LogToDebugConsole("");
                LogToDebugConsole("This is an advanced feature typically used in");
                LogToDebugConsole("production modality-to-PACS integrations.");
                LogToDebugConsole("");

                // Example pseudo-code for Storage Commitment request:
                LogToDebugConsole("--- Pseudo-code Example ---");
                LogToDebugConsole(@"
// Create Transaction UID
var transactionUid = DicomUID.Generate();

// Build Referenced SOP Sequence
var referencedSops = new DicomSequence(DicomTag.ReferencedSOPSequence);
foreach (var storedFile in storedFiles)
{
    var item = new DicomDataset();
    item.Add(DicomTag.ReferencedSOPClassUID, storedFile.SopClassUid);
    item.Add(DicomTag.ReferencedSOPInstanceUID, storedFile.SopInstanceUid);
    referencedSops.Items.Add(item);
}

// Create N-ACTION request
var actionRequest = new DicomNActionRequest(
    DicomUID.StorageCommitmentPushModelSOPClass,
    DicomUID.StorageCommitmentPushModelSOPInstance,
    1); // Action Type ID = 1 (Request Storage Commitment)

actionRequest.Dataset = new DicomDataset();
actionRequest.Dataset.Add(DicomTag.TransactionUID, transactionUid);
actionRequest.Dataset.Add(referencedSops);
");

                LogToDebugConsole("");
                LogToDebugConsole("Storage Commitment tutorial completed.");
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
