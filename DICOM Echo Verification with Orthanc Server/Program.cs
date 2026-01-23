using System.Threading;
using System.Threading.Tasks;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Network.Client.EventArguments;
using System;
using System.Diagnostics;

namespace Com.SaravananSubramanian.DICOMEchoVerificationWithOrthancServer
{
    class Program
    {
        // Configuration constants
        private static readonly string DicomRemoteHost = "localhost";
        private static readonly int DicomRemoteHostPort = 4242;
        private static readonly bool UseTls = false;
        private static readonly string OurDotNetTestClientDicomAeTitle = "OurDotNetTestClient";
        private static readonly string RemoteDicomHostAeTitle = "ORTHANC";

        static async Task Main(string[] args)
        {
            try
            {
                //create DICOM echo verification client with handlers
                var client = await CreateDicomVerificationClient();

                //send the verification request to the remote DICOM server
                await client.SendAsync(CancellationToken.None);
                LogToDebugConsole("Our DICOM ping operation was successfully completed");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error occured during DICOM verification request -> {e.StackTrace}");
            }
        }

        private static async Task<IDicomClient> CreateDicomVerificationClient()
        {
            var client = DicomClientFactory.Create(DicomRemoteHost, DicomRemoteHostPort, UseTls, OurDotNetTestClientDicomAeTitle, RemoteDicomHostAeTitle);

            //register that we want to do a DICOM ping here
            var dicomCEchoRequest = new DicomCEchoRequest();

            //attach an event handler when remote peer responds to echo request
            dicomCEchoRequest.OnResponseReceived += OnEchoResponseReceivedFromRemoteHost;
            await client.AddRequestAsync(dicomCEchoRequest);

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
            LogToDebugConsole($"Association was rejected. Reason:{e.Reason}");
        }

        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LogToDebugConsole("Association was released.");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
