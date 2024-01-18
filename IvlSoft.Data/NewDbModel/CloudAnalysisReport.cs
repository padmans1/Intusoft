using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTUSOFT.Data.NewDbModel
{
    public class CloudAnalysisReport:  IDisposable, IBaseModel
    {
        public static CloudAnalysisReport CreateCloudAnalysisReport()
        {
            return new CloudAnalysisReport();
        }

        public CloudAnalysisReport()
        {
            this.createdDate = DateTime.Now;
            this.lastModifiedDate = DateTime.Now;
        }

        public static CloudAnalysisReport CreateCloudAnalysisReport(int cloudId, report Report, bool voided, int cloudAnalysisReportStatusStr, string leftEyeImp, string rightEyeImp, DateTime createdDateTime, DateTime lastModifiedDateTime)
        {
            return new CloudAnalysisReport
            {
                cloudAnalysisReportId = cloudId,
                Report = Report,
                voided = voided,
                leftEyeImpression = leftEyeImp,
                rightEyeImpression = rightEyeImp,
                cloudAnalysisReportStatus = cloudAnalysisReportStatusStr,
            };
        }
        public static CloudAnalysisReport CreateNewCloudAnlysisReport(CloudAnalysisReport proxyCloudAnalysisReport)
        {
            return new CloudAnalysisReport
            {
                cloudAnalysisReportId = proxyCloudAnalysisReport.cloudAnalysisReportId,
                Report= proxyCloudAnalysisReport.Report,
                leftEyeImpression = proxyCloudAnalysisReport.leftEyeImpression,
                rightEyeImpression = proxyCloudAnalysisReport.rightEyeImpression,
                voided = proxyCloudAnalysisReport.voided,
                cloudAnalysisReportStatus=proxyCloudAnalysisReport.cloudAnalysisReportStatus
            };
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }


        #region State Properties

        public DateTime lastModifiedDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual int cloudAnalysisReportId { get; set; }

        public virtual string leftEyeImpression { get; set; }

        public virtual string rightEyeImpression { get; set; }

        public virtual DateTime createdDate { get; set; }

        public virtual bool voided { get; set; }

        public virtual int cloudAnalysisReportStatus { get; set; }

        public virtual report Report { get; set; }

        #endregion
    }
}
