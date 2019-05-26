using System;
using Dicom;
using Dicom.Media;

namespace UnderstandingDicomDirectoryPart2
{
    public class OurDicomDirectoryHelper
    {
        private readonly Action<string> _log;

        public OurDicomDirectoryHelper(Action<string> log)
        {
            _log = log;
        }
        public void ShowDicomDirectoryMetaInformation(DicomDirectory dicomDirectory)
        {
            _log($"Dicom Directory Information:");
            var fileMetaInfo = dicomDirectory.FileMetaInfo;
            _log($"Media Storage SOP Class UID: '{fileMetaInfo.MediaStorageSOPClassUID}'");
            _log($"Media Storage SOP Instance UID: '{fileMetaInfo.MediaStorageSOPInstanceUID}'");
            _log($"Transfer Syntax: '{fileMetaInfo.TransferSyntax}'");
            _log($"Implementation Class UID: '{fileMetaInfo.ImplementationClassUID}'");
            _log($"Implementation Version Name: '{fileMetaInfo.ImplementationVersionName}'");
            _log($"Source Application Entity Title: '{fileMetaInfo.SourceApplicationEntityTitle}'");
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
                    _log($"Unknown directory record type: {record.DirectoryRecordType}. " +
                            $"Please check your inputs");
                    break;

            };
        }

        private void ShowImageLevelInfo(DicomDataset dataset)
        {
            _log("\t\t\tImage Level Information:");
            var values = dataset.GetValues<string>(DicomTag.ReferencedFileID);
            var referencedFileId = string.Join(@"\", values);
            _log($"\t\t\t-> Referenced File ID '{referencedFileId}'");
            //Please see https://www.dicomlibrary.com/dicom/sop/ for what these UIDs represent
            var sopClassUidInFile = dataset.GetValue<string>(DicomTag.ReferencedSOPClassUIDInFile, 0);
            _log($"\t\t\t-> Referenced SOP Class UID In File '{sopClassUidInFile}'");
            var sopInstanceUidInFile = dataset.GetValue<string>(DicomTag.ReferencedSOPInstanceUIDInFile, 0);
            _log($"\t\t\t-> Referenced SOP Instance UID In File '{sopInstanceUidInFile}'");
            var transferSyntaxUidInFile = dataset.GetValue<string>(DicomTag.ReferencedTransferSyntaxUIDInFile, 0);
            _log($"\t\t\t-> Referenced Transfer Syntax UID In File '{transferSyntaxUidInFile}'");
        }

        private void ShowSeriesLevelInfo(DicomDataset dataset)
        {
            _log("\t\tSeries Level Information:");
            var seriesInstanceUid = dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            _log($"\t\t-> Series Instance UID: '{seriesInstanceUid}'");
            var modality = dataset.GetSingleValue<string>(DicomTag.Modality);
            _log($"\t\t-> Series Modality: '{modality}'");
        }

        private void ShowStudyLevelInfo(DicomDataset dataset)
        {
            _log("\tStudy Level Information:");
            var studyInstanceUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            _log($"\t-> Study Instance UID: '{studyInstanceUid}'");
            _log($"\t-> Study ID: '{dataset.GetSingleValue<string>(DicomTag.StudyID)}'");
            _log($"\t-> Study Date: '{dataset.GetSingleValue<string>(DicomTag.StudyDate)}'");
        }

        private void ShowPatientLevelInfo(DicomDataset dataset)
        {
            _log("Patient Level Information:");
            _log($"-> Patient Name: '{dataset.GetSingleValue<string>(DicomTag.PatientName)}'");
            _log($"-> Patient ID: '{dataset.GetSingleValue<string>(DicomTag.PatientID)}'");
        }
    }
}
