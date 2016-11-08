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
                                       ".group {{ background-color: #93B4BF; }}" + 
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

            var totalTopAdvertismentsCount = companies.Sum(c => c.TopAdvertismentsCount);
            var totalBottomAdvertismentsCount = companies.Sum(c => c.BottomAdvertismentsCount);
            var totalCount = totalTopAdvertismentsCount + totalBottomAdvertismentsCount;
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td>Всего(СР/Г)</td>");
            sb.AppendFormat("<td>{0}({1}/{2})</td>", totalCount, totalTopAdvertismentsCount, totalBottomAdvertismentsCount);
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
            sb.AppendLine("<td>Количество</br> объявлений </br>конкурентов</br>Всего(СР/Г)</td>");
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
            var groups = new Queue<string>();
            foreach (var reportRow in report.Rows)
            {
                if (reportRow.QueryGroup.Count != 0)
                {
                    reportRow.QueryGroup.ForEach(g => groups.Enqueue(g));
                    sb.AppendLine(GetGroupRow(groups));
                }
                sb.AppendLine(GetRow(reportRow, groups));
            }

            return sb.ToString();
        }

        private string GetGroupRow(Queue<string> groups)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");
            
            sb.AppendFormat("<td>{0}</td>", groups.Dequeue());
            for (int i = 0; i < 3 + CompanyCount; i++)
            {
                sb.AppendLine("<td class='group'></td>");
            }
            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        private string GetRow(RivalListReport.ReportRow reportRow, Queue<string> groups)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendFormat("<td>{0}</td>", groups.Count > 0 ? groups.Dequeue() : "");
            sb.AppendLine("<td>" + reportRow.QueryName + "</td>");
            sb.AppendFormat("<td>{0}</td>", reportRow.Companies.Count);
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
                        var queryResult = companyAdv.Advertisments[reportRow.QueryName];
                        advPosition = (queryResult.ResultType == QResultType.TopAdvertisement ? "СР" : "Г") + queryResult.ResultNumber;
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