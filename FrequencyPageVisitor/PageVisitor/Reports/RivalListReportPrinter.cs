using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Settings;

namespace FrequencyPageVisitor.Reports
{
    public class RivalListReportPrinter
    {
        private const int MaxCompanyCount = 5;

        private readonly int CompanyCount;
    

        private readonly RivalListReport _report;

        public RivalListReportPrinter(RivalListReport report)
        {
            _report = report;

            CompanyCount = report.Companies.Count < MaxCompanyCount ? report.Companies.Count : MaxCompanyCount;
        }

        public void Print(string path)
        {
            var tableHeader = GetTableHeader(_report.Companies);
            var rows = GetRowsLayout(_report);
            var tableBottom = GetTableBottom(_report.Companies);

            var result = string.Format("<html>" +
                                       "<body>" +
                                       "<head>" +
                                       "<style>" +
                                       "table  {{ border-collapse:collapse; }}" +
                                       "TD, TH  {{border:1px solid black; padding: 15px;}}" +
                                       ".bold {{ font-weight: bold; }}" + 
                                       "</style>" +
                                       "</head>" +

                                       "{0}{1}{2}" +
                                       "</body>" +
                                       "</html>", tableHeader, rows, tableBottom);
            File.WriteAllText(path, result);
        }

        private string GetTableBottom(List<RivalListReport.CompanyAdverisment> companies)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr class='bold' >");
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td>Всего(СР/Г)</td>");
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td></td>");

            for (int i = 0; i < CompanyCount; i++)
            {
                sb.AppendFormat("<td>{0}({1}/{2})</td>"
                    , companies[i].TopAdvertismentsCount + companies[i].BottomAdvertismentsCount,
                    companies[i].TopAdvertismentsCount,
                    companies[i].BottomAdvertismentsCount);
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string GetTableHeader(List<RivalListReport.CompanyAdverisment> companies)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<table>");
            
            sb.AppendLine("<tr>");
            
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td>Сайты</td>");
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td></td>");

            for (int i = 0; i < CompanyCount; i++)
            {
                sb.AppendFormat("<td>{0}</td>", companies[i].CompanyName);
            }


            sb.AppendLine("</tr>");

            sb.AppendLine("<tr class='bold'>");
            sb.AppendLine("<td class='bold'>Группировки запросов</td>");
            sb.AppendLine("<td>Запросы</td>");
            sb.AppendLine("<td>Количество</br> объявлений </br>конкурентов</td>");
            sb.AppendLine("<td>Частотность</td>");
            for (int i = 0; i < CompanyCount; i++)
            {
                sb.AppendFormat("<td></td>");
            }
            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        private string GetRowsLayout(RivalListReport report)
        {
            var sb = new StringBuilder();
            foreach (var reportRow in report.Rows)
            {
                sb.AppendLine(GetRow(reportRow));
            }

            return sb.ToString();
        }

        private string GetRow(RivalListReport.ReportRow reportRow)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendLine("<td></td>");
            sb.AppendLine("<td>" + reportRow.QueryName + "</td>");
            sb.AppendLine("<td>" + reportRow.Companies.Count.ToString() + "</td>");
            sb.AppendLine("<td>" + reportRow.Frequency + "</td>");

            for (int i = 0; i < CompanyCount; i++)
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