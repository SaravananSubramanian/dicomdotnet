//-----------------------------------------------------------------------
// Tutorial: DICOM Structured Reports (SR)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to read and create DICOM Structured Reports,
//   which encode clinical findings, measurements, and observations.
//
// Key Concepts:
//   - SR: Tree-structured clinical content (not images)
//   - Contains coded concepts, measurements, and references to images
//   - Supports CAD results, dose reports, key images, and more
//   - Structure: Container > Content Items with coded values
//
// SR Content Types:
//   | Type       | Contains                                    |
//   |------------|---------------------------------------------|
//   | CODE       | Coded concept (code meaning + scheme)       |
//   | NUM        | Numeric measurement with units              |
//   | TEXT       | Free-text description                       |
//   | IMAGE      | Reference to DICOM image                    |
//   | UIDREF     | Reference by UID                            |
//   | CONTAINER  | Group of related content items              |
//
// Common SR SOP Classes:
//   - Basic Text SR (1.2.840.10008.5.1.4.1.1.88.11)
//   - Enhanced SR (1.2.840.10008.5.1.4.1.1.88.22)
//   - Comprehensive SR (1.2.840.10008.5.1.4.1.1.88.33)
//   - X-Ray Radiation Dose SR
//   - Mammography CAD SR
//
// Requirements:
//   - SR DICOM file: Place a SR .dcm file in "Test Files" folder
//   - SR files are available from radiology teaching files
//   - Or create using this tutorial's code
//
// fo-dicom References:
//   - DicomFile.Open() - Open SR files like any DICOM
//   - DicomSequence (0040,A730) - Content Sequence navigation
//   - DicomDataset iteration for SR tree structure
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom;

