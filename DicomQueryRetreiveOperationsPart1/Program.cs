using System;
using System.Collections.Generic;
using Dicom;
using Dicom.Network;

namespace DicomFindOperation
{
    public class Program
    {
        // Dr. Harvey graciously provides a free DICOM server to connect and play with
        private static string QRServerHost = "www.dicomserver.co.uk";
        private static int QRServerPort = 104;
        private static string QRServerAET = "STORESCP";
        private static string AET = "FODICOMSCU";

        static void Main(string[] args)
        {
            //create a C FIND SCU Client that filters based on patient name 
            var client = CreateCFindScuDicomClient("Bowen*");
            
            client.Send(QRServerHost, QRServerPort, false, AET, QRServerAET);

            Console.ReadLine();
        }

        public static DicomClient CreateCFindScuDicomClient(string patientName)
        {
            var cFindScuDicomClient = new DicomClient();
            cFindScuDicomClient.NegotiateAsyncOps();

            //this specifies the level that we are interested in
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

            // To retrieve the attributes of data you are interested in
            // that must be returned in the result
            // you must specify them in advance with empty parameters like shown below

            request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDate, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");

            // Specify the patient name filter 
            request.Dataset.AddOrUpdate(DicomTag.PatientName, patientName);

            // Specify the encoding of the retrieved results
            // here the character set is 'Latin alphabet No. 1'
            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            // Find a list of Studies
            var studyUids = new List<string>();
            request.OnResponseReceived += (req, response) =>
            {
                LogStudyResultsFoundToDebugConsole(response);
                studyUids.Add(response.Dataset?.GetSingleValue<string>(DicomTag.StudyInstanceUID));
            };

            //add the request payload to the C FIND SCU Client
            cFindScuDicomClient.AddRequest(request);

            //Add a handler to be notified of any association rejections
            cFindScuDicomClient.AssociationRejected += OnAssociationRejected;

            //Add a handler to be notified of any association information on successful connections
            cFindScuDicomClient.AssociationAccepted += OnAssociationAccepted;

            //Add a handler to be notified when association is successfully released - this can be triggered by the remote peer as well
            cFindScuDicomClient.AssociationReleased += OnAssociationReleased;

            return cFindScuDicomClient;
        }

        public static void LogStudyResultsFoundToDebugConsole(DicomCFindResponse response)
        {
            // See http://dicom.nema.org/medical/dicom/current/output/chtml/part04/sect_CC.2.8.4.html for status codes for C-FIND responses
            // The remote AE will provide a status of pending if it has matches and continues to send it one by one
            if (response.Status == DicomStatus.Pending)
            {
                var patientName = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);
                var studyDate = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, new DateTime());
                var studyUID = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

                LogToDebugConsole("Matched Result...");
                LogToDebugConsole($"Patient Found ->  {patientName} ");
                LogToDebugConsole($"Study Date ->  {studyDate} ");
                LogToDebugConsole($"Study UID ->  {studyUID} ");
                LogToDebugConsole("\n");
            }
            
            //if all match operations against the supplied query filter have been completed
            if (response.Status == DicomStatus.Success)
            {
                LogToDebugConsole(response.Status.ToString());
            }
        }

        private static void OnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            LogToDebugConsole($"Association was accepted by:{e.Association.RemoteHost}");
        }

        private static void OnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole($"Association was rejected. Rejected Reason:{e.Reason}");
        }

        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("Association was released. BYE BYE!");
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Console.WriteLine(informationToLog);
        }

    }
}
