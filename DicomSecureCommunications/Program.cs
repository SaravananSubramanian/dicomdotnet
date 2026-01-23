//-----------------------------------------------------------------------
// Tutorial: DICOM Secure Communications (TLS)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates DICOM TLS (Transport Layer Security) concepts for
//   encrypted, secure communications between DICOM applications.
//
// Key Concepts:
//   - DICOM TLS encrypts PHI in transit
//   - HIPAA compliance requirement
//   - Mutual authentication with certificates
//   - DICOM TLS ports: 2761, 2762
//   - TLS profiles: Basic TLS, AES TLS, BCP 195 TLS
//
// Requirements:
//   - For actual TLS: X.509 certificates required
//   - This demo shows concepts and configuration
//
// fo-dicom References:
//   - DicomClient - Supports TLS connections
//   - DicomServer - Can be configured for TLS
//   - Uses .NET SslStream for encryption
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomSecureCommunications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Secure Communications (TLS) Demo ===");
                LogToDebugConsole("");

                // Demo 1: Why use TLS
                LogToDebugConsole("--- Why Use DICOM TLS? ---");
                DemonstrateTlsImportance();

                // Demo 2: TLS concepts
                LogToDebugConsole("");
                LogToDebugConsole("--- TLS Concepts ---");
                DemonstrateTlsConcepts();

                // Demo 3: Certificate requirements
                LogToDebugConsole("");
                LogToDebugConsole("--- Certificate Requirements ---");
                DemonstrateCertificateRequirements();

                // Demo 4: fo-dicom TLS configuration
                LogToDebugConsole("");
                LogToDebugConsole("--- fo-dicom TLS Configuration ---");
                DemonstrateFoDicomTlsConfig();

                // Demo 5: Troubleshooting
                LogToDebugConsole("");
                LogToDebugConsole("--- TLS Troubleshooting ---");
                DemonstrateTroubleshooting();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate why TLS is important
        /// </summary>
        private static void DemonstrateTlsImportance()
        {
            LogToDebugConsole("");
            LogToDebugConsole("1. HIPAA Compliance");
            LogToDebugConsole("   - Encrypt PHI (Protected Health Information) in transit");
            LogToDebugConsole("   - Required for data transmission over public networks");
            LogToDebugConsole("   - Protects against unauthorized interception");
            LogToDebugConsole("");

            LogToDebugConsole("2. Authentication");
            LogToDebugConsole("   - Verify identity of communication partners");
            LogToDebugConsole("   - Ensure data goes to intended recipient");
            LogToDebugConsole("   - Prevent man-in-the-middle attacks");
            LogToDebugConsole("");

            LogToDebugConsole("3. Data Integrity");
            LogToDebugConsole("   - Detect any tampering during transmission");
            LogToDebugConsole("   - MAC (Message Authentication Code) verification");
            LogToDebugConsole("");

            LogToDebugConsole("4. Privacy");
            LogToDebugConsole("   - Prevent eavesdropping on network traffic");
            LogToDebugConsole("   - Patient data remains confidential");
        }

        /// <summary>
        /// Demonstrate TLS concepts
        /// </summary>
        private static void DemonstrateTlsConcepts()
        {
            LogToDebugConsole("");
            LogToDebugConsole("DICOM TLS Ports:");
            LogToDebugConsole("  Port 2761 - DICOM TLS (registered IANA port)");
            LogToDebugConsole("  Port 2762 - DICOM TLS (alternate)");
            LogToDebugConsole("  Custom ports are also common");
            LogToDebugConsole("");

            LogToDebugConsole("DICOM TLS Connection Profiles (PS3.15):");
            LogToDebugConsole("");
            LogToDebugConsole("  Basic TLS Secure Transport:");
            LogToDebugConsole("    - TLS 1.0 or higher");
            LogToDebugConsole("    - RSA key exchange");
            LogToDebugConsole("    - 3DES or AES encryption");
            LogToDebugConsole("");
            LogToDebugConsole("  AES TLS Secure Transport:");
            LogToDebugConsole("    - TLS 1.0 or higher");
            LogToDebugConsole("    - AES-128 or AES-256 encryption");
            LogToDebugConsole("    - Recommended for new implementations");
            LogToDebugConsole("");
            LogToDebugConsole("  BCP 195 TLS Profile:");
            LogToDebugConsole("    - TLS 1.2 or higher (TLS 1.3 preferred)");
            LogToDebugConsole("    - Modern cipher suites");
            LogToDebugConsole("    - ECDHE key exchange preferred");
            LogToDebugConsole("    - AES-GCM encryption");
            LogToDebugConsole("    - Current best practices");
        }

        /// <summary>
        /// Demonstrate certificate requirements
        /// </summary>
        private static void DemonstrateCertificateRequirements()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Server Certificate:");
            LogToDebugConsole("  - X.509 v3 certificate for the DICOM server");
            LogToDebugConsole("  - Should be signed by trusted CA");
            LogToDebugConsole("  - Common Name (CN) should match hostname");
            LogToDebugConsole("  - Subject Alternative Names (SAN) recommended");
            LogToDebugConsole("");

            LogToDebugConsole("Client Certificate (for mutual authentication):");
            LogToDebugConsole("  - X.509 v3 certificate for the client");
            LogToDebugConsole("  - Required if server demands client auth");
            LogToDebugConsole("  - CN often set to AE Title");
            LogToDebugConsole("");

            LogToDebugConsole("Trust Store:");
            LogToDebugConsole("  - Contains CA certificates to trust");
            LogToDebugConsole("  - May include root and intermediate CAs");
            LogToDebugConsole("  - Both server and client need trust stores");
            LogToDebugConsole("");

            LogToDebugConsole("Creating Self-Signed Certificates (.NET):");
            LogToDebugConsole("  // Using PowerShell:");
            LogToDebugConsole("  New-SelfSignedCertificate -DnsName \"localhost\" \\");
            LogToDebugConsole("    -CertStoreLocation \"cert:\\LocalMachine\\My\" \\");
            LogToDebugConsole("    -KeyAlgorithm RSA -KeyLength 2048 \\");
            LogToDebugConsole("    -NotAfter (Get-Date).AddYears(1)");
            LogToDebugConsole("");

            LogToDebugConsole("  // Export to PFX:");
            LogToDebugConsole("  Export-PfxCertificate -Cert $cert \\");
            LogToDebugConsole("    -FilePath \"server.pfx\" -Password $password");
        }

        /// <summary>
        /// Demonstrate fo-dicom TLS configuration
        /// </summary>
        private static void DemonstrateFoDicomTlsConfig()
        {
            LogToDebugConsole("");
            LogToDebugConsole("fo-dicom TLS Client Example:");
            LogToDebugConsole("");
            LogToDebugConsole("  // Load certificate");
            LogToDebugConsole("  var cert = new X509Certificate2(\"client.pfx\", password);");
            LogToDebugConsole("");
            LogToDebugConsole("  // Create client with TLS");
            LogToDebugConsole("  var client = DicomClientFactory.Create(host, port, useTls: true,");
            LogToDebugConsole("      callingAe, calledAe);");
            LogToDebugConsole("");
            LogToDebugConsole("  // Configure TLS options");
            LogToDebugConsole("  client.Options.TlsOptions = new DicomTlsOptions {");
            LogToDebugConsole("      CertificateValidationCallback = ValidateCert,");
            LogToDebugConsole("      LocalCertificateSelectionCallback = SelectCert");
            LogToDebugConsole("  };");
            LogToDebugConsole("");
            LogToDebugConsole("  // Add request and send");
            LogToDebugConsole("  await client.AddRequestAsync(new DicomCEchoRequest());");
            LogToDebugConsole("  await client.SendAsync();");
            LogToDebugConsole("");

            LogToDebugConsole("fo-dicom TLS Server Example:");
            LogToDebugConsole("");
            LogToDebugConsole("  // Create server with TLS");
            LogToDebugConsole("  var server = DicomServer.Create<DicomCEchoProvider>(");
            LogToDebugConsole("      port,");
            LogToDebugConsole("      userState: null,");
            LogToDebugConsole("      tlsOptions: new DicomTlsOptions {");
            LogToDebugConsole("          CertificatePath = \"server.pfx\",");
            LogToDebugConsole("          CertificatePassword = \"password\"");
            LogToDebugConsole("      });");
        }

        /// <summary>
        /// Demonstrate TLS troubleshooting
        /// </summary>
        private static void DemonstrateTroubleshooting()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Common TLS Issues:");
            LogToDebugConsole("");
            LogToDebugConsole("1. Certificate not trusted");
            LogToDebugConsole("   - Solution: Add CA to trust store");
            LogToDebugConsole("   - Or: Custom validation callback");
            LogToDebugConsole("");
            LogToDebugConsole("2. Hostname mismatch");
            LogToDebugConsole("   - Solution: CN or SAN must match server hostname");
            LogToDebugConsole("   - Or: Custom validation ignoring name (not recommended)");
            LogToDebugConsole("");
            LogToDebugConsole("3. Cipher suite mismatch");
            LogToDebugConsole("   - Solution: Enable compatible cipher suites");
            LogToDebugConsole("   - Check server and client supported ciphers");
            LogToDebugConsole("");
            LogToDebugConsole("4. Certificate expired");
            LogToDebugConsole("   - Solution: Renew certificate");
            LogToDebugConsole("   - Set up certificate rotation process");
            LogToDebugConsole("");
            LogToDebugConsole("5. Protocol version mismatch");
            LogToDebugConsole("   - Solution: Ensure TLS versions match");
            LogToDebugConsole("   - Prefer TLS 1.2 or higher");
            LogToDebugConsole("");

            LogToDebugConsole("Debugging Tips:");
            LogToDebugConsole("  - Enable .NET TLS logging:");
            LogToDebugConsole("    System.Net.ServicePointManager.SecurityProtocol");
            LogToDebugConsole("  - Use Wireshark to capture TLS handshake");
            LogToDebugConsole("  - Check Windows Event Log for SChannel errors");
            LogToDebugConsole("  - Verify certificate chain with OpenSSL:");
            LogToDebugConsole("    openssl s_client -connect host:port");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
