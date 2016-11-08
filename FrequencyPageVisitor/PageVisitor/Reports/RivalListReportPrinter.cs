using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Settings;

namespace FrequencyPageVisitor.Reports
{
    public class RivalListReportPrinter
    {
        private readonly RivalListReport _report;

        public RivalListReportPrinter(RivalListReport report)
        {
            _report = report;

            var rows = GetRowsLayout(report);
        }

        private string GetRowsLayout(RivalListReport report)
        {
            var sb = new StringBuilder();
            foreach (var reportRow in report.Rows)
            {
                sb.AppendLine(GetRow(reportRow, report.Companies));
            }
        }

        private string GetRow(RivalListReport.ReportRow reportRow, List<RivalListReport.CompanyAdverisment> companies)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendLine("<td>");
            //Группировка
            sb.AppendLine("</td>");

            sb.AppendLine("<td>");
            sb.AppendLine(reportRow.QueryName);
            sb.AppendLine("</td>");

            sb.AppendLine("<td>");
            sb.AppendLine(reportRow.Companies.Count.ToString());
            sb.AppendLine("</td>");

            sb.AppendLine("<td>");
            sb.AppendLine(reportRow.Frequency);
            sb.AppendLine("</td>");

            for (int i = 0; i <= 5; i++)
            {
                var companyColumn = _report.Companies[i];
                var companyAdv = reportRow.Companies.FirstOrDefault(c => c.CompanyName == companyColumn.CompanyName);

                string advPosition = string.Empty;
                if (companyAdv != null)
                {
                    if (companyAdv.Advertisments.ContainsKey(reportRow.QueryName))
                    {
                        advPosition = companyAdv.Advertisments[reportRow.QueryName].ResultType == QResultType.TopAdvertisement ? "СР" : "Г";
                    }
                }

                sb.AppendLine("<td>");
                sb.AppendLine(advPosition);
                sb.AppendLine("</td>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }
    }
}