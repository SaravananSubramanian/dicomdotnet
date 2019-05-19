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
                //replace these with your settings
                //Here, I am using Dr.Dave Harvey's public server 
                //please be careful not to send any confidential info as all traffic is logged
                var dicomRemoteHost = "www.dicomserver.co.uk";
                var dicomRemoteHostPort = 11112;
                var useTls = false;
                var ourDotNetTestClientDicomAeTitle = "Our Dot Net Test Client";
                var remoteDicomHostAeTitle = "Dr.Dave Harvey's Server";

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

            return client;
        }

        private static void OnEchoResponseReceivedFromRemoteHost(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LogToDebugConsole($"DICOM Echo Verification request was received by remote host");
            LogToDebugConsole($"Response was received from remote host...");
            LogToDebugConsole($"Verification response status returned was:{response.Status.ToString()}");
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
    }
}