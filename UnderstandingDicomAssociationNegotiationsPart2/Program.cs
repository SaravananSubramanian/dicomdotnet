using System;
using System.Diagnostics;
using Dicom;
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

                //create DICOM client
                var client = new DicomClient();

                //Create a meaningless DICOM request. This association should be rejected by the server
                var badAbstractSyntax = new DicomUID("000", string.Empty, DicomUidType.Unknown);
                client.AdditionalPresentationContexts.Add(new DicomPresentationContext(0, badAbstractSyntax));

                //add event handler to track the association rejection event
                client.AssociationRejected += ClientOnAssociationRejected;

                //send a bad DICOM request to the remote DICOM server
                client.Send(dicomRemoteHost, dicomRemoteHostPort, useTls, ourDotNetTestClientDicomAeTitle, remoteDicomHostAeTitle);
            }
            catch (Exception e)
            {
                LogToDebugConsole("Error was thrown here during DICOM association request");
                LogToDebugConsole($"Error was: {e.Message}");
            }
        }

        private static void ClientOnAssociationRejected(object sender, AssociationRejectedEventArgs e)
        {
            LogToDebugConsole($"Association was rejected. Rejected Reason:{e.Reason}");
        }

        private static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
    }
}