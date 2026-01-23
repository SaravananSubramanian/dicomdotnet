//-----------------------------------------------------------------------
// Tutorial: DICOM Transfer Syntax and Compression
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates DICOM Transfer Syntax concepts including byte ordering,
//   VR encoding, and various compression schemes (JPEG, JPEG 2000, RLE).
//
// Key Concepts:
//   - Transfer Syntax defines encoding rules
//   - Byte ordering (Little Endian vs Big Endian)
//   - VR encoding (Implicit vs Explicit)
//   - Lossy vs Lossless compression
//   - Codec requirements for transcoding
//
// Common Transfer Syntaxes:
//   - Implicit VR Little Endian (default)
//   - Explicit VR Little Endian
//   - JPEG Baseline (lossy)
//   - JPEG Lossless
//   - JPEG 2000 (lossy/lossless)
//   - JPEG-LS
//   - RLE Lossless
//
// fo-dicom References:
//   - DicomTransferSyntax - Transfer syntax definitions
//   - DicomTranscoder - For changing transfer syntax
//   - DicomFile.ChangeTransferSyntax()
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;

namespace Com.SaravananSubramanian.DicomTransferSyntax
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Transfer Syntax and Compression Demo ===");
                LogToDebugConsole("");

                // Demo 1: Transfer syntax overview
                LogToDebugConsole("--- Transfer Syntax Overview ---");
                DemonstrateTransferSyntaxOverview();

                // Demo 2: Common transfer syntaxes
                LogToDebugConsole("");
                LogToDebugConsole("--- Common Transfer Syntaxes ---");
                DemonstrateCommonTransferSyntaxes();

                // Demo 3: Compression types
                LogToDebugConsole("");
                LogToDebugConsole("--- Compression Types ---");
                DemonstrateCompressionTypes();

                // Demo 4: Working with transfer syntax in fo-dicom
                LogToDebugConsole("");
                LogToDebugConsole("--- fo-dicom Transfer Syntax APIs ---");
                DemonstrateFoDicomTransferSyntax();

                // Demo 5: Transcoding
                LogToDebugConsole("");
                LogToDebugConsole("--- Transcoding Between Transfer Syntaxes ---");
                DemonstrateTranscoding();

                // Demo 6: Practical considerations
                LogToDebugConsole("");
                LogToDebugConsole("--- Practical Considerations ---");
                DemonstratePracticalConsiderations();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate transfer syntax overview
        /// </summary>
        private static void DemonstrateTransferSyntaxOverview()
        {
            LogToDebugConsole("");
            LogToDebugConsole("What is Transfer Syntax?");
            LogToDebugConsole("  - Defines how DICOM data is encoded for transmission/storage");
            LogToDebugConsole("  - Specifies byte ordering (endianness)");
            LogToDebugConsole("  - Specifies VR encoding (implicit/explicit)");
            LogToDebugConsole("  - Specifies pixel data compression");
            LogToDebugConsole("");

            LogToDebugConsole("Transfer Syntax Components:");
            LogToDebugConsole("");
            LogToDebugConsole("1. Byte Ordering (Endianness):");
            LogToDebugConsole("   Little Endian - LSB first (most common)");
            LogToDebugConsole("   Big Endian - MSB first (rarely used now)");
            LogToDebugConsole("");

            LogToDebugConsole("2. Value Representation (VR) Encoding:");
            LogToDebugConsole("   Implicit VR - VR not included, determined by tag");
            LogToDebugConsole("   Explicit VR - VR included in each element");
            LogToDebugConsole("");

            LogToDebugConsole("3. Encapsulation:");
            LogToDebugConsole("   Native - Uncompressed pixel data");
            LogToDebugConsole("   Encapsulated - Compressed pixel data in fragments");
            LogToDebugConsole("");

            LogToDebugConsole("Transfer Syntax UID:");
            LogToDebugConsole("  (0002,0010) Transfer Syntax UID");
            LogToDebugConsole("  Located in File Meta Information");
            LogToDebugConsole("  Required for all DICOM files/associations");
        }

        /// <summary>
        /// Demonstrate common transfer syntaxes
        /// </summary>
        private static void DemonstrateCommonTransferSyntaxes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Uncompressed Transfer Syntaxes:");
            LogToDebugConsole("");

            LogToDebugConsole("  Implicit VR Little Endian (Default):");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.ImplicitVRLittleEndian.UID.UID}");
            LogToDebugConsole("    The default DICOM transfer syntax");
            LogToDebugConsole("    VR must be looked up in data dictionary");
            LogToDebugConsole("");

            LogToDebugConsole("  Explicit VR Little Endian:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.ExplicitVRLittleEndian.UID.UID}");
            LogToDebugConsole("    Widely supported, VR included");
            LogToDebugConsole("    Recommended for network transfer");
            LogToDebugConsole("");

            LogToDebugConsole("  Explicit VR Big Endian (Retired):");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.ExplicitVRBigEndian.UID.UID}");
            LogToDebugConsole("    Retired in DICOM 2016b");
            LogToDebugConsole("    Avoid for new implementations");
            LogToDebugConsole("");

            LogToDebugConsole("Compressed Transfer Syntaxes:");
            LogToDebugConsole("");

            LogToDebugConsole("  JPEG Baseline (Lossy):");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.JPEGProcess1.UID.UID}");
            LogToDebugConsole("    8-bit lossy compression");
            LogToDebugConsole("    Good compression ratio");
            LogToDebugConsole("");

            LogToDebugConsole("  JPEG Lossless:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.JPEGProcess14SV1.UID.UID}");
            LogToDebugConsole("    Selection Value 1 (Predictor 1)");
            LogToDebugConsole("    Most commonly used lossless");
            LogToDebugConsole("");

            LogToDebugConsole("  JPEG 2000 Lossless:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.JPEG2000Lossless.UID.UID}");
            LogToDebugConsole("    Better compression than JPEG Lossless");
            LogToDebugConsole("    Computationally more expensive");
            LogToDebugConsole("");

            LogToDebugConsole("  JPEG 2000 Lossy:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.JPEG2000Lossy.UID.UID}");
            LogToDebugConsole("    Configurable quality levels");
            LogToDebugConsole("    Part of JPEG 2000 family");
            LogToDebugConsole("");

            LogToDebugConsole("  JPEG-LS Lossless:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.JPEGLSLossless.UID.UID}");
            LogToDebugConsole("    Excellent lossless compression");
            LogToDebugConsole("");

            LogToDebugConsole("  RLE Lossless:");
            LogToDebugConsole($"    UID: {FellowOakDicom.DicomTransferSyntax.RLELossless.UID.UID}");
            LogToDebugConsole("    Run-Length Encoding");
            LogToDebugConsole("    Simple, widely supported");
        }

        /// <summary>
        /// Demonstrate compression types
        /// </summary>
        private static void DemonstrateCompressionTypes()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Lossy Compression:");
            LogToDebugConsole("  - Data is lost during compression");
            LogToDebugConsole("  - Cannot recover original values");
            LogToDebugConsole("  - Much better compression ratios (10:1 to 50:1)");
            LogToDebugConsole("  - Use cases: viewing, archival (with caution)");
            LogToDebugConsole("");
            LogToDebugConsole("  Lossy Compression Attributes:");
            LogToDebugConsole("  (0028,2110) Lossy Image Compression = \"01\"");
            LogToDebugConsole("  (0028,2112) Lossy Image Compression Ratio");
            LogToDebugConsole("  (0028,2114) Lossy Image Compression Method");
            LogToDebugConsole("");

            LogToDebugConsole("Lossless Compression:");
            LogToDebugConsole("  - Original data fully recoverable");
            LogToDebugConsole("  - Lower compression ratios (2:1 to 4:1)");
            LogToDebugConsole("  - Required for diagnostic/legal purposes");
            LogToDebugConsole("  - Safe for all clinical use");
            LogToDebugConsole("");

            LogToDebugConsole("Compression Comparison:");
            LogToDebugConsole("  | Type          | Ratio | Quality  | Use Case           |");
            LogToDebugConsole("  |---------------|-------|----------|---------------------|");
            LogToDebugConsole("  | None          | 1:1   | Perfect  | Acquisition         |");
            LogToDebugConsole("  | RLE           | ~2:1  | Lossless | Simple images       |");
            LogToDebugConsole("  | JPEG Lossless | ~3:1  | Lossless | General archival    |");
            LogToDebugConsole("  | JPEG-LS       | ~3:1  | Lossless | Better compression  |");
            LogToDebugConsole("  | JPEG 2K LL    | ~4:1  | Lossless | Best lossless       |");
            LogToDebugConsole("  | JPEG Baseline | ~20:1 | Lossy    | Web viewing         |");
            LogToDebugConsole("  | JPEG 2K Lossy | ~50:1 | Lossy    | High compression    |");
        }

        /// <summary>
        /// Demonstrate fo-dicom transfer syntax APIs
        /// </summary>
        private static void DemonstrateFoDicomTransferSyntax()
        {
            LogToDebugConsole("");
            LogToDebugConsole("DicomTransferSyntax Class:");
            LogToDebugConsole("");

            // Show properties of a transfer syntax
            var ts = FellowOakDicom.DicomTransferSyntax.JPEGProcess14SV1;
            LogToDebugConsole($"  Example: {ts.UID.Name}");
            LogToDebugConsole($"    UID: {ts.UID.UID}");
            LogToDebugConsole($"    IsExplicitVR: {ts.IsExplicitVR}");
            LogToDebugConsole($"    IsLittleEndian: true");
            LogToDebugConsole($"    IsEncapsulated: {ts.IsEncapsulated}");
            LogToDebugConsole($"    IsLossy: {ts.IsLossy}");
            LogToDebugConsole("");

            LogToDebugConsole("Looking up Transfer Syntax:");
            LogToDebugConsole("  var ts = FellowOakDicom.DicomTransferSyntax.Parse(\"1.2.840.10008.1.2\");");
            LogToDebugConsole("  var ts = FellowOakDicom.DicomTransferSyntax.ImplicitVRLittleEndian;");
            LogToDebugConsole("");

            LogToDebugConsole("Reading File's Transfer Syntax:");
            LogToDebugConsole("  var file = DicomFile.Open(\"image.dcm\");");
            LogToDebugConsole("  var ts = file.FileMetaInfo.TransferSyntax;");
            LogToDebugConsole("");

            LogToDebugConsole("Creating File with Specific Transfer Syntax:");
            LogToDebugConsole("  var file = new DicomFile(dataset);");
            LogToDebugConsole("  file.FileMetaInfo.TransferSyntax = ");
            LogToDebugConsole("      FellowOakDicom.DicomTransferSyntax.ExplicitVRLittleEndian;");
        }

        /// <summary>
        /// Demonstrate transcoding between transfer syntaxes
        /// </summary>
        private static void DemonstrateTranscoding()
        {
            LogToDebugConsole("");
            LogToDebugConsole("What is Transcoding?");
            LogToDebugConsole("  Converting DICOM data from one transfer syntax to another");
            LogToDebugConsole("  May involve compression, decompression, or re-encoding");
            LogToDebugConsole("");

            LogToDebugConsole("fo-dicom Transcoding:");
            LogToDebugConsole("");
            LogToDebugConsole("  // Using DicomFile extension method");
            LogToDebugConsole("  var compressed = file.Clone(FellowOakDicom.DicomTransferSyntax.JPEGProcess14SV1);");
            LogToDebugConsole("");
            LogToDebugConsole("  // Using transcoder directly");
            LogToDebugConsole("  var transcoder = new DicomTranscoder(");
            LogToDebugConsole("      inputSyntax, outputSyntax);");
            LogToDebugConsole("  var newDataset = transcoder.Transcode(dataset);");
            LogToDebugConsole("");

            LogToDebugConsole("Codec Parameters:");
            LogToDebugConsole("");
            LogToDebugConsole("  // JPEG quality (0-100)");
            LogToDebugConsole("  var jpegParams = new DicomJpegParams");
            LogToDebugConsole("  {");
            LogToDebugConsole("      Quality = 90");
            LogToDebugConsole("  };");
            LogToDebugConsole("");
            LogToDebugConsole("  // JPEG 2000 compression ratio");
            LogToDebugConsole("  var j2kParams = new DicomJpeg2000Params");
            LogToDebugConsole("  {");
            LogToDebugConsole("      Rate = 20  // 20:1 compression");
            LogToDebugConsole("  };");
            LogToDebugConsole("");

            LogToDebugConsole("Important Considerations:");
            LogToDebugConsole("  - Lossy transcoding is irreversible");
            LogToDebugConsole("  - Re-compressing lossy data degrades quality");
            LogToDebugConsole("  - Update (0028,2110) Lossy Image Compression");
            LogToDebugConsole("  - Preserve original when possible");
        }

        /// <summary>
        /// Demonstrate practical considerations
        /// </summary>
        private static void DemonstratePracticalConsiderations()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Storage Considerations:");
            LogToDebugConsole("  - Use lossless for diagnostic images");
            LogToDebugConsole("  - JPEG 2000 Lossless for best ratios");
            LogToDebugConsole("  - Consider JPEG-LS for speed");
            LogToDebugConsole("  - RLE for simple palette images");
            LogToDebugConsole("");

            LogToDebugConsole("Network Transfer:");
            LogToDebugConsole("  - Negotiate supported transfer syntaxes");
            LogToDebugConsole("  - Offer multiple options in presentation context");
            LogToDebugConsole("  - Accept syntax that requires least transcoding");
            LogToDebugConsole("");

            LogToDebugConsole("Viewing/Display:");
            LogToDebugConsole("  - Web viewers may need JPEG for speed");
            LogToDebugConsole("  - Always decompress for measurements");
            LogToDebugConsole("  - Be aware of compression artifacts");
            LogToDebugConsole("");

            LogToDebugConsole("Regulatory/Legal:");
            LogToDebugConsole("  - FDA recommends lossless for mammography");
            LogToDebugConsole("  - Some jurisdictions prohibit lossy for legal images");
            LogToDebugConsole("  - Document compression in DICOM attributes");
            LogToDebugConsole("  - Keep original as \"gold copy\" when compressing");
            LogToDebugConsole("");

            LogToDebugConsole("fo-dicom Codec Registration:");
            LogToDebugConsole("  // fo-dicom.Desktop includes native codecs");
            LogToDebugConsole("  // For fo-dicom.Core, install codec packages:");
            LogToDebugConsole("  //   fo-dicom.Imaging.ImageSharp");
            LogToDebugConsole("  //   fo-dicom.Imaging.Desktop");
            LogToDebugConsole("");

            LogToDebugConsole("Checking Codec Availability:");
            LogToDebugConsole("  var canDecode = DicomTranscoder.HasCodec(ts);");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
