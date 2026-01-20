//-----------------------------------------------------------------------
// Tutorial: Extracting and Exporting DICOM Image Data
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how to extract pixel data from a DICOM file and
//   export it to standard image formats (JPEG, PNG, BMP).
//
// Key Concepts:
//   - DICOM images store pixel data in various formats and bit depths
//   - Window Width/Level controls contrast/brightness for display
//   - Photometric Interpretation defines how to interpret pixel values
//   - Common formats: MONOCHROME1, MONOCHROME2, RGB
//
// Requirements:
//   - DICOM test file: Place a .dcm file in the "Test Files" folder
//   - Sample files available from: https://www.dicomlibrary.com/
//   - fo-dicom.Imaging namespace for image rendering
//
// Key Image Attributes:
//   - (0028,0010) Rows - Image height in pixels
//   - (0028,0011) Columns - Image width in pixels
//   - (0028,0100) Bits Allocated - 8, 16, or 32
//   - (0028,0101) Bits Stored - Actual bits used
//   - (0028,0004) Photometric Interpretation - MONOCHROME1/2, RGB
//   - (0028,1050) Window Center - Brightness control
//   - (0028,1051) Window Width - Contrast control
//   - (7FE0,0010) Pixel Data - Raw pixel bytes
//
// fo-dicom References:
//   - DicomImage - Renders DICOM images
//   - RenderImage() - Creates displayable bitmap
//   - DicomPixelData - Access to raw pixel data
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Dicom;
using Dicom.Imaging;

namespace ExtractingImageData
{
    public class Program
    {
        //-----------------------------------------------------------------------
        // Configuration: Paths for input and output files
        // NOTE: Ensure a valid DICOM image file exists at this path before running
        //-----------------------------------------------------------------------
        private static readonly string PathToDicomTestFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Files", "0002.dcm");
        private static readonly string OutputPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");

        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== Extracting DICOM Image Data Tutorial ===");
                LogToDebugConsole($"Reading DICOM file: {PathToDicomTestFile}");
                LogToDebugConsole("");

                // Ensure output directory exists
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                    LogToDebugConsole($"Created output directory: {OutputPath}");
                }

                // Open the DICOM file
                var file = DicomFile.Open(PathToDicomTestFile);
                var dataset = file.Dataset;

                //-----------------------------------------------------------------------
                // Display Image Parameters
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Image Parameters ---");

                var rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, (ushort)0);
                var columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, (ushort)0);
                var bitsAllocated = dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, (ushort)0);
                var bitsStored = dataset.GetSingleValueOrDefault(DicomTag.BitsStored, (ushort)0);
                var highBit = dataset.GetSingleValueOrDefault(DicomTag.HighBit, (ushort)0);
                var pixelRepresentation = dataset.GetSingleValueOrDefault(DicomTag.PixelRepresentation, (ushort)0);
                var samplesPerPixel = dataset.GetSingleValueOrDefault(DicomTag.SamplesPerPixel, (ushort)0);
                var photometricInterpretation = dataset.GetSingleValueOrDefault(DicomTag.PhotometricInterpretation, "");
                var windowCenter = dataset.GetSingleValueOrDefault(DicomTag.WindowCenter, 0.0);
                var windowWidth = dataset.GetSingleValueOrDefault(DicomTag.WindowWidth, 0.0);

                LogToDebugConsole($"  Dimensions:          {columns} x {rows} pixels");
                LogToDebugConsole($"  Bits Allocated:      {bitsAllocated}");
                LogToDebugConsole($"  Bits Stored:         {bitsStored}");
                LogToDebugConsole($"  High Bit:            {highBit}");
                LogToDebugConsole($"  Pixel Representation: {(pixelRepresentation == 0 ? "Unsigned" : "Signed")}");
                LogToDebugConsole($"  Samples Per Pixel:   {samplesPerPixel} ({(samplesPerPixel == 1 ? "Grayscale" : "Color")})");
                LogToDebugConsole($"  Photometric:         {photometricInterpretation}");
                LogToDebugConsole($"  Window Center:       {windowCenter}");
                LogToDebugConsole($"  Window Width:        {windowWidth}");
                LogToDebugConsole("");

                //-----------------------------------------------------------------------
                // Render and Export Image
                //-----------------------------------------------------------------------
                LogToDebugConsole("--- Exporting Image ---");

                // Create a DicomImage for rendering
                var dicomImage = new DicomImage(PathToDicomTestFile);

                // Get the number of frames (for multi-frame images like cine)
                var frameCount = dicomImage.NumberOfFrames;
                LogToDebugConsole($"  Number of Frames:    {frameCount}");

                // Render the first frame (or all frames for multi-frame)
                for (int frame = 0; frame < frameCount; frame++)
                {
                    // Render the image to a bitmap
                    var renderedImage = dicomImage.RenderImage(frame);
                    var bitmap = renderedImage.As<Bitmap>();

                    // Generate output filenames
                    var frameLabel = frameCount > 1 ? $"_frame_{frame + 1:D3}" : "";
                    var jpegPath = Path.Combine(OutputPath, $"exported_image{frameLabel}.jpg");
                    var pngPath = Path.Combine(OutputPath, $"exported_image{frameLabel}.png");
                    var bmpPath = Path.Combine(OutputPath, $"exported_image{frameLabel}.bmp");

                    // Save in multiple formats
                    bitmap.Save(jpegPath, ImageFormat.Jpeg);
                    bitmap.Save(pngPath, ImageFormat.Png);
                    bitmap.Save(bmpPath, ImageFormat.Bmp);

                    if (frameCount > 1)
                    {
                        LogToDebugConsole($"  Exported frame {frame + 1}/{frameCount}");
                    }
                    else
                    {
                        LogToDebugConsole($"  JPEG: {jpegPath}");
                        LogToDebugConsole($"  PNG:  {pngPath}");
                        LogToDebugConsole($"  BMP:  {bmpPath}");
                    }

                    // Clean up bitmap
                    bitmap.Dispose();
                }

                LogToDebugConsole("");
                LogToDebugConsole("Image export completed successfully!");
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error extracting image data: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
