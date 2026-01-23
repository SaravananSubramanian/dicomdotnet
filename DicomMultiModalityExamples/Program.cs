//-----------------------------------------------------------------------
// Tutorial: DICOM Multi-modality Examples
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates modality-specific DICOM attributes for different
//   imaging modalities like CT, MR, US, XA, DX, MG, and NM/PT.
//
// Key Concepts:
//   - Each modality has specific IODs (Information Object Definitions)
//   - Modality-specific modules contain unique attributes
//   - Understanding these attributes is essential for DICOM handling
//   - Common modalities: CT, MR, US, XA, CR/DX, MG, NM, PT
//
// Requirements:
//   - No external server connection required
//   - No test files required
//
// fo-dicom References:
//   - DicomTag - Standard DICOM tag definitions for each modality
//   - DicomUID - SOP Class UIDs for different modalities
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomMultiModalityExamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Multi-modality Examples ===");
                LogToDebugConsole("");

                // CT-specific attributes
                LogToDebugConsole("=== CT (Computed Tomography) ===");
                DemonstrateCTAttributes();

                // MR-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== MR (Magnetic Resonance) ===");
                DemonstrateMRAttributes();

                // US-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== US (Ultrasound) ===");
                DemonstrateUSAttributes();

                // XA-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== XA (X-Ray Angiography) ===");
                DemonstrateXAAttributes();

                // CR/DX-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== CR/DX (Radiography) ===");
                DemonstrateDXAttributes();

                // MG-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== MG (Mammography) ===");
                DemonstrateMGAttributes();

                // NM/PT-specific attributes
                LogToDebugConsole("");
                LogToDebugConsole("=== NM/PT (Nuclear Medicine/PET) ===");
                DemonstrateNMAttributes();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// CT-specific attributes
        /// </summary>
        private static void DemonstrateCTAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Class: CT Image Storage (1.2.840.10008.5.1.4.1.1.2)");
            LogToDebugConsole("");

            LogToDebugConsole("Key CT Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("CT Image Module:");
            LogToDebugConsole("  (0018,0060) KVP - X-ray tube voltage (e.g., 120 kV)");
            LogToDebugConsole("  (0018,1151) X-Ray Tube Current - mA (e.g., 250)");
            LogToDebugConsole("  (0018,1150) Exposure Time - ms (e.g., 500)");
            LogToDebugConsole("  (0018,1152) Exposure - mAs (e.g., 125)");
            LogToDebugConsole("  (0018,9345) CTDIvol - dose index in mGy");
            LogToDebugConsole("  (0018,0050) Slice Thickness - mm (e.g., 1.25)");
            LogToDebugConsole("  (0018,0088) Spacing Between Slices - mm");
            LogToDebugConsole("");

            LogToDebugConsole("CT Reconstruction:");
            LogToDebugConsole("  (0018,1100) Reconstruction Diameter - mm (FOV)");
            LogToDebugConsole("  (0018,1210) Convolution Kernel - e.g., STANDARD, BONE");
            LogToDebugConsole("  (0018,5100) Patient Position - HFS, HFP, FFS, FFP");
            LogToDebugConsole("");

            LogToDebugConsole("Hounsfield Units:");
            LogToDebugConsole("  (0028,1052) Rescale Intercept - typically -1024");
            LogToDebugConsole("  (0028,1053) Rescale Slope - typically 1");
            LogToDebugConsole("  HU = Rescale Slope * Pixel Value + Rescale Intercept");
            LogToDebugConsole("  Water = 0 HU, Air = -1000 HU, Bone = +400 to +1000 HU");
        }

        /// <summary>
        /// MR-specific attributes
        /// </summary>
        private static void DemonstrateMRAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Class: MR Image Storage (1.2.840.10008.5.1.4.1.1.4)");
            LogToDebugConsole("");

            LogToDebugConsole("Key MR Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("MR Image Module:");
            LogToDebugConsole("  (0018,0020) Scanning Sequence - SE, IR, GR, EP, RM");
            LogToDebugConsole("  (0018,0021) Sequence Variant - SK, MTC, SS, TRSS, SP, MP");
            LogToDebugConsole("  (0018,0022) Scan Options - PER, RG, CG, PPG, FC, PFF");
            LogToDebugConsole("  (0018,0023) MR Acquisition Type - 2D, 3D");
            LogToDebugConsole("  (0018,0080) Repetition Time (TR) - ms");
            LogToDebugConsole("  (0018,0081) Echo Time (TE) - ms");
            LogToDebugConsole("  (0018,0082) Inversion Time (TI) - ms (for IR sequences)");
            LogToDebugConsole("  (0018,0083) Number of Averages (NEX/NSA)");
            LogToDebugConsole("  (0018,0087) Magnetic Field Strength - Tesla");
            LogToDebugConsole("  (0018,1314) Flip Angle - degrees");
            LogToDebugConsole("");

            LogToDebugConsole("Common Sequence Types:");
            LogToDebugConsole("  T1-weighted: Short TR (~500ms), Short TE (~10-20ms)");
            LogToDebugConsole("  T2-weighted: Long TR (~2000-4000ms), Long TE (~80-120ms)");
            LogToDebugConsole("  FLAIR: Long TR, Long TE, TI ~2500ms");
            LogToDebugConsole("  DWI: Echo-planar, b-value in (0018,9087)");
        }

        /// <summary>
        /// US-specific attributes
        /// </summary>
        private static void DemonstrateUSAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Class: US Image Storage (1.2.840.10008.5.1.4.1.1.6.1)");
            LogToDebugConsole("         US Multi-frame (1.2.840.10008.5.1.4.1.1.3.1)");
            LogToDebugConsole("");

            LogToDebugConsole("Key US Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("US Image Module:");
            LogToDebugConsole("  (0018,6011) Sequence of Ultrasound Regions");
            LogToDebugConsole("  (0018,602C) Physical Delta X - mm per pixel");
            LogToDebugConsole("  (0018,602E) Physical Delta Y - mm per pixel");
            LogToDebugConsole("  (0008,2142) Start Trim - frame number");
            LogToDebugConsole("  (0008,2143) Stop Trim - frame number");
            LogToDebugConsole("  (0008,2144) Recommended Display Frame Rate");
            LogToDebugConsole("  (0018,6030) Transducer Type - SECTOR, LINEAR, CURVED");
            LogToDebugConsole("");

            LogToDebugConsole("Multi-frame:");
            LogToDebugConsole("  (0028,0008) Number of Frames - typically high (cine loops)");
            LogToDebugConsole("  (0018,1063) Frame Time - ms between frames");
        }

        /// <summary>
        /// XA-specific attributes
        /// </summary>
        private static void DemonstrateXAAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Class: X-Ray Angiographic Image (1.2.840.10008.5.1.4.1.1.12.1)");
            LogToDebugConsole("");

            LogToDebugConsole("Key XA Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("XA Image Module:");
            LogToDebugConsole("  (0018,1147) Field of View Shape - RECTANGLE, ROUND");
            LogToDebugConsole("  (0018,1149) Field of View Dimension(s) - mm");
            LogToDebugConsole("  (0018,1500) Positioner Motion - STATIC, DYNAMIC");
            LogToDebugConsole("  (0018,1510) Positioner Primary Angle - LAO/RAO");
            LogToDebugConsole("  (0018,1511) Positioner Secondary Angle - CRAN/CAUD");
            LogToDebugConsole("  (0018,1114) Magnification Factor");
            LogToDebugConsole("  (0018,1164) Imager Pixel Spacing - mm/pixel");
            LogToDebugConsole("");

            LogToDebugConsole("Multi-frame (cine):");
            LogToDebugConsole("  (0028,0008) Number of Frames");
            LogToDebugConsole("  (0008,2144) Recommended Display Frame Rate - fps");
        }

        /// <summary>
        /// DX/CR-specific attributes
        /// </summary>
        private static void DemonstrateDXAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Classes:");
            LogToDebugConsole("  Digital X-Ray (DX): 1.2.840.10008.5.1.4.1.1.1.1");
            LogToDebugConsole("  Computed Radiography (CR): 1.2.840.10008.5.1.4.1.1.1");
            LogToDebugConsole("");

            LogToDebugConsole("Key DX/CR Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("DX Anatomy Imaged Module:");
            LogToDebugConsole("  (0018,5101) View Position - AP, PA, LL, RL");
            LogToDebugConsole("  (0008,2218) Anatomic Region Sequence");
            LogToDebugConsole("  (0020,0060) Laterality - R, L");
            LogToDebugConsole("");

            LogToDebugConsole("DX Positioning Module:");
            LogToDebugConsole("  (0018,1110) Distance Source to Detector - mm");
            LogToDebugConsole("  (0018,1111) Distance Source to Patient - mm");
            LogToDebugConsole("  (0018,1166) Grid - IN, NONE");
            LogToDebugConsole("");

            LogToDebugConsole("Exposure:");
            LogToDebugConsole("  (0018,0060) KVP");
            LogToDebugConsole("  (0018,1152) Exposure - mAs");
        }

        /// <summary>
        /// MG-specific attributes
        /// </summary>
        private static void DemonstrateMGAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Class: Digital Mammography (1.2.840.10008.5.1.4.1.1.1.2)");
            LogToDebugConsole("");

            LogToDebugConsole("Key MG Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("Mammography Image Module:");
            LogToDebugConsole("  (0018,0060) KVP - typically 26-32 kV");
            LogToDebugConsole("  (0018,1114) Magnification Factor");
            LogToDebugConsole("  (0018,1166) Grid - IN or NONE");
            LogToDebugConsole("  (0018,7004) Detector Type - DIRECT, SCINTILLATOR");
            LogToDebugConsole("  (0018,7050) Filter Material - Mo, Rh, Al");
            LogToDebugConsole("  (0018,11A0) Body Part Thickness - compressed thickness in mm");
            LogToDebugConsole("  (0018,11A2) Compression Force - N (Newtons)");
            LogToDebugConsole("");

            LogToDebugConsole("View Information:");
            LogToDebugConsole("  (0020,0060) Laterality - R, L");
            LogToDebugConsole("  (0018,5101) View Position - CC, MLO, ML, LM");
        }

        /// <summary>
        /// NM/PT-specific attributes
        /// </summary>
        private static void DemonstrateNMAttributes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("SOP Classes:");
            LogToDebugConsole("  NM Image: 1.2.840.10008.5.1.4.1.1.20");
            LogToDebugConsole("  PET Image: 1.2.840.10008.5.1.4.1.1.128");
            LogToDebugConsole("");

            LogToDebugConsole("Key NM/PT Attributes:");
            LogToDebugConsole("");

            LogToDebugConsole("NM Image Module:");
            LogToDebugConsole("  (0054,0016) Radiopharmaceutical Information Sequence");
            LogToDebugConsole("  (0018,0031) Radiopharmaceutical - e.g., Tc-99m, FDG");
            LogToDebugConsole("  (0018,1071) Radiopharmaceutical Volume - ml");
            LogToDebugConsole("  (0018,1074) Radionuclide Total Dose - MBq");
            LogToDebugConsole("  (0018,1072) Radiopharmaceutical Start DateTime");
            LogToDebugConsole("");

            LogToDebugConsole("PET-specific:");
            LogToDebugConsole("  (0054,1001) Units - BQML, CNTS");
            LogToDebugConsole("  (0054,1102) Decay Correction - START, ADMIN, NONE");
            LogToDebugConsole("  (0010,1030) Patient Weight - needed for SUV calculation");
            LogToDebugConsole("  SUV = Activity / (Injected Dose / Patient Weight)");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
