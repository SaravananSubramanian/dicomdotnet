using Dicom.Network;
using System;
using System.Diagnostics;

namespace DICOMEchoVerificationWithOrthancServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dicomRemoteHost = "localhost";
                var dicomRemoteHostPort = 4242;
                var useTls = false;
                var ourDotNetTestClientDicomAeTitle = "Our Dot Net Test Client";
                var remoteDicomHostAeTitle = "ORTHANC";

                //create DICOM echo verification client with handlers
                var client = CreateDicomVerificationClient();

                //send the verification request to the remote DICOM server
                client.Send(dicomRemoteHost, dicomRemoteHostPort, useTls, ourDotNetTestClientDicomAeTitle, remoteDicomHostAeTitle);
                LogToDebugConsole("Our DICOM ping operation was successfully completed");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error occured during DICOM verification request -> {e.StackTrace}");
            }
        }

        private static DicomClient CreateDicomVerificationClient()
        {
            var client = new DicomClient();

            //register that we want to do a DICOM ping here
            var dicomCEchoRequest = new DicomCEchoRequest();

            //attach an event handler when remote peer responds to echo request 
            dicomCEchoRequest.OnResponseReceived += OnEchoResponseReceivedFromRemoteHost;
            client.AddRequest(dicomCEchoRequest);

            //Add a handler to be notified of any association rejections
            client.AssociationRejected += OnAssociationRejected;

            //Add a handler to be notified of any association information on successful connections
            client.AssociationAccepted += OnAssociationAccepted;

            //Add a handler to be notified when association is successfully released - this can be triggered by the remote peer as well
            client.AssociationReleased += OnAssociationReleased;

            return client;
        }

        private static void OnEchoResponseReceivedFromRemoteHost(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LogToDebugConsole($"DICOM Echo Verification request was received by remote host");
            LogToDebugConsole($"Response was received from remote host...");
            LogToDebugConsole($"Verification response status returned was:{response.Status.ToString()}");
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
