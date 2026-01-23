//-----------------------------------------------------------------------
// Tutorial: DICOM Encapsulated Documents (PDF, CDA, STL)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to create DICOM objects that encapsulate non-DICOM
//   documents such as PDFs, allowing storage alongside images in PACS.
//
// Key Concepts:
//   - Encapsulated PDF Storage SOP Class: 1.2.840.10008.5.1.4.1.1.104.1
//   - Encapsulated CDA Storage: 1.2.840.10008.5.1.4.1.1.104.2
//   - Encapsulated STL Storage: 1.2.840.10008.5.1.4.1.1.104.3 (3D printing)
//   - Modality = DOC for documents
//   - MIME Type specifies document format
//   - Burned In Annotation indicates PHI in document
//
// Requirements:
//   - Optional: PDF file to encapsulate
//   - No external server connection required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomFile - For saving the encapsulated document
//   - DicomTag.EncapsulatedDocument - Contains the PDF bytes
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomEncapsulatedDocuments
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
                LogToDebugConsole("=== DICOM Encapsulated Documents Demo ===");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Demo 1: Create an encapsulated PDF (with sample data)
                LogToDebugConsole("--- Demo 1: Creating Encapsulated PDF ---");
                CreateEncapsulatedPdfDemo();

                // Demo 2: Show encapsulated document structure
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 2: Encapsulated Document Structure ---");
                DemonstrateEncapsulatedPdfStructure();

                // Demo 3: Document types reference
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 3: Supported Document Types ---");
                DemonstrateSupportedDocumentTypes();

                // Demo 4: Privacy considerations
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 4: Privacy Considerations ---");
                DemonstratePrivacyConsiderations();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Create an encapsulated PDF DICOM object
        /// </summary>
        private static void CreateEncapsulatedPdfDemo()
        {
            // Create sample PDF data (in real use, read from actual PDF file)
            // This is just placeholder data for demonstration
            byte[] samplePdfData = CreateSamplePdfPlaceholder();

            var dataset = new DicomDataset();

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HHmmss");

            //-----------------------------------------------------------------------
            // SOP Common Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.SOPClassUID, DicomUID.EncapsulatedPDFStorage);
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
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.StudyDate, currentDate);
            dataset.Add(DicomTag.StudyTime, currentTime);
            dataset.Add(DicomTag.AccessionNumber, "ACC456");
            dataset.Add(DicomTag.ReferringPhysicianName, "Smith^Jane^Dr");
            dataset.Add(DicomTag.StudyID, "STUDY001");

            //-----------------------------------------------------------------------
            // Encapsulated Document Series Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.Modality, "DOC"); // Document
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "1");

            //-----------------------------------------------------------------------
            // SC Equipment Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.ConversionType, "WSD"); // Workstation

            //-----------------------------------------------------------------------
            // Encapsulated Document Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.InstanceNumber, "1");
            dataset.Add(DicomTag.ContentDate, currentDate);
            dataset.Add(DicomTag.ContentTime, currentTime);
            dataset.Add(DicomTag.AcquisitionDateTime, currentDate + currentTime);

            // Burned In Annotation - important for PHI
            dataset.Add(DicomTag.BurnedInAnnotation, "YES"); // PDF likely contains patient info

            // Document Title
            dataset.Add(DicomTag.DocumentTitle, "Chest CT Report");

            // MIME Type
            dataset.Add(DicomTag.MIMETypeOfEncapsulatedDocument, "application/pdf");

            // Encapsulated Document (the actual PDF bytes)
            dataset.Add(new DicomOtherByte(DicomTag.EncapsulatedDocument, samplePdfData));

            //-----------------------------------------------------------------------
            // Save the file
            //-----------------------------------------------------------------------
            string outputFile = Path.Combine(OutputPath, "encapsulated_pdf.dcm");
            var dicomFile = new DicomFile(dataset);
            dicomFile.Save(outputFile);

            LogToDebugConsole($"Encapsulated PDF created: {outputFile}");
            LogToDebugConsole($"Document size: {samplePdfData.Length} bytes");
            LogToDebugConsole("This DICOM object can now be stored in PACS.");
        }

        /// <summary>
        /// Create a sample PDF placeholder (not a real PDF)
        /// </summary>
        private static byte[] CreateSamplePdfPlaceholder()
        {
            // In real use, you would read an actual PDF file:
            // return File.ReadAllBytes(pdfFilePath);

            // This is just a placeholder for demonstration
            return new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF header
        }

        /// <summary>
        /// Demonstrate encapsulated PDF structure
        /// </summary>
        private static void DemonstrateEncapsulatedPdfStructure()
        {
            LogToDebugConsole("Key Attributes for Encapsulated PDF:");
            LogToDebugConsole("");
            LogToDebugConsole("  (0008,0016) SOP Class UID = 1.2.840.10008.5.1.4.1.1.104.1");
            LogToDebugConsole("  (0008,0060) Modality = DOC");
            LogToDebugConsole("  (0008,0064) Conversion Type = WSD");
            LogToDebugConsole("  (0028,0301) Burned In Annotation = YES");
            LogToDebugConsole("  (0042,0010) Document Title = (your title)");
            LogToDebugConsole("  (0042,0012) MIME Type = application/pdf");
            LogToDebugConsole("  (0042,0011) Encapsulated Document = (PDF bytes)");
            LogToDebugConsole("");

            LogToDebugConsole("Required Modules:");
            LogToDebugConsole("  - Patient Module");
            LogToDebugConsole("  - General Study Module");
            LogToDebugConsole("  - Encapsulated Document Series Module");
            LogToDebugConsole("  - SC Equipment Module");
            LogToDebugConsole("  - Encapsulated Document Module");
            LogToDebugConsole("  - SOP Common Module");
        }

        /// <summary>
        /// Demonstrate supported document types
        /// </summary>
        private static void DemonstrateSupportedDocumentTypes()
        {
            LogToDebugConsole("Encapsulated PDF (1.2.840.10008.5.1.4.1.1.104.1):");
            LogToDebugConsole("  MIME Type: application/pdf");
            LogToDebugConsole("  Use: Reports, forms, documents");
            LogToDebugConsole("");

            LogToDebugConsole("Encapsulated CDA (1.2.840.10008.5.1.4.1.1.104.2):");
            LogToDebugConsole("  MIME Type: text/xml");
            LogToDebugConsole("  Use: HL7 Clinical Document Architecture");
            LogToDebugConsole("");

            LogToDebugConsole("Encapsulated STL (1.2.840.10008.5.1.4.1.1.104.3):");
            LogToDebugConsole("  MIME Type: model/stl");
            LogToDebugConsole("  Use: 3D printing models");
            LogToDebugConsole("");

            LogToDebugConsole("Encapsulated OBJ (1.2.840.10008.5.1.4.1.1.104.4):");
            LogToDebugConsole("  MIME Type: model/obj");
            LogToDebugConsole("  Use: 3D surface models");
            LogToDebugConsole("");

            LogToDebugConsole("Encapsulated MTL (1.2.840.10008.5.1.4.1.1.104.5):");
            LogToDebugConsole("  MIME Type: model/mtl");
            LogToDebugConsole("  Use: 3D model materials");
        }

        /// <summary>
        /// Demonstrate privacy considerations
        /// </summary>
        private static void DemonstratePrivacyConsiderations()
        {
            LogToDebugConsole("Privacy Considerations for Encapsulated Documents:");
            LogToDebugConsole("");
            LogToDebugConsole("1. Burned In Annotation");
            LogToDebugConsole("   - Set to YES if document contains PHI");
            LogToDebugConsole("   - Alerts downstream systems to sensitive content");
            LogToDebugConsole("");

            LogToDebugConsole("2. Document Content");
            LogToDebugConsole("   - PDF text cannot be anonymized by DICOM tools");
            LogToDebugConsole("   - Consider using redacted PDFs for teaching/research");
            LogToDebugConsole("   - May need manual review before sharing");
            LogToDebugConsole("");

            LogToDebugConsole("3. Document Title");
            LogToDebugConsole("   - Should not contain PHI in the title attribute");
            LogToDebugConsole("   - Use generic descriptions");
            LogToDebugConsole("");

            LogToDebugConsole("4. De-identification");
            LogToDebugConsole("   - Standard DICOM de-identification won't remove PDF content");
            LogToDebugConsole("   - Must process document separately if needed");
            LogToDebugConsole("   - Consider PDF redaction tools for sensitive content");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
