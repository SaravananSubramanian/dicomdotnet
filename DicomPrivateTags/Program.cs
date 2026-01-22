//-----------------------------------------------------------------------
// Tutorial: DICOM Private Tags
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how vendors use private tags to store proprietary
//   data in DICOM objects, and how to work with them.
//
// Key Concepts:
//   - Private tags use odd group numbers (0009, 0011, 0019, etc.)
//   - Private Creator Identification reserves a block
//   - Private data elements are within the reserved block
//   - Format: (gggg,00xx) = Creator ID, (gggg,xxyy) = Data
//   - Private tags may not survive DICOM transfers
//
// Requirements:
//   - No external server connection required
//   - No test files required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomTag - For creating private tags
//   - DicomDictionary - For private tag registration
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Dicom;

namespace Com.SaravananSubramanian.DicomPrivateTags
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Private Tags Demo ===");
                LogToDebugConsole("");

                // Demo 1: Understanding private tag structure
                LogToDebugConsole("--- Demo 1: Private Tag Structure ---");
                DemonstratePrivateTagStructure();

                // Demo 2: Creating private tags
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 2: Creating Private Tags ---");
                DemonstrateCreatingPrivateTags();

                // Demo 3: Common vendor private tags
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 3: Common Vendor Private Tags ---");
                DemonstrateVendorPrivateTags();

                // Demo 4: Best practices
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 4: Best Practices ---");
                DemonstrateBestPractices();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate private tag structure
        /// </summary>
        private static void DemonstratePrivateTagStructure()
        {
            LogToDebugConsole("Private Tag Anatomy:");
            LogToDebugConsole("");
            LogToDebugConsole("Standard tag format: (gggg,eeee)");
            LogToDebugConsole("  gggg = group number (even for standard, odd for private)");
            LogToDebugConsole("  eeee = element number");
            LogToDebugConsole("");

            LogToDebugConsole("Private tag blocks:");
            LogToDebugConsole("");
            LogToDebugConsole("  Group 0009, Block 10:");
            LogToDebugConsole("    (0009,0010) = Private Creator Identification");
            LogToDebugConsole("    (0009,1000) through (0009,10FF) = Private data elements");
            LogToDebugConsole("");
            LogToDebugConsole("  Group 0009, Block 11:");
            LogToDebugConsole("    (0009,0011) = Private Creator Identification");
            LogToDebugConsole("    (0009,1100) through (0009,11FF) = Private data elements");
            LogToDebugConsole("");

            LogToDebugConsole("Example from a GE scanner:");
            LogToDebugConsole("  (0009,0010) LO \"GEMS_IDEN_01\"         <- Creator ID for block 10");
            LogToDebugConsole("  (0009,1001) LO \"CT01\"                  <- Product ID");
            LogToDebugConsole("  (0009,1002) SH \"CT Lightspeed\"         <- Scanner model");
            LogToDebugConsole("");

            LogToDebugConsole("Example from a Siemens scanner:");
            LogToDebugConsole("  (0019,0010) LO \"SIEMENS MR HEADER\"    <- Creator ID");
            LogToDebugConsole("  (0019,100C) IS 1                        <- Gradient mode");
            LogToDebugConsole("  (0019,100F) DS 2.3                      <- Flow compensation");
        }

        /// <summary>
        /// Demonstrate creating private tags
        /// </summary>
        private static void DemonstrateCreatingPrivateTags()
        {
            var dataset = new DicomDataset();

            // Add standard attributes first
            dataset.Add(DicomTag.PatientName, "Doe^John");
            dataset.Add(DicomTag.PatientID, "PAT123");

            //-----------------------------------------------------------------------
            // Step 1: Reserve a private block
            // Use group 0009, block 10 (element 0010)
            //-----------------------------------------------------------------------
            var creatorTag = new DicomTag(0x0009, 0x0010);
            dataset.Add(creatorTag, "MY_APPLICATION");  // Your application identifier

            LogToDebugConsole("Step 1: Reserve private block");
            LogToDebugConsole("  Tag: (0009,0010)");
            LogToDebugConsole("  Creator: MY_APPLICATION");
            LogToDebugConsole("");

            //-----------------------------------------------------------------------
            // Step 2: Add private data elements
            //-----------------------------------------------------------------------
            // Private element (0009,1000) - block 10, element 00
            var privateTag1 = new DicomTag(0x0009, 0x1000);
            dataset.Add(privateTag1, "Custom Value 1");

            // Private element (0009,1001) - block 10, element 01
            var privateTag2 = new DicomTag(0x0009, 0x1001);
            dataset.Add(privateTag2, "Custom Value 2");

            LogToDebugConsole("Step 2: Add private data elements");
            LogToDebugConsole("  (0009,1000) = \"Custom Value 1\"");
            LogToDebugConsole("  (0009,1001) = \"Custom Value 2\"");
            LogToDebugConsole("");

            //-----------------------------------------------------------------------
            // Step 3: Use another block if needed
            //-----------------------------------------------------------------------
            var creator2Tag = new DicomTag(0x0009, 0x0011);
            dataset.Add(creator2Tag, "MY_APP_EXTENDED");

            var privateTag3 = new DicomTag(0x0009, 0x1100);
            dataset.Add(privateTag3, "Extended data");

            LogToDebugConsole("Step 3: Use additional block if needed");
            LogToDebugConsole("  (0009,0011) Creator: MY_APP_EXTENDED");
            LogToDebugConsole("  (0009,1100) = \"Extended data\"");
            LogToDebugConsole("");

            // Show the result
            LogToDebugConsole("Reading back private tags:");
            LogToDebugConsole($"  (0009,0010) = {dataset.GetSingleValueOrDefault(creatorTag, "")}");
            LogToDebugConsole($"  (0009,1000) = {dataset.GetSingleValueOrDefault(privateTag1, "")}");
            LogToDebugConsole($"  (0009,1001) = {dataset.GetSingleValueOrDefault(privateTag2, "")}");
        }

        /// <summary>
        /// Demonstrate common vendor private tags
        /// </summary>
        private static void DemonstrateVendorPrivateTags()
        {
            LogToDebugConsole("=== Common Vendor Private Tags ===");
            LogToDebugConsole("");

            LogToDebugConsole("GE Healthcare:");
            LogToDebugConsole("  Creator: GEMS_IDEN_01, GEMS_ACQU_01, GEMS_IMAG_01");
            LogToDebugConsole("  Groups: 0009, 0019, 0021, 0023, 0025, 0027, 0043, 0045");
            LogToDebugConsole("  Contains: Scanner parameters, reconstruction settings");
            LogToDebugConsole("");

            LogToDebugConsole("Siemens Healthineers:");
            LogToDebugConsole("  Creator: SIEMENS MR HEADER, SIEMENS CT VA0, etc.");
            LogToDebugConsole("  Groups: 0019, 0021, 0029, 0051, 7FE1");
            LogToDebugConsole("  Contains: Sequence parameters, CSA headers (MR)");
            LogToDebugConsole("");

            LogToDebugConsole("Philips Healthcare:");
            LogToDebugConsole("  Creator: PHILIPS MR, Philips Imaging DD 001");
            LogToDebugConsole("  Groups: 2001, 2005, 7053");
            LogToDebugConsole("  Contains: Scanning parameters, Private Pixel Data");
            LogToDebugConsole("");

            LogToDebugConsole("Canon (formerly Toshiba):");
            LogToDebugConsole("  Creator: TOSHIBA_MEC_CT_01, TOSHIBA_MEC_MR_01");
            LogToDebugConsole("  Groups: 7005, 700D");
            LogToDebugConsole("  Contains: Acquisition parameters");
            LogToDebugConsole("");

            LogToDebugConsole("Useful Private Tag Databases:");
            LogToDebugConsole("  - DICOM Innolitics: https://dicom.innolitics.com/");
            LogToDebugConsole("  - dicom.offis.de/dcmtk private tags");
            LogToDebugConsole("  - Grassroots DICOM wiki");
        }

        /// <summary>
        /// Best practices for private tags
        /// </summary>
        private static void DemonstrateBestPractices()
        {
            LogToDebugConsole("=== Private Tag Best Practices ===");
            LogToDebugConsole("");

            LogToDebugConsole("DO:");
            LogToDebugConsole("  1. Always register a Private Creator Identification");
            LogToDebugConsole("  2. Use unique, identifiable creator names");
            LogToDebugConsole("  3. Document your private tag definitions");
            LogToDebugConsole("  4. Use appropriate VRs for your data");
            LogToDebugConsole("  5. Consider using standard attributes first");
            LogToDebugConsole("  6. Check if a standard tag already exists");
            LogToDebugConsole("");

            LogToDebugConsole("DON'T:");
            LogToDebugConsole("  1. Use private tags without a creator identification");
            LogToDebugConsole("  2. Assume private tags will survive transfer");
            LogToDebugConsole("  3. Store PHI in private tags without proper handling");
            LogToDebugConsole("  4. Use even group numbers for private data");
            LogToDebugConsole("  5. Conflict with known vendor private tags");
            LogToDebugConsole("");

            LogToDebugConsole("Reading Private Tags Safely:");
            LogToDebugConsole("  // Check for creator first");
            LogToDebugConsole("  var creator = dataset.GetSingleValueOrDefault<string>(");
            LogToDebugConsole("      new DicomTag(0x0009, 0x0010), null);");
            LogToDebugConsole("  if (creator == \"MY_APP\") {");
            LogToDebugConsole("      // Safe to read our private data");
            LogToDebugConsole("      var data = dataset.GetSingleValueOrDefault<string>(");
            LogToDebugConsole("          new DicomTag(0x0009, 0x1000), null);");
            LogToDebugConsole("  }");
            LogToDebugConsole("");

            LogToDebugConsole("Anonymization Consideration:");
            LogToDebugConsole("  Private tags may contain PHI!");
            LogToDebugConsole("  Options:");
            LogToDebugConsole("    1. Remove all private tags during de-identification");
            LogToDebugConsole("    2. Selectively remove known PHI-containing tags");
            LogToDebugConsole("    3. Document private tag contents in policy");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
