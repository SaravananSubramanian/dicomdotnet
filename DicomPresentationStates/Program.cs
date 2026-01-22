//-----------------------------------------------------------------------
// Tutorial: DICOM Presentation States (GSPS)
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to create Grayscale Softcopy Presentation States
//   that store display settings separate from image data.
//
// Key Concepts:
//   - Presentation States store display preferences
//   - Window width/level (brightness/contrast)
//   - Annotations (text, graphics)
//   - Display shutters
//   - Zoom, pan, rotation, flip
//   - GSPS SOP Class: 1.2.840.10008.5.1.4.1.1.11.1
//
// Presentation State Types:
//   - Grayscale Softcopy PS (1.2.840.10008.5.1.4.1.1.11.1)
//   - Color Softcopy PS (1.2.840.10008.5.1.4.1.1.11.2)
//   - Pseudo-Color Softcopy PS (1.2.840.10008.5.1.4.1.1.11.3)
//   - Blending Softcopy PS (1.2.840.10008.5.1.4.1.1.11.4)
//
// Requirements:
//   - No external server connection required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomSequence - For VOI LUT and annotation sequences
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using Dicom;

namespace Com.SaravananSubramanian.DicomPresentationStates
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
                LogToDebugConsole("=== DICOM Presentation States Demo ===");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Demo 1: Create a Grayscale Presentation State
                LogToDebugConsole("--- Demo 1: Creating GSPS ---");
                CreateGrayscalePresentationState();

                // Demo 2: Presentation State concepts
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 2: Presentation State Concepts ---");
                DemonstratePSConcepts();

                // Demo 3: Common presets
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 3: Common Window Presets ---");
                DemonstrateWindowPresets();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Create a Grayscale Softcopy Presentation State
        /// </summary>
        private static void CreateGrayscalePresentationState()
        {
            var dataset = new DicomDataset();

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string currentTime = DateTime.Now.ToString("HHmmss");

            // Referenced image (example UIDs)
            string referencedStudyUID = "1.2.3.4.5.6.7.8.9";
            string referencedSeriesUID = "1.2.3.4.5.6.7.8.9.1";
            string referencedSOPInstanceUID = "1.2.3.4.5.6.7.8.9.1.1";
            string referencedSOPClassUID = DicomUID.CTImageStorage.UID;

            //-----------------------------------------------------------------------
            // SOP Common Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.SOPClassUID, DicomUID.GrayscaleSoftcopyPresentationStateStorage);
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
            // Presentation State Module
            //-----------------------------------------------------------------------
            dataset.Add(DicomTag.Modality, "PR"); // Presentation State
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "100");
            dataset.Add(DicomTag.InstanceNumber, "1");

            // Content Label - user-friendly name
            dataset.Add(DicomTag.ContentLabel, "CT_LUNG_WINDOW");
            dataset.Add(DicomTag.ContentDescription, "Lung window preset for CT chest");

            // Presentation Creation Date/Time
            dataset.Add(DicomTag.PresentationCreationDate, currentDate);
            dataset.Add(DicomTag.PresentationCreationTime, currentTime);

            // Content Creator's Name
            dataset.Add(DicomTag.ContentCreatorName, "Tech^John");

            //-----------------------------------------------------------------------
            // Referenced Series Sequence
            //-----------------------------------------------------------------------
            var refSeriesSeq = new DicomSequence(DicomTag.ReferencedSeriesSequence);
            var refSeriesItem = new DicomDataset();
            refSeriesItem.Add(DicomTag.SeriesInstanceUID, referencedSeriesUID);

            // Referenced Image Sequence
            var refImageSeq = new DicomSequence(DicomTag.ReferencedImageSequence);
            var refImageItem = new DicomDataset();
            refImageItem.Add(DicomTag.ReferencedSOPClassUID, referencedSOPClassUID);
            refImageItem.Add(DicomTag.ReferencedSOPInstanceUID, referencedSOPInstanceUID);
            refImageSeq.Items.Add(refImageItem);
            refSeriesItem.Add(refImageSeq);

            refSeriesSeq.Items.Add(refSeriesItem);
            dataset.Add(refSeriesSeq);

            //-----------------------------------------------------------------------
            // Softcopy VOI LUT Module (Window Width/Level)
            //-----------------------------------------------------------------------
            var voiLutSeq = new DicomSequence(DicomTag.SoftcopyVOILUTSequence);
            var voiLutItem = new DicomDataset();

            // Reference the image this applies to
            var refImageSeq2 = new DicomSequence(DicomTag.ReferencedImageSequence);
            var refImageItem2 = new DicomDataset();
            refImageItem2.Add(DicomTag.ReferencedSOPClassUID, referencedSOPClassUID);
            refImageItem2.Add(DicomTag.ReferencedSOPInstanceUID, referencedSOPInstanceUID);
            refImageSeq2.Items.Add(refImageItem2);
            voiLutItem.Add(refImageSeq2);

            // Window Center and Width for lung window
            voiLutItem.Add(DicomTag.WindowCenter, "-600");  // Lung window center
            voiLutItem.Add(DicomTag.WindowWidth, "1500");   // Lung window width
            voiLutItem.Add(DicomTag.VOILUTFunction, "LINEAR");

            voiLutSeq.Items.Add(voiLutItem);
            dataset.Add(voiLutSeq);

            //-----------------------------------------------------------------------
            // Graphic Layer Module
            //-----------------------------------------------------------------------
            var graphicLayerSeq = new DicomSequence(DicomTag.GraphicLayerSequence);
            var layerItem = new DicomDataset();
            layerItem.Add(DicomTag.GraphicLayer, "LAYER1");
            layerItem.Add(DicomTag.GraphicLayerOrder, "1");
            layerItem.Add(DicomTag.GraphicLayerDescription, "Annotation layer");
            graphicLayerSeq.Items.Add(layerItem);
            dataset.Add(graphicLayerSeq);

            //-----------------------------------------------------------------------
            // Save the file
            //-----------------------------------------------------------------------
            string outputFile = Path.Combine(OutputPath, "presentation_state.dcm");
            var dicomFile = new DicomFile(dataset);
            dicomFile.Save(outputFile);

            LogToDebugConsole($"Presentation State created: {outputFile}");
            LogToDebugConsole($"References image: {referencedSOPInstanceUID}");
            LogToDebugConsole("Window Width: 1500, Window Center: -600 (Lung window)");
        }

        /// <summary>
        /// Demonstrate Presentation State concepts
        /// </summary>
        private static void DemonstratePSConcepts()
        {
            LogToDebugConsole("Presentation State Types:");
            LogToDebugConsole("");
            LogToDebugConsole("  Grayscale Softcopy PS (GSPS):");
            LogToDebugConsole("    - Most common type");
            LogToDebugConsole("    - Window/level, annotations, shutters");
            LogToDebugConsole("    - SOP: 1.2.840.10008.5.1.4.1.1.11.1");
            LogToDebugConsole("");
            LogToDebugConsole("  Color Softcopy PS:");
            LogToDebugConsole("    - For color images");
            LogToDebugConsole("    - SOP: 1.2.840.10008.5.1.4.1.1.11.2");
            LogToDebugConsole("");
            LogToDebugConsole("  Pseudo-Color Softcopy PS:");
            LogToDebugConsole("    - Apply color LUT to grayscale");
            LogToDebugConsole("    - SOP: 1.2.840.10008.5.1.4.1.1.11.3");
            LogToDebugConsole("");
            LogToDebugConsole("  Blending Softcopy PS:");
            LogToDebugConsole("    - Blend multiple images (e.g., PET/CT fusion)");
            LogToDebugConsole("    - SOP: 1.2.840.10008.5.1.4.1.1.11.4");
            LogToDebugConsole("");

            LogToDebugConsole("Key Modules:");
            LogToDebugConsole("  - Softcopy VOI LUT: Window Width/Level");
            LogToDebugConsole("  - Graphic Annotation: Text and graphics");
            LogToDebugConsole("  - Graphic Layer: Annotation layers");
            LogToDebugConsole("  - Display Shutter: Hide regions");
            LogToDebugConsole("  - Displayed Area: Zoom, pan");
            LogToDebugConsole("  - Spatial Transformation: Rotate, flip");
        }

        /// <summary>
        /// Demonstrate common window presets
        /// </summary>
        private static void DemonstrateWindowPresets()
        {
            LogToDebugConsole("Common CT Window Presets:");
            LogToDebugConsole("");
            LogToDebugConsole("Preset          | Window Width | Window Center");
            LogToDebugConsole("----------------|--------------|---------------");
            LogToDebugConsole("Lung            | 1500         | -600");
            LogToDebugConsole("Mediastinum     | 350          | 50");
            LogToDebugConsole("Soft Tissue     | 400          | 40");
            LogToDebugConsole("Bone            | 2000         | 300");
            LogToDebugConsole("Brain           | 80           | 40");
            LogToDebugConsole("Subdural        | 200          | 75");
            LogToDebugConsole("Stroke          | 40           | 40");
            LogToDebugConsole("Liver           | 150          | 30");
            LogToDebugConsole("Abdomen         | 400          | 50");
            LogToDebugConsole("");

            LogToDebugConsole("Formula: Display Value = (Pixel Value - (WC - WW/2)) / WW * 255");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
