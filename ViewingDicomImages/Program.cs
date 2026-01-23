//-----------------------------------------------------------------------
// Tutorial: Viewing DICOM Images with Window/Level Control
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to display DICOM images and apply Window Width
//   and Window Center (Level) adjustments for optimal visualization.
//   This tutorial includes an interactive Windows Forms viewer that
//   allows real-time adjustment of Window/Level settings.
//
// Key Concepts:
//   - Window Width (WW): Controls contrast - range of gray values displayed
//   - Window Center/Level (WL): Controls brightness - center of the range
//   - Formula: if (pixel <= WL - WW/2) => black; if (pixel >= WL + WW/2) => white
//   - Common CT Presets: Lung (WW=1500, WL=-600), Bone (WW=2500, WL=480)
//
// Interactive Viewer Features:
//   - Real-time image rendering as Window/Level values change
//   - Slider controls for fine adjustment of Window Width and Center
//   - Preset buttons for common CT viewing configurations
//   - Keyboard shortcuts: R=Reset, S=Save, 1-5=Presets, Esc=Close
//   - Display of DICOM metadata (patient, study, modality, etc.)
//
// Requirements:
//   - DICOM test file: Place a .dcm file in the "Test Files" folder
//   - Sample files available from: https://www.dicomlibrary.com/
//   - For CT images: files should contain Window Center (0028,1050)
//     and Window Width (0028,1051) attributes
//
// Common Window/Level Presets:
//   | Preset      | Width | Center | Use Case          |
//   |-------------|-------|--------|-------------------|
//   | Lung        | 1500  | -600   | CT lung tissue    |
//   | Bone        | 2500  | 480    | CT bone           |
//   | Soft Tissue | 400   | 40     | CT soft tissue    |
//   | Brain       | 80    | 40     | CT brain          |
//   | Abdomen     | 350   | 50     | CT abdomen        |
//   | Mediastinum | 500   | 50     | CT mediastinum    |
//   | Liver       | 150   | 30     | CT liver          |
//
// fo-dicom References:
//   - DicomImage - Image rendering with W/L support
//   - WindowCenter/WindowWidth properties
//   - GrayscaleRenderOptions - Advanced rendering control
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FellowOakDicom;
using FellowOakDicom.Imaging;

namespace Com.SaravananSubramanian.ViewingDicomImages
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Path to DICOM test file
        // NOTE: Ensure a valid DICOM image file exists at this path before running
        // For best results, use a CT image with embedded window/level values
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomTestFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "CT_small.dcm");
        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Image Viewing Tutorial ===");
                LogToDebugConsole($"Reading DICOM file: {PathToDicomTestFile}");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                // Verify the DICOM file exists
                if (!File.Exists(PathToDicomTestFile))
                {
                    LogToDebugConsole($"ERROR: DICOM file not found at: {PathToDicomTestFile}");
                    LogToDebugConsole("Please place a DICOM file in the 'Test Files' folder.");
                    return;
                }

                // Open the DICOM file
                var file = DicomFile.Open(PathToDicomTestFile);
                var dataset = file.Dataset;

                //-----------------------------------------------------------------------
                // Display existing Window/Level values from the DICOM file
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Window/Level Information ---");

                var windowCenter = dataset.GetSingleValueOrDefault(DicomTag.WindowCenter, double.NaN);
                var windowWidth = dataset.GetSingleValueOrDefault(DicomTag.WindowWidth, double.NaN);
                var windowExplanation = dataset.GetSingleValueOrDefault(DicomTag.WindowCenterWidthExplanation, "");

                if (!double.IsNaN(windowCenter) && !double.IsNaN(windowWidth))
                {
                    LogToDebugConsole($"  Original Window Center: {windowCenter}");
                    LogToDebugConsole($"  Original Window Width:  {windowWidth}");
                    if (!string.IsNullOrEmpty(windowExplanation))
                    {
                        LogToDebugConsole($"  Explanation: {windowExplanation}");
                    }
                }
                else
                {
                    LogToDebugConsole("  No window/level values embedded in file");
                }
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Render with default settings and save to files
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Rendering Images to Files ---");

                var dicomImage = new DicomImage(PathToDicomTestFile);

                // Render with default (original) window/level
                LogToDebugConsole("  Rendering with default settings...");
                SaveRenderedImage(dicomImage, "default");

                //-----------------------------------------------------------------------
                // Demonstrate different Window/Level presets
                // These presets are commonly used for CT images
                //-----------------------------------------------------------------------

                // Lung preset (typical for viewing lung parenchyma in CT)
                LogToDebugConsole("  Rendering with Lung preset (WW=1500, WL=-600)...");
                dicomImage.WindowWidth = 1500;
                dicomImage.WindowCenter = -600;
                SaveRenderedImage(dicomImage, "lung");

                // Bone preset (typical for viewing bone in CT)
                LogToDebugConsole("  Rendering with Bone preset (WW=2500, WL=480)...");
                dicomImage.WindowWidth = 2500;
                dicomImage.WindowCenter = 480;
                SaveRenderedImage(dicomImage, "bone");

                // Soft tissue preset (typical for viewing soft tissue in CT)
                LogToDebugConsole("  Rendering with Soft Tissue preset (WW=400, WL=40)...");
                dicomImage.WindowWidth = 400;
                dicomImage.WindowCenter = 40;
                SaveRenderedImage(dicomImage, "soft_tissue");

                // Brain preset (typical for brain CT)
                LogToDebugConsole("  Rendering with Brain preset (WW=80, WL=40)...");
                dicomImage.WindowWidth = 80;
                dicomImage.WindowCenter = 40;
                SaveRenderedImage(dicomImage, "brain");

                LogToDebugConsole("");
                LogToDebugConsole($"All images saved to: {OutputPath}");

                //-----------------------------------------------------------------------
                // Launch the Interactive DICOM Image Viewer
                //-----------------------------------------------------------------------
                LogToDebugConsole("");
                LogToDebugConsole("--- Launching Interactive DICOM Viewer ---");
                LogToDebugConsole("  Keyboard shortcuts:");
                LogToDebugConsole("    R     - Reset to original window/level values");
                LogToDebugConsole("    S     - Save current view to file");
                LogToDebugConsole("    1-5   - Apply presets (1=Lung, 2=Bone, 3=Soft Tissue, 4=Brain, 5=Abdomen)");
                LogToDebugConsole("    Esc   - Close viewer");
                LogToDebugConsole("");

                // Initialize Windows Forms
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Launch the interactive viewer
                using (var viewer = new DicomImageViewer(PathToDicomTestFile))
                {
                    Application.Run(viewer);
                }

                LogToDebugConsole("Image viewing tutorial completed successfully!");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error viewing DICOM image: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
                MessageBox.Show(
                    $"Error: {e.Message}\n\nPlease ensure a valid DICOM file exists at:\n{PathToDicomTestFile}",
                    "DICOM Viewer Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Renders the DICOM image and saves it with the specified preset name.
        /// </summary>
        private static void SaveRenderedImage(DicomImage dicomImage, string presetName)
        {
            var outputPath = Path.Combine(OutputPath, $"view_{presetName}.png");
            var renderedImage = dicomImage.RenderImage(0);
            var bitmap = renderedImage.As<Bitmap>();
            bitmap.Save(outputPath, ImageFormat.Png);
            bitmap.Dispose();
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
