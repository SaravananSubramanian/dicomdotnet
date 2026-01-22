//-----------------------------------------------------------------------
// Tutorial: Modality Worklist Query (MWL)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to query a Modality Worklist server to retrieve
//   scheduled procedures for a modality workstation.
//
// Key Concepts:
//   - MWL: Allows modalities to receive scheduled procedure information
//   - Reduces manual data entry errors at the modality
//   - Patient demographics auto-populated from RIS/HIS
//   - Uses C-FIND with Modality Worklist Information Model
//
// MWL Use Case:
//   Patient arrives → Technologist queries worklist →
//   Selects scheduled procedure → Demographics auto-populated
//
// SOP Class: Modality Worklist Information Model - FIND (1.2.840.10008.5.1.4.31)
//
// Key Query Attributes:
//   Patient Level:
//     - (0010,0010) Patient Name
//     - (0010,0020) Patient ID
//     - (0010,0030) Patient Birth Date
//     - (0010,0040) Patient Sex
//
//   Study/Procedure Level:
//     - (0008,0050) Accession Number
//     - (0020,000D) Study Instance UID
//     - (0032,1060) Requested Procedure Description
//     - (0040,1001) Requested Procedure ID
//
//   Scheduled Procedure Step Sequence (0040,0100):
//     - (0008,0060) Modality
//     - (0040,0001) Scheduled Station AE Title
//     - (0040,0002) Scheduled Procedure Step Start Date
//     - (0040,0003) Scheduled Procedure Step Start Time
//     - (0040,0007) Scheduled Procedure Step Description
//
// Requirements:
//   - MWL Server: Orthanc with Worklist plugin enabled
//     - Download: https://www.orthanc-server.com/
//     - Enable WorklistsPlugin in configuration
//     - Create .wl files in WorklistsDatabase folder
//   - Alternative: DCM4CHEE or other MWL-capable server
//
// fo-dicom References:
//   - DicomCFindRequest with Worklist affectedSopClassUid
//   - Scheduled Procedure Step Sequence handling
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Dicom;
using Dicom.Network;

namespace Com.SaravananSubramanian.UnderstandingDicomWorklistsAndMpps
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: MWL Server Settings
        // NOTE: Orthanc must have WorklistsPlugin enabled with worklist files
        //-----------------------------------------------------------------------
        private static readonly string DicomServerHost = "localhost";
        private static readonly int DicomServerPort = 4242;
        private static readonly string RemoteAeTitle = "ORTHANC";
        private static readonly string LocalAeTitle = "FODICOM_MWL";
        private static readonly bool UseTls = false;

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Modality Worklist Query Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  MWL Server:  {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE:   {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:    {LocalAeTitle}");
                LogToDebugConsole("");
                LogToDebugConsole("NOTE: This requires Orthanc with WorklistsPlugin enabled");
                LogToDebugConsole("      and worklist (.wl) files in the WorklistsDatabase folder.");
                LogToDebugConsole("");

                // Create MWL client
                var client = CreateMwlClient();

                LogToDebugConsole("Sending Modality Worklist query...");
                LogToDebugConsole("");

                // Execute the query
                client.Send(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

                LogToDebugConsole("");
                LogToDebugConsole("Modality Worklist query completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error during MWL query: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Creates a DICOM client configured for Modality Worklist query.
        /// </summary>
        private static DicomClient CreateMwlClient()
        {
            var client = new DicomClient();

            //-----------------------------------------------------------------------
            // Create MWL C-FIND request
            // Uses Modality Worklist Information Model
            //-----------------------------------------------------------------------
            var request = DicomCFindRequest.CreateWorklistQuery();

            //-----------------------------------------------------------------------
            // Patient Level Return Keys
            //-----------------------------------------------------------------------
            request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientBirthDate, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientSex, "");

            //-----------------------------------------------------------------------
            // Study/Procedure Level Return Keys
            //-----------------------------------------------------------------------
            request.Dataset.AddOrUpdate(DicomTag.AccessionNumber, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");
            request.Dataset.AddOrUpdate(DicomTag.RequestedProcedureDescription, "");
            request.Dataset.AddOrUpdate(DicomTag.RequestedProcedureID, "");

            //-----------------------------------------------------------------------
            // Scheduled Procedure Step Sequence (required for MWL)
            //-----------------------------------------------------------------------
            var scheduledProcedureStep = new DicomDataset();
            scheduledProcedureStep.Add(DicomTag.Modality, "");
            scheduledProcedureStep.Add(DicomTag.ScheduledStationAETitle, "");
            scheduledProcedureStep.Add(DicomTag.ScheduledProcedureStepStartDate, "");
            scheduledProcedureStep.Add(DicomTag.ScheduledProcedureStepStartTime, "");
            scheduledProcedureStep.Add(DicomTag.ScheduledProcedureStepDescription, "");
            scheduledProcedureStep.Add(DicomTag.ScheduledProcedureStepID, "");

            request.Dataset.AddOrUpdate(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, scheduledProcedureStep));

            // Filter by today's date (optional)
            // scheduledProcedureStep.AddOrUpdate(DicomTag.ScheduledProcedureStepStartDate, DateTime.Today.ToString("yyyyMMdd"));

            // Filter by modality (optional)
            // scheduledProcedureStep.AddOrUpdate(DicomTag.Modality, "CT");

            // Attach response handler
            request.OnResponseReceived += OnMwlResponseReceived;

            client.AddRequest(request);

            // Add association event handlers
            client.AssociationAccepted += (s, e) => LogToDebugConsole($"Association accepted by: {e.Association.RemoteHost}");
            client.AssociationRejected += (s, e) => LogToDebugConsole($"Association rejected: {e.Reason}");
            client.AssociationReleased += (s, e) => LogToDebugConsole("Association released.");

            return client;
        }

        /// <summary>
        /// Called for each MWL response received.
        /// </summary>
        private static void OnMwlResponseReceived(DicomCFindRequest request, DicomCFindResponse response)
        {
            if (response.Status == DicomStatus.Pending)
            {
                LogToDebugConsole("--- Scheduled Procedure Found ---");
                LogToDebugConsole($"  Patient Name: {response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, "")}");
                LogToDebugConsole($"  Patient ID:   {response.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, "")}");
                LogToDebugConsole($"  Accession #:  {response.Dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, "")}");
                LogToDebugConsole($"  Procedure:    {response.Dataset.GetSingleValueOrDefault(DicomTag.RequestedProcedureDescription, "")}");

                // Extract Scheduled Procedure Step information
                var sps = response.Dataset.GetSequence(DicomTag.ScheduledProcedureStepSequence);
                if (sps != null && sps.Items.Count > 0)
                {
                    var spsItem = sps.Items[0];
                    LogToDebugConsole($"  Modality:     {spsItem.GetSingleValueOrDefault(DicomTag.Modality, "")}");
                    LogToDebugConsole($"  Scheduled:    {spsItem.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepStartDate, "")} {spsItem.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepStartTime, "")}");
                }
                LogToDebugConsole("");
            }

            if (response.Status == DicomStatus.Success)
            {
                LogToDebugConsole("--- MWL Query Complete ---");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
