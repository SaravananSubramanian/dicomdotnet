//-----------------------------------------------------------------------
// Tutorial: DICOM Waveform Objects
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates DICOM Waveform IOD concepts for storing physiological
//   signal data such as ECG, EEG, hemodynamic, and other waveforms.
//
// Key Concepts:
//   - DICOM Waveform IODs (ECG, Hemodynamic, etc.)
//   - Waveform Sequence structure
//   - Channel definitions and sources
//   - Sampling frequency and data encoding
//   - Waveform annotations
//
// Common Waveform SOP Classes:
//   - 12-Lead ECG Waveform Storage
//   - General ECG Waveform Storage
//   - Hemodynamic Waveform Storage
//   - Basic Voice Audio Waveform Storage
//   - General Audio Waveform Storage
//
// fo-dicom References:
//   - DicomDataset - Core data structure
//   - DicomSequence - For waveform sequences
//   - DicomOtherWord - For waveform data (OW)
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomWaveforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Waveform Objects Demo ===");
                LogToDebugConsole("");

                // Demo 1: Waveform IOD overview
                LogToDebugConsole("--- Waveform IOD Overview ---");
                DemonstrateWaveformIOD();

                // Demo 2: Waveform SOP Classes
                LogToDebugConsole("");
                LogToDebugConsole("--- Waveform SOP Classes ---");
                DemonstrateWaveformSOPClasses();

                // Demo 3: Waveform Sequence structure
                LogToDebugConsole("");
                LogToDebugConsole("--- Waveform Sequence Structure ---");
                DemonstrateWaveformSequence();

                // Demo 4: Channel definitions
                LogToDebugConsole("");
                LogToDebugConsole("--- Channel Definitions ---");
                DemonstrateChannelDefinitions();

                // Demo 5: Creating waveform with fo-dicom
                LogToDebugConsole("");
                LogToDebugConsole("--- Creating Waveform with fo-dicom ---");
                DemonstrateWaveformCreation();

                // Demo 6: Waveform annotations
                LogToDebugConsole("");
                LogToDebugConsole("--- Waveform Annotations ---");
                DemonstrateWaveformAnnotations();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate Waveform IOD overview
        /// </summary>
        private static void DemonstrateWaveformIOD()
        {
            LogToDebugConsole("");
            LogToDebugConsole("What are DICOM Waveforms?");
            LogToDebugConsole("  - Storage of physiological signal data");
            LogToDebugConsole("  - Time-based sampled data (voltage, pressure, etc.)");
            LogToDebugConsole("  - Multi-channel support (e.g., 12-lead ECG)");
            LogToDebugConsole("  - Includes acquisition context and annotations");
            LogToDebugConsole("");

            LogToDebugConsole("Key Waveform Modules:");
            LogToDebugConsole("  - Patient Module");
            LogToDebugConsole("  - General Study Module");
            LogToDebugConsole("  - Waveform Series Module");
            LogToDebugConsole("  - Waveform Module");
            LogToDebugConsole("  - Waveform Annotation Module (optional)");
            LogToDebugConsole("  - Acquisition Context Module");
            LogToDebugConsole("");

            LogToDebugConsole("Core Waveform Attributes:");
            LogToDebugConsole("  (5400,0100) Waveform Sequence");
            LogToDebugConsole("  (003A,0005) Number of Waveform Channels");
            LogToDebugConsole("  (003A,0010) Number of Waveform Samples");
            LogToDebugConsole("  (003A,001A) Sampling Frequency");
            LogToDebugConsole("  (5400,0110) Channel Minimum Value");
            LogToDebugConsole("  (5400,0112) Channel Maximum Value");
            LogToDebugConsole("  (5400,1004) Waveform Bits Allocated");
            LogToDebugConsole("  (5400,1006) Waveform Sample Interpretation");
            LogToDebugConsole("  (5400,1010) Waveform Data");
        }

        /// <summary>
        /// Demonstrate waveform SOP Classes
        /// </summary>
        private static void DemonstrateWaveformSOPClasses()
        {
            LogToDebugConsole("");
            LogToDebugConsole("ECG Waveform SOP Classes:");
            LogToDebugConsole("");
            LogToDebugConsole("  12-Lead ECG Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.1.1");
            LogToDebugConsole("    Standard diagnostic 12-lead ECG");
            LogToDebugConsole("    Leads: I, II, III, aVR, aVL, aVF, V1-V6");
            LogToDebugConsole("");

            LogToDebugConsole("  General ECG Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.1.2");
            LogToDebugConsole("    Flexible ECG format");
            LogToDebugConsole("    Variable lead configurations");
            LogToDebugConsole("");

            LogToDebugConsole("  Ambulatory ECG Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.1.3");
            LogToDebugConsole("    Holter monitoring data");
            LogToDebugConsole("    Long duration recordings");
            LogToDebugConsole("");

            LogToDebugConsole("Other Waveform SOP Classes:");
            LogToDebugConsole("");
            LogToDebugConsole("  Hemodynamic Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.2.1");
            LogToDebugConsole("    Blood pressure, cardiac output");
            LogToDebugConsole("");

            LogToDebugConsole("  Cardiac Electrophysiology Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.3.1");
            LogToDebugConsole("    Intracardiac signals");
            LogToDebugConsole("");

            LogToDebugConsole("  Basic Voice Audio Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.4.1");
            LogToDebugConsole("    Voice annotations, dictation");
            LogToDebugConsole("");

            LogToDebugConsole("  Respiratory Waveform Storage:");
            LogToDebugConsole("    UID: 1.2.840.10008.5.1.4.1.1.9.6.1");
            LogToDebugConsole("    Spirometry, respiratory flow");
        }

        /// <summary>
        /// Demonstrate waveform sequence structure
        /// </summary>
        private static void DemonstrateWaveformSequence()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Waveform Sequence (5400,0100):");
            LogToDebugConsole("  Contains one or more waveform multiplex groups");
            LogToDebugConsole("");

            LogToDebugConsole("Each Waveform Item contains:");
            LogToDebugConsole("");
            LogToDebugConsole("  Multiplex Group Identification:");
            LogToDebugConsole("    (003A,0020) Multiplex Group Time Offset");
            LogToDebugConsole("    (003A,0200) Channel Definition Sequence");
            LogToDebugConsole("");

            LogToDebugConsole("  Sampling Information:");
            LogToDebugConsole("    (003A,001A) Sampling Frequency (Hz)");
            LogToDebugConsole("    (003A,0005) Number of Waveform Channels");
            LogToDebugConsole("    (003A,0010) Number of Waveform Samples");
            LogToDebugConsole("");

            LogToDebugConsole("  Data Encoding:");
            LogToDebugConsole("    (5400,1004) Waveform Bits Allocated (8 or 16)");
            LogToDebugConsole("    (5400,1006) Waveform Sample Interpretation:");
            LogToDebugConsole("                SS (signed 16-bit)");
            LogToDebugConsole("                US (unsigned 16-bit)");
            LogToDebugConsole("                SB (signed 8-bit)");
            LogToDebugConsole("                UB (unsigned 8-bit)");
            LogToDebugConsole("");

            LogToDebugConsole("  Waveform Data:");
            LogToDebugConsole("    (5400,1010) Waveform Data (OW/OB)");
            LogToDebugConsole("    Interleaved samples: Ch1S1, Ch2S1, Ch1S2, Ch2S2...");
            LogToDebugConsole("");

            LogToDebugConsole("Example Data Layout (2 channels, 3 samples):");
            LogToDebugConsole("  [Ch1S1][Ch2S1][Ch1S2][Ch2S2][Ch1S3][Ch2S3]");
        }

        /// <summary>
        /// Demonstrate channel definitions
        /// </summary>
        private static void DemonstrateChannelDefinitions()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Channel Definition Sequence (003A,0200):");
            LogToDebugConsole("  One item per channel in the multiplex group");
            LogToDebugConsole("");

            LogToDebugConsole("Channel Attributes:");
            LogToDebugConsole("");
            LogToDebugConsole("  Source Identification:");
            LogToDebugConsole("    (003A,0208) Channel Source Sequence");
            LogToDebugConsole("      Code Value, Coding Scheme, Code Meaning");
            LogToDebugConsole("      Example: (5.6.3-9-1, SCPECG, Lead I)");
            LogToDebugConsole("");

            LogToDebugConsole("  Sensitivity & Calibration:");
            LogToDebugConsole("    (003A,0210) Channel Sensitivity (e.g., 1.0)");
            LogToDebugConsole("    (003A,0211) Channel Sensitivity Units Sequence");
            LogToDebugConsole("      Example: uV (microvolts)");
            LogToDebugConsole("    (003A,0212) Channel Sensitivity Correction Factor");
            LogToDebugConsole("");

            LogToDebugConsole("  Baseline:");
            LogToDebugConsole("    (003A,0213) Channel Baseline");
            LogToDebugConsole("    (5400,0110) Channel Minimum Value");
            LogToDebugConsole("    (5400,0112) Channel Maximum Value");
            LogToDebugConsole("");

            LogToDebugConsole("  Filtering:");
            LogToDebugConsole("    (003A,0220) Filter Low Frequency");
            LogToDebugConsole("    (003A,0221) Filter High Frequency");
            LogToDebugConsole("    (003A,0222) Notch Filter Frequency");
            LogToDebugConsole("    (003A,0223) Notch Filter Bandwidth");
            LogToDebugConsole("");

            LogToDebugConsole("Standard ECG Lead Codes (SCPECG):");
            LogToDebugConsole("  | Lead | Code       | Meaning    |");
            LogToDebugConsole("  |------|------------|------------|");
            LogToDebugConsole("  | I    | 5.6.3-9-1  | Lead I     |");
            LogToDebugConsole("  | II   | 5.6.3-9-2  | Lead II    |");
            LogToDebugConsole("  | III  | 5.6.3-9-61 | Lead III   |");
            LogToDebugConsole("  | aVR  | 5.6.3-9-62 | Lead aVR   |");
            LogToDebugConsole("  | aVL  | 5.6.3-9-63 | Lead aVL   |");
            LogToDebugConsole("  | aVF  | 5.6.3-9-64 | Lead aVF   |");
            LogToDebugConsole("  | V1   | 5.6.3-9-3  | Lead V1    |");
            LogToDebugConsole("  | V2   | 5.6.3-9-4  | Lead V2    |");
            LogToDebugConsole("  | V3   | 5.6.3-9-5  | Lead V3    |");
            LogToDebugConsole("  | V4   | 5.6.3-9-6  | Lead V4    |");
            LogToDebugConsole("  | V5   | 5.6.3-9-7  | Lead V5    |");
            LogToDebugConsole("  | V6   | 5.6.3-9-8  | Lead V6    |");
        }

        /// <summary>
        /// Demonstrate creating waveform with fo-dicom
        /// </summary>
        private static void DemonstrateWaveformCreation()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Creating ECG Waveform with fo-dicom:");
            LogToDebugConsole("");

            // Create main dataset
            var dataset = new DicomDataset();

            // Patient Module
            dataset.Add(DicomTag.PatientName, "Test^Patient");
            dataset.Add(DicomTag.PatientID, "12345");
            dataset.Add(DicomTag.PatientBirthDate, "19800101");
            dataset.Add(DicomTag.PatientSex, "M");

            // Study Module
            dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.StudyDate, DateTime.Now);
            dataset.Add(DicomTag.StudyTime, DateTime.Now);
            dataset.Add(DicomTag.AccessionNumber, "ECG001");

            // Series Module
            dataset.Add(DicomTag.Modality, "ECG");
            dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
            dataset.Add(DicomTag.SeriesNumber, "1");

            // SOP Common
            dataset.Add(DicomTag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.9.1.2"); // General ECG
            dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());

            // Waveform parameters
            int numChannels = 2;
            int numSamples = 500; // 1 second at 500 Hz
            double samplingFrequency = 500.0;

            LogToDebugConsole("Waveform Parameters:");
            LogToDebugConsole($"  Channels: {numChannels}");
            LogToDebugConsole($"  Samples: {numSamples}");
            LogToDebugConsole($"  Sampling Frequency: {samplingFrequency} Hz");
            LogToDebugConsole("");

            // Create waveform sequence
            var waveformSequence = new DicomSequence(DicomTag.WaveformSequence);
            var waveformItem = new DicomDataset();

            // Multiplex group attributes
            waveformItem.Add(DicomTag.MultiplexGroupTimeOffset, 0.0m);
            waveformItem.Add(DicomTag.SamplingFrequency, (decimal)samplingFrequency);
            waveformItem.Add(DicomTag.NumberOfWaveformChannels, (ushort)numChannels);
            waveformItem.Add(DicomTag.NumberOfWaveformSamples, (uint)numSamples);
            waveformItem.Add(DicomTag.WaveformBitsAllocated, (ushort)16);
            waveformItem.Add(new DicomCodeString(DicomTag.WaveformSampleInterpretation, "SS")); // Signed 16-bit

            // Channel Definition Sequence
            var channelDefSequence = new DicomSequence(DicomTag.ChannelDefinitionSequence);

            // Channel 1: Lead I
            var channel1 = new DicomDataset();
            var sourceSeq1 = new DicomSequence(DicomTag.ChannelSourceSequence);
            var source1 = new DicomDataset();
            source1.Add(DicomTag.CodeValue, "5.6.3-9-1");
            source1.Add(DicomTag.CodingSchemeDesignator, "SCPECG");
            source1.Add(DicomTag.CodeMeaning, "Lead I");
            sourceSeq1.Items.Add(source1);
            channel1.Add(sourceSeq1);
            channel1.Add(DicomTag.ChannelSensitivity, 1.0m);

            var unitsSeq1 = new DicomSequence(DicomTag.ChannelSensitivityUnitsSequence);
            var units1 = new DicomDataset();
            units1.Add(DicomTag.CodeValue, "uV");
            units1.Add(DicomTag.CodingSchemeDesignator, "UCUM");
            units1.Add(DicomTag.CodeMeaning, "microvolt");
            unitsSeq1.Items.Add(units1);
            channel1.Add(unitsSeq1);

            channel1.Add(DicomTag.ChannelSensitivityCorrectionFactor, 1.0m);
            channel1.Add(DicomTag.ChannelBaseline, 0.0m);
            channel1.Add(DicomTag.FilterLowFrequency, 0.05m);
            channel1.Add(DicomTag.FilterHighFrequency, 150.0m);
            channelDefSequence.Items.Add(channel1);

            // Channel 2: Lead II
            var channel2 = new DicomDataset();
            var sourceSeq2 = new DicomSequence(DicomTag.ChannelSourceSequence);
            var source2 = new DicomDataset();
            source2.Add(DicomTag.CodeValue, "5.6.3-9-2");
            source2.Add(DicomTag.CodingSchemeDesignator, "SCPECG");
            source2.Add(DicomTag.CodeMeaning, "Lead II");
            sourceSeq2.Items.Add(source2);
            channel2.Add(sourceSeq2);
            channel2.Add(DicomTag.ChannelSensitivity, 1.0m);
            channel2.Add(unitsSeq1); // Same units
            channel2.Add(DicomTag.ChannelSensitivityCorrectionFactor, 1.0m);
            channel2.Add(DicomTag.ChannelBaseline, 0.0m);
            channel2.Add(DicomTag.FilterLowFrequency, 0.05m);
            channel2.Add(DicomTag.FilterHighFrequency, 150.0m);
            channelDefSequence.Items.Add(channel2);

            waveformItem.Add(channelDefSequence);

            // Generate synthetic ECG data (simplified sine waves)
            short[] waveformData = new short[numChannels * numSamples];
            for (int s = 0; s < numSamples; s++)
            {
                double t = s / samplingFrequency;
                // Lead I - simulated ECG pattern
                waveformData[s * numChannels + 0] = (short)(1000 * Math.Sin(2 * Math.PI * 1.2 * t));
                // Lead II - slightly different pattern
                waveformData[s * numChannels + 1] = (short)(1200 * Math.Sin(2 * Math.PI * 1.2 * t + 0.2));
            }

            // Convert to byte array
            byte[] waveformBytes = new byte[waveformData.Length * 2];
            Buffer.BlockCopy(waveformData, 0, waveformBytes, 0, waveformBytes.Length);
            waveformItem.Add(DicomTag.WaveformData, waveformBytes);

            waveformSequence.Items.Add(waveformItem);
            dataset.Add(waveformSequence);

            LogToDebugConsole("Waveform dataset created:");
            LogToDebugConsole("  Channel 1: Lead I");
            LogToDebugConsole("  Channel 2: Lead II");
            LogToDebugConsole($"  Data size: {waveformBytes.Length} bytes");
            LogToDebugConsole("");

            // Could save with:
            // var file = new DicomFile(dataset);
            // file.Save("waveform.dcm");
            LogToDebugConsole("Note: Use DicomFile.Save() to persist to disk.");
        }

        /// <summary>
        /// Demonstrate waveform annotations
        /// </summary>
        private static void DemonstrateWaveformAnnotations()
        {
            LogToDebugConsole("");
            LogToDebugConsole("Waveform Annotation Module:");
            LogToDebugConsole("");

            LogToDebugConsole("Waveform Annotation Sequence (0040,B020):");
            LogToDebugConsole("  Stores annotations linked to waveform data");
            LogToDebugConsole("");

            LogToDebugConsole("Annotation Attributes:");
            LogToDebugConsole("  (0040,A180) Annotation Group Number");
            LogToDebugConsole("  (0040,A195) Concept Name Code Sequence");
            LogToDebugConsole("  (0040,A0B0) Referenced Waveform Channels");
            LogToDebugConsole("  (0040,A130) Temporal Range Type:");
            LogToDebugConsole("              POINT, MULTIPOINT, SEGMENT, etc.");
            LogToDebugConsole("  (0040,A132) Referenced Sample Positions");
            LogToDebugConsole("  (0040,A138) Referenced Time Offsets");
            LogToDebugConsole("");

            LogToDebugConsole("Common ECG Annotations:");
            LogToDebugConsole("  | Annotation | Code    | Description            |");
            LogToDebugConsole("  |------------|---------|------------------------|");
            LogToDebugConsole("  | QRS onset  | MDC_... | Start of QRS complex   |");
            LogToDebugConsole("  | QRS offset | MDC_... | End of QRS complex     |");
            LogToDebugConsole("  | P wave     | MDC_... | Atrial depolarization  |");
            LogToDebugConsole("  | T wave     | MDC_... | Ventricular repol.     |");
            LogToDebugConsole("  | R-R int.   | MDC_... | Beat interval          |");
            LogToDebugConsole("  | Heart rate | MDC_... | Computed HR            |");
            LogToDebugConsole("");

            LogToDebugConsole("Measurement Annotations:");
            LogToDebugConsole("  - PR Interval");
            LogToDebugConsole("  - QRS Duration");
            LogToDebugConsole("  - QT Interval");
            LogToDebugConsole("  - QTc (corrected)");
            LogToDebugConsole("  - Axis measurements");
            LogToDebugConsole("");

            LogToDebugConsole("Example Annotation (R peak):");
            LogToDebugConsole("  Concept Name: (F-32010, SRT, \"R wave\")");
            LogToDebugConsole("  Temporal Range Type: POINT");
            LogToDebugConsole("  Referenced Sample Positions: 125");
            LogToDebugConsole("  Referenced Waveform Channels: 1\\\\2");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