namespace UnderstandingDicomStructuredReports
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to DICOM SR file
        // NOTE: Place a Structured Report .dcm file here, or run this
        //       to create a sample SR file
        //-----------------------------------------------------------------------
        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        private static readonly string OutputSrFile =
            Path.Combine(OutputPath, "sample_structured_report.dcm");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Structured Reports Tutorial ===");
                LogToDebugConsole("");
                LogToDebugConsole("--- Overview ---");
                LogToDebugConsole("Structured Reports encode clinical findings as coded content,");
                LogToDebugConsole("not images. They contain measurements, observations, and");
                LogToDebugConsole("references to related images.");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Create a sample Basic Text SR
                CreateSampleStructuredReport();

                // Read and display the created SR
                ReadStructuredReport(OutputSrFile);

                LogToDebugConsole("");
                LogToDebugConsole("Structured Report tutorial completed.");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Creates a sample Basic Text Structured Report.
        /// </summary>
        private static void CreateSampleStructuredReport()
        {
            LogToDebugConsole("--- Creating Sample Structured Report ---");
            LogToDebugConsole("");

            var dataset = new DicomDataset();

            //-----------------------------------------------------------------------
            // Patient Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.PatientName, "Doe^John");
            dataset.Add(DicomTag.PatientID, "SR123456");
            dataset.Add(DicomTag.PatientBirthDate, "19800101");
            dataset.Add(DicomTag.PatientSex, "M");

            //-----------------------------------------------------------------------
            // Study Module
            //-----------------------------------------------------------------------
            var studyUid = DicomUID.Generate();
            dataset.Add(DicomTag.StudyInstanceUID, studyUid);
            dataset.Add(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.StudyTime, DateTime.Now.ToString("HHmmss"));
            dataset.Add(DicomTag.AccessionNumber, "SR-ACC-001");
            dataset.Add(DicomTag.StudyDescription, "Chest CT Findings Report");

            //-----------------------------------------------------------------------
            // Series Module
            //-----------------------------------------------------------------------
            var seriesUid = DicomUID.Generate();
            dataset.Add(DicomTag.SeriesInstanceUID, seriesUid);
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.Modality, "SR");  // Structured Report modality
            dataset.Add(DicomTag.SeriesDescription, "CT Findings SR");

            //-----------------------------------------------------------------------
            // SOP Common Module - Basic Text SR
            //-----------------------------------------------------------------------
            var sopUid = DicomUID.Generate();
            dataset.Add(DicomTag.SOPClassUID, DicomUID.BasicTextSRStorage);
            dataset.Add(DicomTag.SOPInstanceUID, sopUid);
            dataset.Add(DicomTag.InstanceNumber, "1");

            //-----------------------------------------------------------------------
            // SR Document General Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.InstanceCreationDate, DateTime.Now.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.InstanceCreationTime, DateTime.Now.ToString("HHmmss"));
            dataset.Add(DicomTag.ContentDate, DateTime.Now.ToString("yyyyMMdd"));
            dataset.Add(DicomTag.ContentTime, DateTime.Now.ToString("HHmmss"));

            // Completion Flag: COMPLETE or PARTIAL
            dataset.Add(DicomTag.CompletionFlag, "COMPLETE");
            // Verification Flag: VERIFIED or UNVERIFIED
            dataset.Add(DicomTag.VerificationFlag, "UNVERIFIED");

            //-----------------------------------------------------------------------
            // SR Document Content Module - Content Sequence
            // This is where the structured content goes
            //-----------------------------------------------------------------------

            // Create root container content item
            var contentSequence = new DicomSequence(DicomTag.ContentSequence);

            // Add a TEXT content item with findings
            var findingsItem = new DicomDataset();
            findingsItem.Add(DicomTag.RelationshipType, "CONTAINS");
            findingsItem.Add(DicomTag.ValueType, "TEXT");

            // Concept Name Code Sequence - What this item represents
            var conceptNameSeq = new DicomSequence(DicomTag.ConceptNameCodeSequence);
            var conceptName = new DicomDataset();
            conceptName.Add(DicomTag.CodeValue, "121071");
            conceptName.Add(DicomTag.CodingSchemeDesignator, "DCM");
            conceptName.Add(DicomTag.CodeMeaning, "Finding");
            conceptNameSeq.Items.Add(conceptName);
            findingsItem.Add(conceptNameSeq);

            // The actual text value
            findingsItem.Add(DicomTag.TextValue, "No acute cardiopulmonary abnormality. " +
                "Clear lung fields bilaterally. Heart size within normal limits.");

            contentSequence.Items.Add(findingsItem);
            dataset.Add(contentSequence);

            //-----------------------------------------------------------------------
            // Save the Structured Report
            //-----------------------------------------------------------------------
            var dicomFile = new DicomFile(dataset);
            dicomFile.Save(OutputSrFile);

            LogToDebugConsole($"Created SR file: {OutputSrFile}");
            LogToDebugConsole("");
        }

        /// <summary>
        /// Reads and displays a Structured Report.
        /// </summary>
        private static void ReadStructuredReport(string filePath)
        {
            LogToDebugConsole("--- Reading Structured Report ---");
            LogToDebugConsole("");

            var file = DicomFile.Open(filePath);
            var dataset = file.Dataset;

            LogToDebugConsole($"  SOP Class: {dataset.GetSingleValueOrDefault(DicomTag.SOPClassUID, "")}");
            LogToDebugConsole($"  Modality:  {dataset.GetSingleValueOrDefault(DicomTag.Modality, "")}");
            LogToDebugConsole($"  Patient:   {dataset.GetSingleValueOrDefault(DicomTag.PatientName, "")}");
            LogToDebugConsole($"  Study:     {dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "")}");
            LogToDebugConsole("");

            // Navigate Content Sequence
            var contentSequence = dataset.GetSequence(DicomTag.ContentSequence);
            if (contentSequence != null)
            {
                LogToDebugConsole("  Content Items:");
                foreach (var item in contentSequence.Items)
                {
                    var valueType = item.GetSingleValueOrDefault(DicomTag.ValueType, "");
                    var relationshipType = item.GetSingleValueOrDefault(DicomTag.RelationshipType, "");

                    LogToDebugConsole($"    - Type: {valueType}, Relationship: {relationshipType}");

                    if (valueType == "TEXT")
                    {
                        var textValue = item.GetSingleValueOrDefault(DicomTag.TextValue, "");
                        LogToDebugConsole($"      Text: {textValue}");
                    }
                }
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
