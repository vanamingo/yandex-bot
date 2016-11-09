using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Reports.Helpers;
using FrequencyPageVisitor.Settings;

namespace FrequencyPageVisitor.Reports
{
    public class RivalListReportPrinter
    {
        //private const int MaxCompanyCount = 5;
        //
        //private readonly int CompanyCount;
    

        private readonly RivalListReport _report;
        private readonly string _path;

        public RivalListReportPrinter(RivalListReport report, string path)
        {
            _report = report;
            _path = path;

            //CompanyCount = report.Companies.Count < MaxCompanyCount ? report.Companies.Count : MaxCompanyCount;
        }

        public void Print(int companiesOnPageCount)
        {
            var lastCompanyIndex = _report.Companies.Count - 1;
            PrintPage(0,lastCompanyIndex, "All");
            var firstIndex = 0;
            var lastIndex = 0;
            var num = 1;
            do
            {
                lastIndex = firstIndex + companiesOnPageCount;
                lastIndex = lastIndex > lastCompanyIndex ? lastCompanyIndex : lastIndex;
                PrintPage(firstIndex, lastIndex, num.ToString());
                firstIndex = lastIndex + 1;
                num++;
            }
            while (lastIndex < lastCompanyIndex);
        }

        private void PrintPage(int firstIndex, int lastIndex, string postFix)
        {
            var tableHeader = GetTableHeader(_report.Companies, firstIndex, lastIndex);
            var rows = GetRowsLayout(_report, firstIndex, lastIndex);
            var tableBottom = GetTableBottom(_report.Companies, firstIndex, lastIndex);

            var result = string.Format("<html>" +
                                       "<body>" +
                                       "<head>" +
                                       "<style >" +
                                       "table  {{ border-collapse:collapse; }}" +
                                       "TD, TH  {{border:1px solid black; padding: 15px;}}" +
                                       ".bold {{ font-weight: bold; }}" +
                                       ".group {{ background-color: #93B4BF; }}" +
                                       ".green {{ background-color: #71ba77; }}" +
                                       ".odd {{ background-color: #e5e5e5; }}" + 
                                       "</style>" +
                                       "</head>" +

                                       "{0}{1}{2}" +
                                       "</body>" +
                                       "</html>", tableHeader, rows, tableBottom);
            File.WriteAllText(String.Format(_path, postFix), result);
        }

        private string GetTableBottom(List<CompanyAdverisment> companies, int firstIndex, int lastIndex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr class='bold' >");

            var totalTopAdvertismentsCount = companies.Sum(c => c.TopAdvertismentsCount);
            var totalBottomAdvertismentsCount = companies.Sum(c => c.BottomAdvertismentsCount);
            var totalCount = totalTopAdvertismentsCount + totalBottomAdvertismentsCount;
            sb.AppendLine("<td class='green'></td>");
            sb.AppendLine("<td class='green'>Всего(СР/Г)</td>");
            sb.AppendFormat("<td class='green'>{0}({1}/{2})</td>", totalCount, totalTopAdvertismentsCount, totalBottomAdvertismentsCount);
            sb.AppendLine("<td class='green'></td>");

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                sb.AppendFormat("<td>{0}({1}/{2})</td>"
                    , companies[i].TopAdvertismentsCount + companies[i].BottomAdvertismentsCount,
                    companies[i].TopAdvertismentsCount,
                    companies[i].BottomAdvertismentsCount);
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string GetTableHeader(List<CompanyAdverisment> companies, int firstIndex, int lastIndex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<table>");
            
            sb.AppendLine("<tr>");

            sb.AppendLine("<td class='green'></td>");
            sb.AppendLine("<td class='green'>Сайты</td>");
            sb.AppendLine("<td class='green'></td>");
            sb.AppendLine("<td class='green'></td>");

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                sb.AppendFormat("<td>{0}</td>", companies[i].CompanyName);
            }


            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td class='bold green'>Группировки запросов</td>");
            sb.AppendLine("<td class='green'>Запросы</td>");
            sb.AppendLine("<td class='green'>Количество</br> объявлений </br>конкурентов</br>Всего(СР/Г)</td>");
            sb.AppendLine("<td class='green'>Частотность</td>");
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                sb.AppendFormat("<td></td>");
            }
            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        private string GetRowsLayout(RivalListReport report, int firstIndex, int lastIndex)
        {
            var sb = new StringBuilder();
            var groups = new Queue<string>();
            var rowNumber = 0;
            foreach (var reportRow in report.Rows)
            {
                if (reportRow.QueryGroup.Count != 0)
                {
                    reportRow.QueryGroup.ForEach(g => groups.Enqueue(g));
                    sb.AppendLine(GetGroupRow(groups, firstIndex, lastIndex));
                }
                sb.AppendLine(GetRow(reportRow, groups, firstIndex, lastIndex, rowNumber++));
            }

            return sb.ToString();
        }

        private string GetGroupRow(Queue<string> groups, int firstIndex, int lastIndex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendFormat("<td class='green'>{0}</td>", groups.Dequeue());
            for (int i = 0; i <= 3 + lastIndex - firstIndex; i++)
            {
                sb.AppendLine("<td class='group'></td>");
            }
            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        private string GetRow(RivalListReport.ReportRow reportRow, Queue<string> groups, int firstIndex, int lastIndex, int rowNumber)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendFormat("<td class='green'>{0}</td>", groups.Count > 0 ? groups.Dequeue() : "");
            sb.AppendLine("<td class='green'>" + reportRow.QueryName + "</td>");
            var advList = reportRow
                .Companies.Select(_ => _.Advertisments)
                .Where(a => a.ContainsKey(reportRow.QueryName))
                .ToList();

            var advTopCount = advList.Count(_ => _[reportRow.QueryName].ResultType == QResultType.TopAdvertisement);
            var advBottomCount = advList.Count(_ => _[reportRow.QueryName].ResultType == QResultType.BottomAdvertisement);

            sb.AppendFormat("<td class='green'>{0}({1}/{2})</td>", reportRow.Companies.Count, advTopCount, advBottomCount);
            sb.AppendLine("<td class='green'>" + reportRow.Frequency + "</td>");

            for (int i = firstIndex; i <= lastIndex; i++)
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

                sb.AppendFormat("<td {0}>", rowNumber % 2 != 0 ? "class='odd'": "");
                sb.AppendLine(advPosition);
                sb.AppendLine("</td>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }
    }
}