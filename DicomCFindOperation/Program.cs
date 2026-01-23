//-----------------------------------------------------------------------
// Tutorial: DICOM Query (C-FIND)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to search for studies, series, and instances on
//   a remote DICOM server using the C-FIND operation.
//
// Key Concepts:
//   - C-FIND: Query service to search for DICOM objects
//   - Query/Retrieve Information Models:
//     - Patient Root: Query starts at Patient level
//     - Study Root: Query starts at Study level
//   - Query Retrieve Levels: PATIENT -> STUDY -> SERIES -> IMAGE
//   - Matching Keys: Attributes used to filter results
//   - Return Keys: Empty attributes filled with matching values
//
// Query Attributes:
//   - Matching keys with values filter results
//   - Return keys (empty values) are populated in response
//   - Wildcard matching: * matches any sequence, ? matches single char
//
// Common Query Levels:
//   | Level   | Key Attributes                                   |
//   |---------|--------------------------------------------------|
//   | PATIENT | Patient Name, Patient ID, Birth Date             |
//   | STUDY   | Study Instance UID, Study Date, Accession Number |
//   | SERIES  | Series Instance UID, Modality, Series Number     |
//   | IMAGE   | SOP Instance UID, Instance Number                |
//
// Requirements:
//   - Network access to a DICOM Query/Retrieve SCP
//   - Public test: www.dicomserver.co.uk:104 (Dr. Dave Harvey's server)
//     WARNING: Do not send any confidential/real patient data!
//   - Local test: Orthanc (localhost:4242, AE: ORTHANC)
//
// SOP Class: Study Root Query/Retrieve - FIND (1.2.840.10008.5.1.4.1.2.2.1)
//
// fo-dicom References:
//   - DicomClient - DICOM network client
//   - DicomCFindRequest - C-FIND query request
//   - DicomCFindResponse - Response with matching results
//   - DicomQueryRetrieveLevel - Query level enumeration
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using FellowOakDicom;
using System.Threading;
using System.Threading.Tasks;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Network.Client.EventArguments;

namespace Com.SaravananSubramanian.DicomCFindOperation
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Remote DICOM Query/Retrieve Server Settings
        //-----------------------------------------------------------------------
        // Option 1: Public Test Server (Dr. Dave Harvey's server)
        // WARNING: Do not send any confidential/real patient data!
        private static readonly string DicomServerHost = "www.dicomserver.co.uk";
        private static readonly int DicomServerPort = 104;
        private static readonly string RemoteAeTitle = "STORESCP";

        // Option 2: Local Orthanc Server (uncomment to use)
        // NOTE: You must have data stored in Orthanc for queries to return results
        // private static readonly string DicomServerHost = "localhost";
        // private static readonly int DicomServerPort = 4242;
        // private static readonly string RemoteAeTitle = "ORTHANC";

        private static readonly string LocalAeTitle = "FODICOM_SCU";
        private static readonly bool UseTls = false;

        // Store found study UIDs for potential C-GET/C-MOVE operations
        private static readonly List<string> FoundStudyUids = new List<string>();

        public static async Task Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM C-FIND (Query) Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  Remote Host: {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Remote AE:   {RemoteAeTitle}");
                LogToDebugConsole($"  Local AE:    {LocalAeTitle}");
                LogToDebugConsole("");

                // Search for patients with name starting with "Bowen"
                // The wildcard * matches any sequence of characters
                var searchPattern = "Bowen*";
                LogToDebugConsole($"Searching for patients matching: {searchPattern}");
                LogToDebugConsole("");

                // Create C-FIND SCU client
                var client = await CreateCFindClient(searchPattern);

                // Execute the query
                await client.SendAsync(CancellationToken.None);

                LogToDebugConsole("");
                LogToDebugConsole($"Query completed. Found {FoundStudyUids.Count} studies.");
                LogToDebugConsole("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error during C-FIND operation: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Creates a DICOM client configured for C-FIND query operation.
        /// </summary>
        private static async Task<IDicomClient> CreateCFindClient(string patientNameFilter)
        {
            var client = DicomClientFactory.Create(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

            // Enable asynchronous operations for better performance
            client.NegotiateAsyncOps();

            //-----------------------------------------------------------------------
            // Build the C-FIND request
            // Query at Study level using Study Root Information Model
            //-----------------------------------------------------------------------
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

            //-----------------------------------------------------------------------
            // Return Keys: Attributes you want returned in the results
            // Add them with empty values - server will fill them in
            //-----------------------------------------------------------------------
            request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDate, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDescription, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");
            request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "");
            request.Dataset.AddOrUpdate(DicomTag.NumberOfStudyRelatedSeries, "");
            request.Dataset.AddOrUpdate(DicomTag.NumberOfStudyRelatedInstances, "");

            //-----------------------------------------------------------------------
            // Matching Keys: Attributes to filter the results
            // Supports wildcards: * (any chars), ? (single char)
            //-----------------------------------------------------------------------
            request.Dataset.AddOrUpdate(DicomTag.PatientName, patientNameFilter);

            // Optional: Filter by date range (format: YYYYMMDD-YYYYMMDD)
            // request.Dataset.AddOrUpdate(DicomTag.StudyDate, "20200101-20241231");

            // Optional: Filter by modality
            // request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "CT");

            //-----------------------------------------------------------------------
            // Specify character encoding for international characters
            // ISO_IR 100 = Latin-1 (Western European)
            //-----------------------------------------------------------------------
            request.Dataset.AddOrUpdate(DicomTag.SpecificCharacterSet, "ISO_IR 100");

            // Attach response handler
            request.OnResponseReceived += OnCFindResponseReceived;

            // Add request to client
            await client.AddRequestAsync(request);

            // Add association event handlers
            client.AssociationAccepted += OnAssociationAccepted;
            client.AssociationRejected += OnAssociationRejected;
            client.AssociationReleased += OnAssociationReleased;

            return client;
        }

        /// <summary>
        /// Called for each C-FIND response received.
        /// Note: Multiple responses are received - one per matching result,
        /// plus a final response with status Success when complete.
        /// </summary>
        private static void OnCFindResponseReceived(DicomCFindRequest request, DicomCFindResponse response)
        {
            // Pending status means more results are coming
            // Each pending response contains one matching study
            if (response.Status == DicomStatus.Pending)
            {
                var patientName = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, "");
                var patientId = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, "");
                var studyDate = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, "");
                var studyDesc = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "");
                var studyUid = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "");
                var modalities = response.Dataset.GetSingleValueOrDefault(DicomTag.ModalitiesInStudy, "");

                LogToDebugConsole("--- Match Found ---");
                LogToDebugConsole($"  Patient Name: {patientName}");
                LogToDebugConsole($"  Patient ID:   {patientId}");
                LogToDebugConsole($"  Study Date:   {studyDate}");
                LogToDebugConsole($"  Description:  {studyDesc}");
                LogToDebugConsole($"  Modalities:   {modalities}");
                LogToDebugConsole($"  Study UID:    {studyUid}");
                LogToDebugConsole("");

                // Store the Study UID for potential C-GET/C-MOVE operations
                if (!string.IsNullOrEmpty(studyUid))
                {
                    FoundStudyUids.Add(studyUid);
                }
            }

            // Success status means query is complete
            if (response.Status == DicomStatus.Success)
            {
                LogToDebugConsole("--- Query Complete ---");
            }
        }

        private static void OnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            LogToDebugConsole($"Association accepted by: {e.Association.RemoteHost}");
            LogToDebugConsole("");
        }

        private static void OnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole($"Association rejected: {e.Reason}");
        }

        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("Association released.");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
