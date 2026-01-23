//-----------------------------------------------------------------------
// Tutorial: DICOM Radiation Therapy (RT) Objects
//-----------------------------------------------------------------------
// Purpose:
//   Provides an overview of DICOM RT objects used in radiation oncology
//   for treatment planning and delivery.
//
// Key Concepts:
//   - RT Structure Set: Anatomical contours (ROIs)
//   - RT Plan: Treatment beam parameters
//   - RT Dose: 3D dose distribution
//   - RT Image: Portal images, DRRs
//   - RT Beams Treatment Record: Delivered treatment
//
// Note: RT objects are complex and typically created by
// Treatment Planning Systems (TPS). This demo provides
// an educational overview of the structure.
//
// Requirements:
//   - No external server connection required
//
// fo-dicom References:
//   - DicomUID - RT SOP Class UIDs
//   - DicomTag - RT-specific tags in groups 3002, 3004, 3006, 300A, 300C
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomRTObjects
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Radiation Therapy Objects Overview ===");
                LogToDebugConsole("");

                // Display RT SOP Classes
                LogToDebugConsole("--- RT SOP Classes ---");
                DemonstrateRTSopClasses();

                // Demonstrate RT Structure Set structure
                LogToDebugConsole("");
                LogToDebugConsole("--- RT Structure Set ---");
                DemonstrateRTStructureSet();

                // Demonstrate RT Plan structure
                LogToDebugConsole("");
                LogToDebugConsole("--- RT Plan ---");
                DemonstrateRTPlan();

                // Demonstrate RT Dose structure
                LogToDebugConsole("");
                LogToDebugConsole("--- RT Dose ---");
                DemonstrateRTDose();

                // RT workflow
                LogToDebugConsole("");
                LogToDebugConsole("--- RT Workflow ---");
                DemonstrateRTWorkflow();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Display RT SOP Classes
        /// </summary>
        private static void DemonstrateRTSopClasses()
        {
            LogToDebugConsole("");
            LogToDebugConsole($"RT Structure Set: {DicomUID.RTStructureSetStorage.UID}");
            LogToDebugConsole("  Contains: Anatomical contours (organs, tumors)");
            LogToDebugConsole("  Key Tags:");
            LogToDebugConsole("    (3006,0020) Structure Set ROI Sequence");
            LogToDebugConsole("    (3006,0039) ROI Contour Sequence");
            LogToDebugConsole("    (3006,0080) RT ROI Observations Sequence");
            LogToDebugConsole("");

            LogToDebugConsole($"RT Plan: {DicomUID.RTPlanStorage.UID}");
            LogToDebugConsole("  Contains: Treatment beam parameters, prescriptions");
            LogToDebugConsole("  Key Tags:");
            LogToDebugConsole("    (300A,00B0) Beam Sequence");
            LogToDebugConsole("    (300A,0070) Fraction Group Sequence");
            LogToDebugConsole("    (300C,0060) Referenced Structure Set Sequence");
            LogToDebugConsole("");

            LogToDebugConsole($"RT Dose: {DicomUID.RTDoseStorage.UID}");
            LogToDebugConsole("  Contains: 3D dose distribution, DVH");
            LogToDebugConsole("  Key Tags:");
            LogToDebugConsole("    (3004,0002) Dose Units");
            LogToDebugConsole("    (3004,0004) Dose Type");
            LogToDebugConsole("    (3004,000A) Dose Summation Type");
            LogToDebugConsole("    (3004,0050) DVH Sequence");
            LogToDebugConsole("");

            LogToDebugConsole($"RT Image: {DicomUID.RTImageStorage.UID}");
            LogToDebugConsole("  Contains: Portal images, DRRs");
            LogToDebugConsole("  Key Tags:");
            LogToDebugConsole("    (3002,0002) RT Image Label");
            LogToDebugConsole("    (3002,000C) RT Image Plane");
            LogToDebugConsole("    (3002,0012) RT Image SID");
        }

        /// <summary>
        /// Demonstrate RT Structure Set structure
        /// </summary>
        private static void DemonstrateRTStructureSet()
        {
            LogToDebugConsole("RT Structure Set contains ROIs defined on images:");
            LogToDebugConsole("");
            LogToDebugConsole("Structure Set ROI Sequence (3006,0020):");
            LogToDebugConsole("  Item 1:");
            LogToDebugConsole("    ROI Number: 1");
            LogToDebugConsole("    ROI Name: PTV (Planning Target Volume)");
            LogToDebugConsole("    ROI Generation Algorithm: MANUAL");
            LogToDebugConsole("  Item 2:");
            LogToDebugConsole("    ROI Number: 2");
            LogToDebugConsole("    ROI Name: Spinal Cord");
            LogToDebugConsole("    ROI Generation Algorithm: MANUAL");
            LogToDebugConsole("  Item 3:");
            LogToDebugConsole("    ROI Number: 3");
            LogToDebugConsole("    ROI Name: Left Lung");
            LogToDebugConsole("    ROI Generation Algorithm: AUTOMATIC");
            LogToDebugConsole("");
            LogToDebugConsole("ROI Contour Sequence (3006,0039):");
            LogToDebugConsole("  Item 1 (ROI 1 - PTV):");
            LogToDebugConsole("    Contour Sequence:");
            LogToDebugConsole("      - Slice 1: 45 points, type CLOSED_PLANAR");
            LogToDebugConsole("      - Slice 2: 52 points, type CLOSED_PLANAR");
            LogToDebugConsole("      - ... (contour for each CT slice)");
            LogToDebugConsole("    ROI Display Color: 255\\0\\0 (Red)");
        }

        /// <summary>
        /// Demonstrate RT Plan structure
        /// </summary>
        private static void DemonstrateRTPlan()
        {
            LogToDebugConsole("RT Plan contains treatment beam parameters:");
            LogToDebugConsole("");
            LogToDebugConsole("Plan Information:");
            LogToDebugConsole("  RT Plan Label: LUNG_SBRT_5FX");
            LogToDebugConsole("  RT Plan Date: 20240115");
            LogToDebugConsole("  Plan Intent: CURATIVE");
            LogToDebugConsole("");
            LogToDebugConsole("Fraction Group Sequence (300A,0070):");
            LogToDebugConsole("  Fraction Group Number: 1");
            LogToDebugConsole("  Number of Fractions Planned: 5");
            LogToDebugConsole("  Number of Beams: 7");
            LogToDebugConsole("");
            LogToDebugConsole("Beam Sequence (300A,00B0):");
            LogToDebugConsole("  Beam 1:");
            LogToDebugConsole("    Beam Number: 1");
            LogToDebugConsole("    Beam Name: AP");
            LogToDebugConsole("    Beam Type: STATIC");
            LogToDebugConsole("    Radiation Type: PHOTON");
            LogToDebugConsole("    Nominal Beam Energy: 6 MV");
            LogToDebugConsole("    Gantry Angle: 0.0");
            LogToDebugConsole("    Collimator Angle: 0.0");
            LogToDebugConsole("    Couch Angle: 0.0");
            LogToDebugConsole("  Beam 2:");
            LogToDebugConsole("    Beam Number: 2");
            LogToDebugConsole("    Beam Name: LAO_45");
            LogToDebugConsole("    Gantry Angle: 45.0");
        }

        /// <summary>
        /// Demonstrate RT Dose structure
        /// </summary>
        private static void DemonstrateRTDose()
        {
            LogToDebugConsole("RT Dose contains dose distribution:");
            LogToDebugConsole("");
            LogToDebugConsole("Dose Information:");
            LogToDebugConsole("  Dose Units: GY");
            LogToDebugConsole("  Dose Type: PHYSICAL");
            LogToDebugConsole("  Dose Summation Type: PLAN");
            LogToDebugConsole("  Dose Grid Scaling: 0.0001");
            LogToDebugConsole("");
            LogToDebugConsole("Dose Grid:");
            LogToDebugConsole("  Rows: 256");
            LogToDebugConsole("  Columns: 256");
            LogToDebugConsole("  Number of Frames: 80");
            LogToDebugConsole("  Pixel Spacing: 2.0\\2.0 mm");
            LogToDebugConsole("");
            LogToDebugConsole("DVH Sequence (3004,0050):");
            LogToDebugConsole("  DVH 1 (PTV):");
            LogToDebugConsole("    DVH Type: CUMULATIVE");
            LogToDebugConsole("    Dose Units: GY");
            LogToDebugConsole("    DVH Volume Units: CM3");
            LogToDebugConsole("    DVH Data: (dose-volume pairs)");
            LogToDebugConsole("  DVH 2 (Spinal Cord):");
            LogToDebugConsole("    DVH Max Dose: 8.5 Gy");
            LogToDebugConsole("    DVH Mean Dose: 2.3 Gy");
        }

        /// <summary>
        /// Demonstrate RT workflow
        /// </summary>
        private static void DemonstrateRTWorkflow()
        {
            LogToDebugConsole("Typical RT Workflow:");
            LogToDebugConsole("");
            LogToDebugConsole("1. Imaging (CT Simulation)");
            LogToDebugConsole("   - Patient positioned and CT scan acquired");
            LogToDebugConsole("   - CT images sent to TPS");
            LogToDebugConsole("   - Output: CT Image Series");
            LogToDebugConsole("");
            LogToDebugConsole("2. Contouring");
            LogToDebugConsole("   - Physician/dosimetrist draws ROIs");
            LogToDebugConsole("   - Targets: GTV, CTV, PTV");
            LogToDebugConsole("   - OARs: Spinal cord, lungs, heart");
            LogToDebugConsole("   - Output: RT Structure Set");
            LogToDebugConsole("");
            LogToDebugConsole("3. Treatment Planning");
            LogToDebugConsole("   - Dosimetrist designs beam arrangement");
            LogToDebugConsole("   - Optimizer calculates MLC positions");
            LogToDebugConsole("   - Dose calculated on CT grid");
            LogToDebugConsole("   - Output: RT Plan + RT Dose");
            LogToDebugConsole("");
            LogToDebugConsole("4. Plan Approval");
            LogToDebugConsole("   - Physician reviews plan and DVH");
            LogToDebugConsole("   - Plan approved for treatment");
            LogToDebugConsole("");
            LogToDebugConsole("5. Treatment Delivery");
            LogToDebugConsole("   - Plan sent to treatment machine");
            LogToDebugConsole("   - Treatment delivered");
            LogToDebugConsole("   - Output: RT Beams Treatment Record");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
