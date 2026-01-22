//-----------------------------------------------------------------------
// Tutorial: DICOM Segmentation Objects
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates DICOM Segmentation IOD concepts for storing
//   segmented image regions (organs, lesions, structures).
//
// Key Concepts:
//   - DICOM Segmentation IOD (1.2.840.10008.5.1.4.1.1.66.4)
//   - Binary vs Fractional segmentation types
//   - Segment sequences and attributes
//   - Anatomical and property categories
//   - Relationship to source images
//
// Background:
//   DICOM Segmentation objects store labeled regions extracted
//   from medical images. Common uses include:
//   - Organ contours from CT/MR
//   - Tumor volumes
//   - AI/ML inference results
//   - Quantitative analysis regions
//
// fo-dicom References:
//   - DicomDataset - Core data structure
//   - DicomSequence - For segment definitions
//   - DicomPixelData - For segmentation masks
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Dicom;

namespace Com.SaravananSubramanian.DicomSegmentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Segmentation Objects Demo ===");
                LogToDebugConsole("");

                // Demo 1: Segmentation IOD overview
                LogToDebugConsole("--- Segmentation IOD Overview ---");
                DemonstrateSegmentationIOD();

                // Demo 2: Segmentation types
                LogToDebugConsole("");
                LogToDebugConsole("--- Segmentation Types ---");
                DemonstrateSegmentationTypes();

                // Demo 3: Segment sequence
                LogToDebugConsole("");
                LogToDebugConsole("--- Segment Sequence Attributes ---");
                DemonstrateSegmentSequence();

                // Demo 4: Coded concepts
                LogToDebugConsole("");
                LogToDebugConsole("--- Coded Concepts for Segmentation ---");
                DemonstrateCodedConcepts();

                // Demo 5: Creating a segmentation
                LogToDebugConsole("");
                LogToDebugConsole("--- Creating Segmentation with fo-dicom ---");
                DemonstrateSegmentationCreation();

                // Demo 6: Multi-frame considerations
                LogToDebugConsole("");
                LogToDebugConsole("--- Multi-Frame Segmentation ---");
                DemonstrateMultiFrameSegmentation();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate Segmentation IOD overview
        /// </summary>
        private static void DemonstrateSegmentationIOD()
        {
            LogToDebugConsole("");
            LogToDebugConsole("DICOM Segmentation Storage SOP Class:");
            LogToDebugConsole("  UID: 1.2.840.10008.5.1.4.1.1.66.4");
            LogToDebugConsole("  Name: Segmentation Storage");
            LogToDebugConsole("");

            LogToDebugConsole("Purpose:");
            LogToDebugConsole("  - Store segmented regions from images");
            LogToDebugConsole("  - Label anatomical structures");
            LogToDebugConsole("  - Store AI/ML segmentation results");
            LogToDebugConsole("  - Support quantitative analysis");
            LogToDebugConsole("");

            LogToDebugConsole("Key Modules:");
            LogToDebugConsole("  - Patient Module (from referenced images)");
            LogToDebugConsole("  - General Study Module");
            LogToDebugConsole("  - Segmentation Series Module");
            LogToDebugConsole("  - General Equipment Module");
            LogToDebugConsole("  - Segmentation Image Module");
            LogToDebugConsole("  - Multi-frame Functional Groups Module");
            LogToDebugConsole("  - Multi-frame Dimension Module");
            LogToDebugConsole("");

            LogToDebugConsole("Important Attributes:");
            LogToDebugConsole("  (0062,0001) Segmentation Type");
            LogToDebugConsole("  (0062,0002) Segment Sequence");
            LogToDebugConsole("  (0062,0004) Segment Number");
            LogToDebugConsole("  (0062,0005) Segment Label");
            LogToDebugConsole("  (0062,0006) Segment Description");
            LogToDebugConsole("  (0062,0008) Segment Algorithm Type");
            LogToDebugConsole("  (0062,0009) Segment Algorithm Name");
            LogToDebugConsole("  (0062,000A) Segmentation Fractional Type");
            LogToDebugConsole("  (0062,000B) Segment Maximum Fractional Value");
        }

        /// <summary>
        /// Demonstrate segmentation types
        /// </summary>
        private static void DemonstrateSegmentationTypes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Segmentation Type (0062,0001):");
            LogToDebugConsole("");

            LogToDebugConsole("1. BINARY:");
            LogToDebugConsole("   - Each pixel is 0 or 1");
            LogToDebugConsole("   - Simple yes/no membership");
            LogToDebugConsole("   - Bits Allocated = 1");
            LogToDebugConsole("   - Common for contour-based segmentation");
            LogToDebugConsole("   - Efficient storage");
            LogToDebugConsole("");

            LogToDebugConsole("2. FRACTIONAL:");
            LogToDebugConsole("   - Pixel values 0 to MaxFractionalValue");
            LogToDebugConsole("   - Represents probability or occupancy");
            LogToDebugConsole("   - Bits Allocated = 8");
            LogToDebugConsole("   - Good for probabilistic segmentations");
            LogToDebugConsole("   - Used with partial volume effects");
            LogToDebugConsole("");

            LogToDebugConsole("Segmentation Fractional Type (0062,000A):");
            LogToDebugConsole("   PROBABILITY - Value represents probability");
            LogToDebugConsole("   OCCUPANCY - Value represents fractional occupancy");
        }

        /// <summary>
        /// Demonstrate segment sequence attributes
        /// </summary>
        private static void DemonstrateSegmentSequence()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Segment Sequence (0062,0002) contains:");
            LogToDebugConsole("");

            LogToDebugConsole("Required Attributes:");
            LogToDebugConsole("  (0062,0004) Segment Number - Unique integer ID");
            LogToDebugConsole("  (0062,0005) Segment Label - Short text name");
            LogToDebugConsole("  (0062,0008) Segment Algorithm Type:");
            LogToDebugConsole("              AUTOMATIC, SEMIAUTOMATIC, MANUAL");
            LogToDebugConsole("  (0008,0100)/(0008,0102)/(0008,0104) Segmented Property");
            LogToDebugConsole("              Category Code Sequence");
            LogToDebugConsole("  (0008,0100)/(0008,0102)/(0008,0104) Segmented Property");
            LogToDebugConsole("              Type Code Sequence");
            LogToDebugConsole("");

            LogToDebugConsole("Optional Attributes:");
            LogToDebugConsole("  (0062,0006) Segment Description");
            LogToDebugConsole("  (0062,0009) Segment Algorithm Name");
            LogToDebugConsole("  (006A,0005) Algorithm Family Code Sequence");
            LogToDebugConsole("  (0062,000D) Recommended Display Grayscale Value");
            LogToDebugConsole("  (0062,000C) Recommended Display CIELab Value");
            LogToDebugConsole("  (0008,2218) Anatomic Region Sequence");
            LogToDebugConsole("  (0008,2220) Anatomic Region Modifier Sequence");
            LogToDebugConsole("");

            LogToDebugConsole("Example Segment:");
            LogToDebugConsole("  Segment Number: 1");
            LogToDebugConsole("  Segment Label: \"Liver\"");
            LogToDebugConsole("  Algorithm Type: AUTOMATIC");
            LogToDebugConsole("  Property Category: Anatomical Structure (T-D0050)");
            LogToDebugConsole("  Property Type: Liver (T-62000)");
        }

        /// <summary>
        /// Demonstrate coded concepts for segmentation
        /// </summary>
        private static void DemonstrateCodedConcepts()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Segmented Property Category Code Sequence:");
            LogToDebugConsole("  CID 7150 - Segmentation Property Categories");
            LogToDebugConsole("");
            LogToDebugConsole("  Common Categories:");
            LogToDebugConsole("  | Code     | Meaning                    | Scheme    |");
            LogToDebugConsole("  |----------|----------------------------|-----------|");
            LogToDebugConsole("  | T-D0050  | Anatomical Structure       | SRT       |");
            LogToDebugConsole("  | M-01000  | Morphologically Altered    | SRT       |");
            LogToDebugConsole("  | T-D0080  | Body substance             | SRT       |");
            LogToDebugConsole("  | A-00004  | Physical object            | SRT       |");
            LogToDebugConsole("");

            LogToDebugConsole("Segmented Property Type Code Sequence:");
            LogToDebugConsole("  CID 7151-7166 - Various anatomical regions");
            LogToDebugConsole("");
            LogToDebugConsole("  Examples (using SNOMED-CT/SRT):");
            LogToDebugConsole("  | Code     | Meaning                    |");
            LogToDebugConsole("  |----------|----------------------------|");
            LogToDebugConsole("  | T-62000  | Liver                      |");
            LogToDebugConsole("  | T-71000  | Kidney                     |");
            LogToDebugConsole("  | T-28000  | Lung                       |");
            LogToDebugConsole("  | T-32000  | Heart                      |");
            LogToDebugConsole("  | T-D1100  | Head                       |");
            LogToDebugConsole("  | T-11100  | Bone                       |");
            LogToDebugConsole("  | T-A0100  | Brain                      |");
            LogToDebugConsole("");

            LogToDebugConsole("Coding Schemes:");
            LogToDebugConsole("  SRT - SNOMED-RT (SNOMED Reference Terminology)");
            LogToDebugConsole("  SCT - SNOMED-CT");
            LogToDebugConsole("  DCM - DICOM Controlled Terminology");
            LogToDebugConsole("  FMA - Foundational Model of Anatomy");
        }

        /// <summary>
        /// Demonstrate creating a segmentation with fo-dicom
        /// </summary>
        private static void DemonstrateSegmentationCreation()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Creating Segmentation with fo-dicom:");
            LogToDebugConsole("");

            // Create dataset
            var dataset = new DicomDataset();

            // Patient and Study (would normally copy from source)
            dataset.Add(DicomTag.PatientName, "Test^Patient");
            dataset.Add(DicomTag.PatientID, "12345");
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.StudyDate, DateTime.Now);
            dataset.Add(DicomTag.StudyTime, DateTime.Now);

            // Series Module
            dataset.Add(DicomTag.Modality, "SEG");
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "1");
            dataset.Add(DicomTag.SeriesDescription, "Segmentation");

            // SOP Common
            dataset.Add(DicomTag.SOPClassUID, DicomUID.SegmentationStorage);
            dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());

            LogToDebugConsole("Basic instance created:");
            LogToDebugConsole($"  SOP Class: {DicomUID.SegmentationStorage.Name}");
            LogToDebugConsole($"  Modality: SEG");
            LogToDebugConsole("");

            // Segmentation-specific attributes
            dataset.Add(new DicomCodeString(DicomTag.SegmentationType, "BINARY"));
            dataset.Add(DicomTag.ContentLabel, "SEGMENTATION");
            dataset.Add(DicomTag.ContentDescription, "Organ segmentation");
            dataset.Add(DicomTag.ContentCreatorName, "Algorithm^Auto");

            LogToDebugConsole("Segmentation attributes:");
            LogToDebugConsole("  Segmentation Type: BINARY");
            LogToDebugConsole("  Content Label: SEGMENTATION");
            LogToDebugConsole("");

            // Create segment sequence
            var segmentSequence = new DicomSequence(DicomTag.SegmentSequence);

            // Segment 1 - Liver
            var segment1 = new DicomDataset();
            segment1.Add(DicomTag.SegmentNumber, (ushort)1);
            segment1.Add(DicomTag.SegmentLabel, "Liver");
            segment1.Add(DicomTag.SegmentDescription, "Liver segmentation");
            segment1.Add(new DicomCodeString(DicomTag.SegmentAlgorithmType, "AUTOMATIC"));
            segment1.Add(DicomTag.SegmentAlgorithmName, "AutoSeg v1.0");

            // Segmented Property Category Code Sequence (Anatomical Structure)
            var categorySequence = new DicomSequence(DicomTag.SegmentedPropertyCategoryCodeSequence);
            var categoryItem = new DicomDataset();
            categoryItem.Add(DicomTag.CodeValue, "T-D0050");
            categoryItem.Add(DicomTag.CodingSchemeDesignator, "SRT");
            categoryItem.Add(DicomTag.CodeMeaning, "Anatomical Structure");
            categorySequence.Items.Add(categoryItem);
            segment1.Add(categorySequence);

            // Segmented Property Type Code Sequence (Liver)
            var typeSequence = new DicomSequence(DicomTag.SegmentedPropertyTypeCodeSequence);
            var typeItem = new DicomDataset();
            typeItem.Add(DicomTag.CodeValue, "T-62000");
            typeItem.Add(DicomTag.CodingSchemeDesignator, "SRT");
            typeItem.Add(DicomTag.CodeMeaning, "Liver");
            typeSequence.Items.Add(typeItem);
            segment1.Add(typeSequence);

            // Recommended display color (CIELab - brownish for liver)
            segment1.Add(DicomTag.RecommendedDisplayCIELabValue, new ushort[] { 39164, 43690, 28835 });

            segmentSequence.Items.Add(segment1);
            dataset.Add(segmentSequence);

            LogToDebugConsole("Segment Sequence created:");
            LogToDebugConsole("  Segment 1: Liver");
            LogToDebugConsole("    Algorithm Type: AUTOMATIC");
            LogToDebugConsole("    Category: Anatomical Structure (T-D0050)");
            LogToDebugConsole("    Type: Liver (T-62000)");
            LogToDebugConsole("");

            LogToDebugConsole("Note: Actual pixel data requires multi-frame setup");
            LogToDebugConsole("with per-frame functional groups and proper encoding.");
        }

        /// <summary>
        /// Demonstrate multi-frame segmentation considerations
        /// </summary>
        private static void DemonstrateMultiFrameSegmentation()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Multi-Frame Segmentation Structure:");
            LogToDebugConsole("");

            LogToDebugConsole("Dimension Organization:");
            LogToDebugConsole("  - Dimension Organization Sequence");
            LogToDebugConsole("  - Dimension Index Sequence");
            LogToDebugConsole("  - Common dimensions: Stack ID, In-Stack Position, Segment");
            LogToDebugConsole("");

            LogToDebugConsole("Per-Frame Functional Groups Sequence:");
            LogToDebugConsole("  Each frame contains:");
            LogToDebugConsole("  - Frame Content Sequence");
            LogToDebugConsole("      Dimension Index Values");
            LogToDebugConsole("  - Derivation Image Sequence");
            LogToDebugConsole("      Source Image Sequence (reference to CT/MR)");
            LogToDebugConsole("  - Segment Identification Sequence");
            LogToDebugConsole("      Referenced Segment Number");
            LogToDebugConsole("  - Plane Position Sequence");
            LogToDebugConsole("      Image Position (Patient)");
            LogToDebugConsole("  - Plane Orientation Sequence");
            LogToDebugConsole("      Image Orientation (Patient)");
            LogToDebugConsole("  - Pixel Measures Sequence");
            LogToDebugConsole("      Pixel Spacing, Slice Thickness");
            LogToDebugConsole("");

            LogToDebugConsole("Pixel Data Layout:");
            LogToDebugConsole("  BINARY:");
            LogToDebugConsole("    - 1 bit per pixel (packed)");
            LogToDebugConsole("    - Bits Allocated = 1");
            LogToDebugConsole("    - Photometric: MONOCHROME2");
            LogToDebugConsole("");
            LogToDebugConsole("  FRACTIONAL:");
            LogToDebugConsole("    - 8 bits per pixel");
            LogToDebugConsole("    - Bits Allocated = 8");
            LogToDebugConsole("    - Maximum Fractional Value = 255");
            LogToDebugConsole("");

            LogToDebugConsole("Frame Organization Options:");
            LogToDebugConsole("  1. One segment, multiple slices:");
            LogToDebugConsole("     Frame 1: Segment 1, Slice 1");
            LogToDebugConsole("     Frame 2: Segment 1, Slice 2");
            LogToDebugConsole("     ...");
            LogToDebugConsole("");
            LogToDebugConsole("  2. Multiple segments, one slice each:");
            LogToDebugConsole("     Frame 1: Segment 1, Slice 1");
            LogToDebugConsole("     Frame 2: Segment 2, Slice 1");
            LogToDebugConsole("     ...");
            LogToDebugConsole("");
            LogToDebugConsole("  3. Full volume, multiple segments:");
            LogToDebugConsole("     Frames for Segment 1 (all slices)");
            LogToDebugConsole("     Frames for Segment 2 (all slices)");
            LogToDebugConsole("     ...");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
