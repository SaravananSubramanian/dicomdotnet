using System;
using System.Diagnostics;
using Dicom.Network;
namespace UnderstandingDicomVerification
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var client = new DicomClient();

                //register that we want to do a DICOM ping here
                var dicomCEchoRequest = new DicomCEchoRequest();
                //attach an event handler when remote peer responds to echo request 
                dicomCEchoRequest.OnResponseReceived += OnEchoResponseReceivedFromRemoteHost;
                client.AddRequest(dicomCEchoRequest);

                //add event handlers for overall association connectivity information
                client.AssociationAccepted += ClientOnAssociationAccepted;
                client.AssociationRejected += ClientOnAssociationRejected;
                client.AssociationReleased += ClientOnAssociationReleased;

                //replace these with your settings
                //Here, I am using Dr.Dave Harvey's public server 
                //please be careful not to send any confidential info as all traffic is logged
                var dicomRemoteHost = "www.dicomserver.co.uk";
                var dicomRemoteHostPort = 11112;
                var useTls = false;
                var ourDotNetTestClientDicomAeTitle = "Our Dot Net Test Client";
                var remoteDicomHostAeTitle = "Dr.Dave Harvey's Server";

                //send the verification request to the remote DICOM server
                client.Send(dicomRemoteHost, dicomRemoteHostPort, useTls, ourDotNetTestClientDicomAeTitle, remoteDicomHostAeTitle);
                LogToDebugConsole("Our DICOM ping operation was successfully completed");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error occured during DICOM verification request -> {e.StackTrace}");
            }
        }

        private static void OnEchoResponseReceivedFromRemoteHost(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LogToDebugConsole($"    DICOM verification request was received by remote host");
            LogToDebugConsole($"    DICOM response received from remote host:");
            LogToDebugConsole($"    Verification response status returned was:{response.Status.ToString()}");
        }

        private static void ClientOnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("Association was released");
        }

        private static void ClientOnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole($"Association was rejected. Rejected Reason:{e.Reason}");
        }

        private static void ClientOnAssociationAccepted(object sender, AssociationAcceptedEventArgs e)
        {
            var association = e.Association;
            LogToDebugConsole($"Association was accepted by remote host: {association.RemoteHost} running on port: {association.RemotePort}");
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
    }
}