//-----------------------------------------------------------------------
// Tutorial: DICOM Key Object Selection (KOS)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates Key Object Selection documents used to mark significant
//   images for teaching, quality assurance, or clinical purposes.
//
// Key Concepts:
//   - KOS SOP Class: 1.2.840.10008.5.1.4.1.1.88.59
//   - Based on DICOM Structured Report (SR) format
//   - Contains references to selected images
//   - Includes purpose/reason codes from CID 7010
//   - Modality = KO (Key Object)
//
// Use Cases:
//   - Teaching Files: Mark exemplary cases
//   - Quality Assurance: Flag images for review
//   - Clinical: Highlight key findings
//   - Manifest: List images for transmission
//   - Rejection: Mark images as rejected for quality
//
// Requirements:
//   - No external server connection required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomSequence - For content tree structure
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomKeyObjectSelection
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration
        //-----------------------------------------------------------------------
        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Key Object Selection Demo ===");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Demo 1: Create a Key Object Selection document
                LogToDebugConsole("--- Demo 1: Creating Key Object Selection ---");
                CreateKeyObjectSelectionDemo();

                // Demo 2: KOS concepts
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 2: KOS Concepts ---");
                DemonstrateKOSConcepts();

                // Demo 3: Purpose codes
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 3: Purpose Codes (CID 7010) ---");
                DemonstratePurposeCodes();

                // Demo 4: Use cases
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 4: Use Cases ---");
                DemonstrateUseCases();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Create a Key Object Selection document
        /// </summary>
        private static void CreateKeyObjectSelectionDemo()
        {
            var dataset = new DicomDataset();

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HHmmss");

            // Referenced images (example UIDs)
            string referencedStudyUID = "1.2.3.4.5.6.7.8.9";
            string referencedSeriesUID = "1.2.3.4.5.6.7.8.9.1";
            string[] referencedSOPInstanceUIDs = {
                "1.2.3.4.5.6.7.8.9.1.1",
                "1.2.3.4.5.6.7.8.9.1.5",
                "1.2.3.4.5.6.7.8.9.1.10"
            };

            //-----------------------------------------------------------------------
            // SOP Common Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.SOPClassUID, DicomUID.KeyObjectSelectionDocumentStorage);
            dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());

            //-----------------------------------------------------------------------
            // Patient Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.PatientName, "Doe^John");
            dataset.Add(DicomTag.PatientID, "PAT123");
            dataset.Add(DicomTag.PatientBirthDate, "19700101");
            dataset.Add(DicomTag.PatientSex, "M");

            //-----------------------------------------------------------------------
            // General Study Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.StudyInstanceUID, referencedStudyUID);
            dataset.Add(DicomTag.StudyDate, currentDate);
            dataset.Add(DicomTag.StudyTime, currentTime);
            dataset.Add(DicomTag.AccessionNumber, "ACC123");
            dataset.Add(DicomTag.ReferringPhysicianName, "Smith^Jane^Dr");
            dataset.Add(DicomTag.StudyID, "STUDY001");

            //-----------------------------------------------------------------------
            // KOS Series Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.Modality, "KO"); // Key Object
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "999");
            dataset.Add(DicomTag.SeriesDescription, "Key Object Selection");

            //-----------------------------------------------------------------------
            // SR Document General Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.InstanceNumber, "1");
            dataset.Add(DicomTag.ContentDate, currentDate);
            dataset.Add(DicomTag.ContentTime, currentTime);

            //-----------------------------------------------------------------------
            // Referenced Series Sequence
            //-----------------------------------------------------------------------
            var referencedSeriesSeq = new DicomSequence(DicomTag.ReferencedSeriesSequence);
            var seriesItem = new DicomDataset();
            seriesItem.Add(DicomTag.SeriesInstanceUID, referencedSeriesUID);

            // Referenced SOP Sequence (images in this series)
            var referencedSOPSeq = new DicomSequence(DicomTag.ReferencedSOPSequence);
            foreach (var instanceUID in referencedSOPInstanceUIDs)
            {
                var sopItem = new DicomDataset();
                sopItem.Add(DicomTag.ReferencedSOPClassUID, DicomUID.CTImageStorage);
                sopItem.Add(DicomTag.ReferencedSOPInstanceUID, instanceUID);
                referencedSOPSeq.Items.Add(sopItem);
            }
            seriesItem.Add(referencedSOPSeq);

            referencedSeriesSeq.Items.Add(seriesItem);
            dataset.Add(referencedSeriesSeq);

            //-----------------------------------------------------------------------
            // Content Sequence (SR structure for KOS)
            //-----------------------------------------------------------------------
            var contentSeq = new DicomSequence(DicomTag.ContentSequence);

            // Add purpose code item
            var purposeItem = new DicomDataset();
            purposeItem.Add(DicomTag.RelationshipType, "HAS CONCEPT MOD");
            purposeItem.Add(DicomTag.ValueType, "CODE");

            // Concept Name Code Sequence
            var conceptNameSeq = new DicomSequence(DicomTag.ConceptNameCodeSequence);
            var conceptNameItem = new DicomDataset();
            conceptNameItem.Add(DicomTag.CodeValue, "113012");
            conceptNameItem.Add(DicomTag.CodingSchemeDesignator, "DCM");
            conceptNameItem.Add(DicomTag.CodeMeaning, "Key Object Description");
            conceptNameSeq.Items.Add(conceptNameItem);
            purposeItem.Add(conceptNameSeq);

            // Concept Code Sequence (the actual purpose)
            var conceptCodeSeq = new DicomSequence(DicomTag.ConceptCodeSequence);
            var conceptCodeItem = new DicomDataset();
            conceptCodeItem.Add(DicomTag.CodeValue, "113000");
            conceptCodeItem.Add(DicomTag.CodingSchemeDesignator, "DCM");
            conceptCodeItem.Add(DicomTag.CodeMeaning, "Of Interest");
            conceptCodeSeq.Items.Add(conceptCodeItem);
            purposeItem.Add(conceptCodeSeq);

            contentSeq.Items.Add(purposeItem);
            dataset.Add(contentSeq);

            //-----------------------------------------------------------------------
            // Save the file
            //-----------------------------------------------------------------------
            string outputFile = Path.Combine(OutputPath, "key_object_selection.dcm");
            var dicomFile = new DicomFile(dataset);
            dicomFile.Save(outputFile);

            LogToDebugConsole($"Key Object Selection created: {outputFile}");
            LogToDebugConsole($"Number of referenced images: {referencedSOPInstanceUIDs.Length}");
            LogToDebugConsole("Purpose: Of Interest");
        }

        /// <summary>
        /// Demonstrate KOS concepts
        /// </summary>
        private static void DemonstrateKOSConcepts()
        {
            LogToDebugConsole("Key Object Selection Structure:");
            LogToDebugConsole("");
            LogToDebugConsole("  SOP Class UID: 1.2.840.10008.5.1.4.1.1.88.59");
            LogToDebugConsole("  Modality: KO (Key Object)");
            LogToDebugConsole("");
            LogToDebugConsole("  Based on DICOM SR (Structured Report):");
            LogToDebugConsole("    - Root container with template ID");
            LogToDebugConsole("    - Purpose/flag codes");
            LogToDebugConsole("    - References to selected images");
            LogToDebugConsole("");
            LogToDebugConsole("  Key Sequences:");
            LogToDebugConsole("    - Referenced Series Sequence");
            LogToDebugConsole("    - Referenced SOP Sequence");
            LogToDebugConsole("    - Content Sequence (SR structure)");
        }

        /// <summary>
        /// Demonstrate purpose codes
        /// </summary>
        private static void DemonstratePurposeCodes()
        {
            LogToDebugConsole("Common KOS Purpose Codes (CID 7010):");
            LogToDebugConsole("");
            LogToDebugConsole("Code Value | Meaning");
            LogToDebugConsole("-----------|--------------------------------");
            LogToDebugConsole("113000     | Of Interest");
            LogToDebugConsole("113001     | Rejected for Quality Reasons");
            LogToDebugConsole("113002     | For Referring Provider");
            LogToDebugConsole("113003     | For Surgery");
            LogToDebugConsole("113004     | For Teaching");
            LogToDebugConsole("113005     | For Conference");
            LogToDebugConsole("113006     | For Therapy");
            LogToDebugConsole("113007     | For Patient");
            LogToDebugConsole("113008     | For Peer Review");
            LogToDebugConsole("113009     | For Research");
            LogToDebugConsole("113010     | Quality Issue");
            LogToDebugConsole("113013     | Best In Set");
            LogToDebugConsole("113018     | For Printing");
            LogToDebugConsole("113020     | For Report Attachment");
            LogToDebugConsole("113030     | Manifest");
            LogToDebugConsole("113031     | Signed Manifest");
            LogToDebugConsole("113032     | Complete Study Content");
            LogToDebugConsole("");

            LogToDebugConsole("Rejection Reason Codes (CID 7011):");
            LogToDebugConsole("113001     | Rejected for Quality Reasons");
            LogToDebugConsole("113037     | Rejected for Patient Safety");
            LogToDebugConsole("113038     | Incorrect Modality Worklist Entry");
            LogToDebugConsole("113039     | Data Retention Policy Expired");
        }

        /// <summary>
        /// Demonstrate use cases
        /// </summary>
        private static void DemonstrateUseCases()
        {
            LogToDebugConsole("1. Teaching File Export:");
            LogToDebugConsole("   - Radiologist marks interesting cases");
            LogToDebugConsole("   - KOS document lists all selected images");
            LogToDebugConsole("   - Export system retrieves images listed in KOS");
            LogToDebugConsole("");

            LogToDebugConsole("2. Quality Rejection:");
            LogToDebugConsole("   - Technologist marks motion-blurred image");
            LogToDebugConsole("   - KOS with rejection code created");
            LogToDebugConsole("   - PACS hides rejected images from display");
            LogToDebugConsole("");

            LogToDebugConsole("3. Referring Physician Report:");
            LogToDebugConsole("   - Radiologist marks key finding images");
            LogToDebugConsole("   - KOS attached to radiology report");
            LogToDebugConsole("   - Physician sees most important images first");
            LogToDebugConsole("");

            LogToDebugConsole("4. Study Manifest:");
            LogToDebugConsole("   - List all images being sent to another system");
            LogToDebugConsole("   - Receiver can verify all images received");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
