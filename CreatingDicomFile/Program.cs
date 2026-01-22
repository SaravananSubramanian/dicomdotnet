//-----------------------------------------------------------------------
// Tutorial: Creating a DICOM File from Scratch
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to create a new DICOM file programmatically,
//   including setting required patient, study, series, and image
//   module attributes.
//
// Key Concepts:
//   - DICOM files must contain minimum required attributes (modules)
//   - Patient Module: Patient Name, Patient ID
//   - Study Module: Study Instance UID, Study ID, Study Date
//   - Series Module: Series Instance UID, Series Number, Modality
//   - SOP Common Module: SOP Class UID, SOP Instance UID
//   - Secondary Capture (SC) is used for non-native DICOM images
//
// Requirements:
//   - Source image file (optional): Place a .jpg or .png in "Test Files"
//   - No external server connection required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomFile - Represents a complete DICOM file
//   - DicomUID.Generate() - Creates unique identifiers
//   - DicomTag - Standard DICOM tag definitions
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom;
using Dicom.Imaging;

namespace Com.SaravananSubramanian.CreatingDicomFile
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Output path for the created DICOM file
        //-----------------------------------------------------------------------
        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        private static readonly string OutputDicomFile =
            Path.Combine(OutputPath, "created_dicom_file.dcm");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Creating DICOM File Tutorial ===");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                    LogToDebugConsole($"Created output directory: {OutputPath}");
                }

                // Create a new DICOM dataset
                var dataset = new DicomDataset();

                //-----------------------------------------------------------------------
                // Patient Module (Required)
                // These attributes identify the patient
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding Patient Module attributes...");
                dataset.Add(DicomTag.PatientName, "Doe^John");
                dataset.Add(DicomTag.PatientID, "PAT123456");
                dataset.Add(DicomTag.PatientBirthDate, "19800101");
                dataset.Add(DicomTag.PatientSex, "M");

                //-----------------------------------------------------------------------
                // General Study Module (Required)
                // These attributes describe the imaging study/exam
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding Study Module attributes...");
                // Generate a unique Study Instance UID
                var studyInstanceUid = DicomUID.Generate();
                dataset.Add(DicomTag.StudyInstanceUID, studyInstanceUid);
                dataset.Add(DicomTag.StudyID, "STUDY001");
                dataset.Add(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));
                dataset.Add(DicomTag.StudyTime, DateTime.Now.ToString("HHmmss"));
                dataset.Add(DicomTag.AccessionNumber, "ACC123456");
                dataset.Add(DicomTag.ReferringPhysicianName, "Smith^Jane^Dr");
                dataset.Add(DicomTag.StudyDescription, "Sample Study Created by fo-dicom");

                //-----------------------------------------------------------------------
                // General Series Module (Required)
                // These attributes describe the series within the study
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding Series Module attributes...");
                // Generate a unique Series Instance UID
                var seriesInstanceUid = DicomUID.Generate();
                dataset.Add(DicomTag.SeriesInstanceUID, seriesInstanceUid);
                dataset.Add(DicomTag.SeriesNumber, "1");
                dataset.Add(DicomTag.SeriesDate, DateTime.Now.ToString("yyyyMMdd"));
                dataset.Add(DicomTag.SeriesTime, DateTime.Now.ToString("HHmmss"));
                dataset.Add(DicomTag.Modality, "OT");  // OT = Other
                dataset.Add(DicomTag.SeriesDescription, "Sample Series");

                //-----------------------------------------------------------------------
                // General Image Module (Required)
                // These attributes describe the image instance
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding Image Module attributes...");
                dataset.Add(DicomTag.InstanceNumber, "1");
                dataset.Add(DicomTag.ContentDate, DateTime.Now.ToString("yyyyMMdd"));
                dataset.Add(DicomTag.ContentTime, DateTime.Now.ToString("HHmmss"));

                //-----------------------------------------------------------------------
                // SOP Common Module (Required)
                // These attributes uniquely identify this DICOM object
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding SOP Common Module attributes...");
                // Secondary Capture SOP Class - used for images not from a medical device
                dataset.Add(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
                // Generate a unique SOP Instance UID for this image
                var sopInstanceUid = DicomUID.Generate();
                dataset.Add(DicomTag.SOPInstanceUID, sopInstanceUid);

                //-----------------------------------------------------------------------
                // Image Pixel Module (Required for images with pixel data)
                // For this example, we create a simple 256x256 grayscale image
                //-----------------------------------------------------------------------
                LogToDebugConsole("Adding Image Pixel Module attributes...");
                const int rows = 256;
                const int columns = 256;

                dataset.Add(DicomTag.Rows, (ushort)rows);
                dataset.Add(DicomTag.Columns, (ushort)columns);
                dataset.Add(DicomTag.BitsAllocated, (ushort)8);
                dataset.Add(DicomTag.BitsStored, (ushort)8);
                dataset.Add(DicomTag.HighBit, (ushort)7);
                dataset.Add(DicomTag.PixelRepresentation, (ushort)0);  // 0 = unsigned
                dataset.Add(DicomTag.SamplesPerPixel, (ushort)1);      // 1 = grayscale
                dataset.Add(DicomTag.PhotometricInterpretation, "MONOCHROME2");

                // Create a simple gradient pattern for pixel data
                var pixelData = new byte[rows * columns];
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        // Create a diagonal gradient pattern
                        pixelData[y * columns + x] = (byte)((x + y) % 256);
                    }
                }

                // Add the pixel data to the dataset
                var pixelDataElement = new DicomOtherByte(DicomTag.PixelData, pixelData);
                dataset.Add(pixelDataElement);

                //-----------------------------------------------------------------------
                // Create and save the DICOM file
                //-----------------------------------------------------------------------
                LogToDebugConsole("");
                LogToDebugConsole("Creating DICOM file...");

                // Create the DICOM file from the dataset
                var dicomFile = new DicomFile(dataset);

                // Save to disk
                dicomFile.Save(OutputDicomFile);

                LogToDebugConsole($"DICOM file created successfully!");
                LogToDebugConsole($"Output file: {OutputDicomFile}");
                LogToDebugConsole("");

                // Display summary of created file
                LogToDebugConsole("--- Created File Summary ---");
                LogToDebugConsole($"  Patient Name:      {dataset.GetSingleValueOrDefault(DicomTag.PatientName, "")}");
                LogToDebugConsole($"  Patient ID:        {dataset.GetSingleValueOrDefault(DicomTag.PatientID, "")}");
                LogToDebugConsole($"  Study Instance UID: {studyInstanceUid}");
                LogToDebugConsole($"  Series Instance UID: {seriesInstanceUid}");
                LogToDebugConsole($"  SOP Instance UID:  {sopInstanceUid}");
                LogToDebugConsole($"  Image Size:        {rows} x {columns} pixels");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error creating DICOM file: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}