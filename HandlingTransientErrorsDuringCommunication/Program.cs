//-----------------------------------------------------------------------
// Tutorial: Handling Transient Errors During DICOM Communication
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates strategies for handling network errors, timeouts,
//   and transient failures during DICOM communication.
//
// Key Concepts:
//   - Transient errors: Temporary failures that may succeed on retry
//   - Connection timeouts: Server not responding in time
//   - Association rejection: Server refuses connection
//   - Network interruption: Connection lost mid-transfer
//
// Common Transient Errors:
//   | Error Type            | Common Cause                    | Strategy    |
//   |-----------------------|---------------------------------|-------------|
//   | Connection timeout    | Network latency, server busy    | Retry       |
//   | Association rejected  | Server overloaded               | Retry later |
//   | Socket exception      | Network interruption            | Reconnect   |
//   | Transfer incomplete   | Connection dropped              | Retry       |
//
// Retry Strategies:
//   - Simple retry: Fixed delay between attempts
//   - Exponential backoff: 1s, 2s, 4s, 8s delays
//   - Jitter: Random variation to prevent thundering herd
//   - Circuit breaker: Stop retrying after threshold
//
// fo-dicom Timeout Settings:
//   - client.Options.AssociationRequestTimeout
//   - client.Options.AssociationReleaseTimeout
//   - client.Options.DimseTimeout
//
// Requirements:
//   - Network access to a DICOM server
//   - Public test: www.dicomserver.co.uk:11112
//   - Local test: Orthanc (localhost:4242)
//
// fo-dicom References:
//   - DicomClient with exception handling
//   - DicomClientOptions for timeout configuration
//   - Association events for error detection
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dicom.Network;

