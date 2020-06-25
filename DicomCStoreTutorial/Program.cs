using Dicom.Network;
using System;
using System.Diagnostics;
using System.IO;

namespace DICOMEchoVerificationWithOrthancServer
{
    public class Program
    {
        private static readonly string PathToDicomTestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");

        static void Main(string[] args)
        {
            try
            {
                var dicomRemoteHost = "localhost";
                var dicomRemoteHostPort = 4242;
                var useTls = false;
                var ourDotNetTestClientDicomAeTitle = "OurDotNetTestClient";
                var remoteDicomHostAeTitle = "ORTHANC";

                //create DICOM store SCU client with handlers
                var client = CreateDicomStoreClient(PathToDicomTestFile);

                //send the verification request to the remote DICOM server
                client.Send(dicomRemoteHost, dicomRemoteHostPort, useTls, ourDotNetTestClientDicomAeTitle, remoteDicomHostAeTitle);
                LogToDebugConsole("Our DICOM CStore operation was successfully completed");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error occured during DICOM verification request -> {e.StackTrace}");
            }
        }

        private static DicomClient CreateDicomStoreClient(string fileToTransmit)
        {
            var client = new DicomClient();

            //request for DICOM store operation
            var dicomCStoreRequest = new DicomCStoreRequest(fileToTransmit);

            //attach an event handler when remote peer responds to store request 
            dicomCStoreRequest.OnResponseReceived += OnStoreResponseReceivedFromRemoteHost;
            client.AddRequest(dicomCStoreRequest);

            //Add a handler to be notified of any association rejections
            client.AssociationRejected += OnAssociationRejected;

            //Add a handler to be notified of any association information on successful connections
            client.AssociationAccepted += OnAssociationAccepted;

            //Add a handler to be notified when association is successfully released - this can be triggered by the remote peer as well
            client.AssociationReleased += OnAssociationReleased;

            return client;
        }

        private static void OnStoreResponseReceivedFromRemoteHost(DicomCStoreRequest request, DicomCStoreResponse response)
        {
            LogToDebugConsole("DICOM Store request was received by remote host for storage...");
            LogToDebugConsole($"DICOM Store request was received by remote host for SOP instance transmitted for storage:{request.SOPInstanceUID}");
            LogToDebugConsole($"Store operation response status returned was:{response.Status}");
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
            Debug.WriteLine(informationToLog);
        }
    }
}