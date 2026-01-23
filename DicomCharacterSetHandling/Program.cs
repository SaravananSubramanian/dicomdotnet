//-----------------------------------------------------------------------
// Tutorial: DICOM Character Set Handling
//-----------------------------------------------------------------------
// Purpose:
//   Demonstrates how DICOM handles international character sets for
//   patient names, institution names, and other text data.
//
// Key Concepts:
//   - DICOM Specific Character Set (0008,0005) attribute
//   - Default character set is ASCII (ISO 646)
//   - Extended character sets: ISO 8859 series, UTF-8, Japanese, etc.
//   - Person Name components: Alphabetic=Ideographic=Phonetic
//   - UTF-8 (ISO_IR 192) is recommended for new implementations
//
// Requirements:
//   - No external server connection required
//   - No test files required
//
// fo-dicom References:
//   - DicomDataset - Container for DICOM attributes
//   - DicomEncoding - Character set handling
//   - DicomTag.SpecificCharacterSet - Character set identifier
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;
using FellowOakDicom;

namespace Com.SaravananSubramanian.DicomCharacterSetHandling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                LogToDebugConsole("=== DICOM Character Set Handling Demo ===");
                LogToDebugConsole("");

                // Demo 1: Default ASCII character set
                LogToDebugConsole("--- Demo 1: Default (ASCII) ---");
                DemonstrateDefaultCharacterSet();

                // Demo 2: ISO 8859-1 (Latin-1) for Western European
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 2: ISO 8859-1 (Latin-1) ---");
                DemonstrateLatin1CharacterSet();

                // Demo 3: UTF-8 (Unicode)
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 3: UTF-8 (Unicode) ---");
                DemonstrateUtf8CharacterSet();

                // Demo 4: Japanese character set
                LogToDebugConsole("");
                LogToDebugConsole("--- Demo 4: Japanese ---");
                DemonstrateJapaneseCharacterSet();

                // Character set reference
                LogToDebugConsole("");
                LogToDebugConsole("--- Character Set Reference ---");
                DemonstrateCharacterSetReference();
            }
            catch (Exception e)
            {
                LogToDebugConsole($"Error: {e.Message}");
                LogToDebugConsole($"Stack trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Demonstrate default (ASCII) character set
        /// </summary>
        private static void DemonstrateDefaultCharacterSet()
        {
            var dataset = new DicomDataset();

            // No Specific Character Set - defaults to ASCII
            // Only basic Latin characters allowed: A-Z, a-z, 0-9, basic punctuation

            dataset.Add(DicomTag.PatientName, "Doe^John");
            dataset.Add(DicomTag.InstitutionName, "General Hospital");

            LogToDebugConsole("Patient Name: Doe^John");
            LogToDebugConsole("Institution: General Hospital");
            LogToDebugConsole("Specific Character Set: (not specified - ASCII default)");
            LogToDebugConsole("Supported characters: A-Z, a-z, 0-9, basic punctuation");
        }

        /// <summary>
        /// Demonstrate ISO 8859-1 (Latin-1) for Western European languages
        /// </summary>
        private static void DemonstrateLatin1CharacterSet()
        {
            var dataset = new DicomDataset();

            // Set ISO 8859-1 (Latin-1) character set
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 100"); // ISO 8859-1

            // Now we can use accented characters
            dataset.Add(DicomTag.PatientName, "Muller^Francois");
            dataset.Add(DicomTag.InstitutionName, "Hopital General de Zurich");

            LogToDebugConsole("Patient Name: Muller^Francois (would be Mueller^Francois with umlauts)");
            LogToDebugConsole("Institution: Hopital General de Zurich (would have accents)");
            LogToDebugConsole("Specific Character Set: ISO_IR 100 (ISO 8859-1, Latin-1)");
            LogToDebugConsole("Supports: Western European languages (French, German, Spanish, etc.)");
        }

        /// <summary>
        /// Demonstrate UTF-8 (Unicode) character set
        /// </summary>
        private static void DemonstrateUtf8CharacterSet()
        {
            var dataset = new DicomDataset();

            // Set UTF-8 character set
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192"); // UTF-8

            // UTF-8 supports all Unicode characters
            LogToDebugConsole("Examples of names in different scripts:");
            LogToDebugConsole("");
            LogToDebugConsole("  Chinese: Wang Xiaoming");
            LogToDebugConsole("  Japanese: Yamada Taro");
            LogToDebugConsole("  Korean: Kim Cheolsu");
            LogToDebugConsole("  Arabic: Muhammad Ahmad");
            LogToDebugConsole("  Russian: Ivan Petrov");
            LogToDebugConsole("  Greek: Nikos Papadopoulos");
            LogToDebugConsole("");
            LogToDebugConsole("Specific Character Set: ISO_IR 192 (UTF-8)");
            LogToDebugConsole("Supports: All Unicode characters - RECOMMENDED for new implementations");
        }

        /// <summary>
        /// Demonstrate Japanese character set
        /// </summary>
        private static void DemonstrateJapaneseCharacterSet()
        {
            var dataset = new DicomDataset();

            // Japanese requires multiple character sets with code extensions
            dataset.Add(DicomTag.SpecificCharacterSet, @"\ISO 2022 IR 87");

            LogToDebugConsole("Japanese names use components:");
            LogToDebugConsole("  Alphabetic: Yamada^Taro");
            LogToDebugConsole("  Ideographic: (Kanji characters)");
            LogToDebugConsole("  Phonetic: (Hiragana characters)");
            LogToDebugConsole("");
            LogToDebugConsole("Person Name format with components:");
            LogToDebugConsole("  Alphabetic=Ideographic=Phonetic");
            LogToDebugConsole("  Yamada^Taro=(Kanji)=(Hiragana)");
            LogToDebugConsole("");
            LogToDebugConsole(@"Specific Character Set: \ISO 2022 IR 87");
            LogToDebugConsole("Note: Backslash indicates code extension for second component");
        }

        /// <summary>
        /// Character set reference
        /// </summary>
        private static void DemonstrateCharacterSetReference()
        {
            LogToDebugConsole("");
            LogToDebugConsole("=== Character Set Code Reference ===");
            LogToDebugConsole("");

            LogToDebugConsole("Single-Byte Character Sets:");
            LogToDebugConsole("  (empty)      - ASCII (ISO 646, default)");
            LogToDebugConsole("  ISO_IR 100   - ISO 8859-1 (Latin-1, Western European)");
            LogToDebugConsole("  ISO_IR 101   - ISO 8859-2 (Latin-2, Central European)");
            LogToDebugConsole("  ISO_IR 109   - ISO 8859-3 (Latin-3, South European)");
            LogToDebugConsole("  ISO_IR 110   - ISO 8859-4 (Latin-4, North European)");
            LogToDebugConsole("  ISO_IR 144   - ISO 8859-5 (Cyrillic)");
            LogToDebugConsole("  ISO_IR 127   - ISO 8859-6 (Arabic)");
            LogToDebugConsole("  ISO_IR 126   - ISO 8859-7 (Greek)");
            LogToDebugConsole("  ISO_IR 138   - ISO 8859-8 (Hebrew)");
            LogToDebugConsole("  ISO_IR 148   - ISO 8859-9 (Latin-5, Turkish)");
            LogToDebugConsole("  ISO_IR 166   - TIS 620 (Thai)");
            LogToDebugConsole("");

            LogToDebugConsole("Multi-Byte Character Sets:");
            LogToDebugConsole("  ISO_IR 192   - UTF-8 (Unicode) *** RECOMMENDED ***");
            LogToDebugConsole("  GB18030      - Chinese (Simplified + Traditional)");
            LogToDebugConsole("  GBK          - Chinese (Simplified)");
            LogToDebugConsole("");

            LogToDebugConsole("Code Extension Character Sets (ISO 2022):");
            LogToDebugConsole("  ISO 2022 IR 6    - ASCII (default)");
            LogToDebugConsole("  ISO 2022 IR 13   - JIS X 0201 (Katakana)");
            LogToDebugConsole("  ISO 2022 IR 87   - JIS X 0208 (Japanese)");
            LogToDebugConsole("  ISO 2022 IR 159  - JIS X 0212 (Japanese supplement)");
            LogToDebugConsole("  ISO 2022 IR 149  - KS X 1001 (Korean)");
            LogToDebugConsole("  ISO 2022 IR 58   - GB 2312 (Chinese Simplified)");
            LogToDebugConsole("");

            LogToDebugConsole("Person Name Components:");
            LogToDebugConsole("  Format: Alphabetic=Ideographic=Phonetic");
            LogToDebugConsole("  Each component: Family^Given^Middle^Prefix^Suffix");
            LogToDebugConsole("  Example: Smith^John=(ideographic)=(phonetic)");
            LogToDebugConsole("");

            LogToDebugConsole("Best Practices:");
            LogToDebugConsole("  1. Use UTF-8 (ISO_IR 192) for new implementations");
            LogToDebugConsole("  2. Always specify Specific Character Set explicitly");
            LogToDebugConsole("  3. Test with international character data");
            LogToDebugConsole("  4. Handle encoding/decoding errors gracefully");
            LogToDebugConsole("  5. Be aware of legacy system character set limitations");
        }

        private static void LogToDebugConsole(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