namespace Com.SaravananSubramanian.HandlingTransientErrorsDuringCommunication
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Remote DICOM Server Settings
        //-----------------------------------------------------------------------
        private static readonly string DicomServerHost = "localhost";
        private static readonly int DicomServerPort = 4242;
        private static readonly string RemoteAeTitle = "ORTHANC";
        private static readonly string LocalAeTitle = "FODICOM_SCU";
        private static readonly bool UseTls = false;

        // Retry configuration
        private static readonly int MaxRetryAttempts = 3;
        private static readonly int InitialRetryDelayMs = 1000;

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Handling Transient Errors Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Overview ---");
                LogToDebugConsole("Network communication can fail due to transient issues.");
                LogToDebugConsole("Proper error handling improves reliability.");
                LogToDebugConsole("");
                LogToDebugConsole("--- Configuration ---");
                LogToDebugConsole($"  Server:       {DicomServerHost}:{DicomServerPort}");
                LogToDebugConsole($"  Max Retries:  {MaxRetryAttempts}");
                LogToDebugConsole($"  Initial Delay: {InitialRetryDelayMs}ms");
                LogToDebugConsole("");

                // Demonstrate retry logic with C-ECHO
                DemonstrateRetryLogic();

                LogToDebugConsole("");
                LogToDebugConsole("Transient error handling tutorial completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrates retry logic with exponential backoff.
        /// </summary>
        private static void DemonstrateRetryLogic()
        {
            LogToDebugConsole("--- Demonstrating Retry Logic ---");
            LogToDebugConsole("");

            int attempt = 0;
            bool success = false;
            Exception lastException = null;

            while (attempt < MaxRetryAttempts && !success)
            {
                attempt++;
                LogToDebugConsole($"Attempt {attempt} of {MaxRetryAttempts}...");

                try
                {
                    // Create DICOM client with timeout configuration
                    var client = new DicomClient();

                    // Configure timeouts
                    // These help detect issues faster than default timeouts
                    // client.Options.AssociationRequestTimeout = TimeSpan.FromSeconds(10);
                    // client.Options.DimseTimeout = TimeSpan.FromSeconds(30);

                    // Add C-ECHO request
                    var echoRequest = new DicomCEchoRequest();
                    bool responseReceived = false;

                    echoRequest.OnResponseReceived += (req, response) =>
                    {
                        LogToDebugConsole($"  Response received: {response.Status}");
                        responseReceived = true;
                    };

                    client.AddRequest(echoRequest);

                    // Track association events
                    client.AssociationRejected += (s, e) =>
                    {
                        LogToDebugConsole($"  Association rejected: {e.Reason}");
                    };

                    // Send with error handling
                    client.Send(DicomServerHost, DicomServerPort, UseTls, LocalAeTitle, RemoteAeTitle);

                    if (responseReceived)
                    {
                        success = true;
                        LogToDebugConsole("  Success!");
                    }
                    else
                    {
                        LogToDebugConsole("  No response received");
                    }
                }
                catch (DicomAssociationRejectedException ex)
                {
                    lastException = ex;
                    LogToDebugConsole($"  Association rejected: {ex.Message}");
                    LogToDebugConsole("  This may be a transient issue - retrying...");
                }
                catch (DicomNetworkException ex)
                {
                    lastException = ex;
                    LogToDebugConsole($"  Network error: {ex.Message}");
                    LogToDebugConsole("  Possible network issue - retrying...");
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    LogToDebugConsole($"  Unexpected error: {ex.Message}");
                }

                // If not successful and more attempts remain, wait before retry
                if (!success && attempt < MaxRetryAttempts)
                {
                    // Exponential backoff: delay doubles each attempt
                    int delayMs = InitialRetryDelayMs * (int)Math.Pow(2, attempt - 1);

                    // Add jitter (random 0-25% variation)
                    var random = new Random();
                    delayMs += random.Next(0, delayMs / 4);

                    LogToDebugConsole($"  Waiting {delayMs}ms before retry...");
                    Thread.Sleep(delayMs);
                    LogToDebugConsole("");
                }
            }

            LogToDebugConsole("");
            if (success)
            {
                LogToDebugConsole($"Operation succeeded after {attempt} attempt(s)");
            }
            else
            {
                LogToDebugConsole($"Operation failed after {attempt} attempts");
                if (lastException != null)
                {
                    LogToDebugConsole($"Last error: {lastException.Message}");
                }
            }

            //-----------------------------------------------------------------------
            // Best Practices Summary
            //-----------------------------------------------------------------------
            LogToDebugConsole("");
            LogToDebugConsole("--- Best Practices for Error Handling ---");
            LogToDebugConsole("");
            LogToDebugConsole("1. Use appropriate timeouts:");
            LogToDebugConsole("   - AssociationRequestTimeout: 10-30 seconds");
            LogToDebugConsole("   - DimseTimeout: 30-120 seconds (depends on data size)");
            LogToDebugConsole("");
            LogToDebugConsole("2. Implement retry with exponential backoff:");
            LogToDebugConsole("   - Start with 1 second delay");
            LogToDebugConsole("   - Double delay each attempt: 1s, 2s, 4s, 8s...");
            LogToDebugConsole("   - Add random jitter to prevent thundering herd");
            LogToDebugConsole("");
            LogToDebugConsole("3. Set maximum retry limits:");
            LogToDebugConsole("   - 3-5 retries for most operations");
            LogToDebugConsole("   - Log failures for investigation");
            LogToDebugConsole("");
            LogToDebugConsole("4. Handle specific exceptions:");
            LogToDebugConsole("   - DicomAssociationRejectedException: Server busy");
            LogToDebugConsole("   - DicomNetworkException: Network issues");
            LogToDebugConsole("   - TimeoutException: Server unresponsive");
            LogToDebugConsole("");
            LogToDebugConsole("5. Consider circuit breaker pattern:");
            LogToDebugConsole("   - Track consecutive failures");
            LogToDebugConsole("   - 'Open' circuit after threshold (e.g., 5 failures)");
            LogToDebugConsole("   - Fast-fail while circuit is open");
            LogToDebugConsole("   - 'Close' circuit after successful test");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
