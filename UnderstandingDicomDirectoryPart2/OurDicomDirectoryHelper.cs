using System;
using Dicom;
using Dicom.Media;

namespace UnderstandingDicomDirectoryPart2
{
    public class OurDicomDirectoryHelper
    {
        private readonly Action<string> _logger;

        public OurDicomDirectoryHelper(Action<string> logger)
        {
            _logger = logger;
        }
        public void ShowDicomDirectoryMetaInformation(DicomDirectory dicomDirectory)
        {
            _logger($"Dicom Directory Information:");
            var fileMetaInfo = dicomDirectory.FileMetaInfo;
            _logger($"Media Storage SOP Class UID: '{fileMetaInfo.MediaStorageSOPClassUID}'");
            _logger($"Media Storage SOP Instance UID: '{fileMetaInfo.MediaStorageSOPInstanceUID}'");
            _logger($"Transfer Syntax: '{fileMetaInfo.TransferSyntax}'");
            _logger($"Implementation Class UID: '{fileMetaInfo.ImplementationClassUID}'");
            _logger($"Implementation Version Name: '{fileMetaInfo.ImplementationVersionName}'");
            _logger($"Source Application Entity Title: '{fileMetaInfo.SourceApplicationEntityTitle}'");
        }

        public void Display(DicomDirectoryRecord record)
        {
            switch (record.DirectoryRecordType)
            {
                case "PATIENT":
                    ShowPatientLevelInfo(record);
                    break;
                case "STUDY":
                    ShowStudyLevelInfo(record);
                    break;
                case "SERIES":
                    ShowSeriesLevelInfo(record);
                    break;
                case "IMAGE":
                    ShowImageLevelInfo(record);
                    break;
                default:
                    _logger($"Unknown directory record type: {record.DirectoryRecordType}. " +
                            $"Please check your inputs");
                    break;

            };
        }

        private void ShowImageLevelInfo(DicomDataset dataset)
        {
            _logger("\t\t\tImage Level Information:");
            var values = dataset.GetValues<string>(DicomTag.ReferencedFileID);
            var referencedFileId = string.Join(@"\", values);
            _logger($"\t\t\t-> Referenced File ID '{referencedFileId}'");
            //Please see https://www.dicomlibrary.com/dicom/sop/ for what these UIDs represent
            var sopClassUidInFile = dataset.GetValue<string>(DicomTag.ReferencedSOPClassUIDInFile, 0);
            _logger($"\t\t\t-> Referenced SOP Class UID In File '{sopClassUidInFile}'");
            var sopInstanceUidInFile = dataset.GetValue<string>(DicomTag.ReferencedSOPInstanceUIDInFile, 0);
            _logger($"\t\t\t-> Referenced SOP Instance UID In File '{sopInstanceUidInFile}'");
            var transferSyntaxUidInFile = dataset.GetValue<string>(DicomTag.ReferencedTransferSyntaxUIDInFile, 0);
            _logger($"\t\t\t-> Referenced Transfer Syntax UID In File '{transferSyntaxUidInFile}'");
        }

        private void ShowSeriesLevelInfo(DicomDataset dataset)
        {
            _logger("\t\tSeries Level Information:");
            var seriesInstanceUid = dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            _logger($"\t\t-> Series Instance UID: '{seriesInstanceUid}'");
            var modality = dataset.GetSingleValue<string>(DicomTag.Modality);
            _logger($"\t\t-> Series Modality: '{modality}'");
        }

        private void ShowStudyLevelInfo(DicomDataset dataset)
        {
            _logger("\tStudy Level Information:");
            var studyInstanceUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            _logger($"\t-> Study Instance UID: '{studyInstanceUid}'");
            _logger($"\t-> Study ID: '{dataset.GetSingleValue<string>(DicomTag.StudyID)}'");
            _logger($"\t-> Study Date: '{dataset.GetSingleValue<string>(DicomTag.StudyDate)}'");
        }

        private void ShowPatientLevelInfo(DicomDataset dataset)
        {
            _logger("Patient Level Information:");
            _logger($"-> Patient Name: '{dataset.GetSingleValue<string>(DicomTag.PatientName)}'");
            _logger($"-> Patient ID: '{dataset.GetSingleValue<string>(DicomTag.PatientID)}'");
        }
    }
}
