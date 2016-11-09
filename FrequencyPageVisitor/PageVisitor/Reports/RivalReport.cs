using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Reports.Helpers;

namespace FrequencyPageVisitor.Reports
{
    public class RivalReport
    {
        private readonly List<YandexPage> _yaPages;
        private readonly string _reportDir;
        public List<CompanyAdverisment> Companies { get; set; }

        public RivalReport(List<YandexPage> yaPages, string reportDir)
        {
            _yaPages = yaPages;
            _reportDir = reportDir;
            Companies = CompaniesProvider.GetCompanies(yaPages);
        }

        public void Print(string reportDir)
        {
            for (int i = 0; i < Companies.Count; i++)
            {
                PrintCompany(Companies[i], i);
            }
        }

        private void PrintCompany(CompanyAdverisment adv, int companyNum)
        {
            var tableHeader = GetHeader();
            var rows = GetRows(adv);

            var result = string.Format("<html>" +
               "<body>" +
               "<head>" +
               "<style>" +
               "table  {{ border-collapse:collapse; width:800px; margin: auto; }}" +
               "TD, TH  {{border:1px solid black; padding: 15px;}}" +
               ".bold {{ font-weight: bold; }}" +
               ".group {{ background-color: #93B4BF; }}" +
               ".green {{ background-color: #71ba77; }}" +
               ".odd {{ background-color: #e5e5e5; }}" +
               "</style>" +
               "</head>" +
               "<table>" +
               "{0}{1}" +
               "</table></body>" +
               "</html>", tableHeader, rows);
            var dirPath = Path.Combine(_reportDir, "Companies");
            var path = Path.Combine(dirPath, string.Format("{0}-{1}.html", companyNum, adv.CompanyName));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText(path, result);

        }

        private string GetRows(CompanyAdverisment adv )
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= _yaPages.Count; i++)
            {
                sb.AppendLine(GetRow(_yaPages[i - 1],  adv ,i));
            }

            return sb.ToString();
        }

        private string GetRow(YandexPage yaPage, CompanyAdverisment company, int rowNum)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr class='green'>");

            sb.AppendFormat("<td>{0}</td>", rowNum);
            sb.AppendFormat("<td>{0}</td>", yaPage.Query);
            sb.AppendFormat("<td>{0}</td>", "");
            sb.AppendFormat("<td>{0}</td>", yaPage.Frequency);
            sb.AppendFormat("<td>{0}</td>", company.CompanyName);
            sb.AppendLine("</tr>");
            //colspan="3">

            
            
            var adv = company.Advertisments.ContainsKey(yaPage.Query) ? company.Advertisments[yaPage.Query] : null;
            if (adv != null)
            {
                sb.AppendFormat("<tr><td colspan='5' class='bold'>Объявления</td></tr>");
                sb.AppendFormat("<tr><td colspan='4'>{0}</td><td colspan='1'>{1}</td></tr>", adv.TitleLink,
                    adv.TitleLink.Length);
                sb.AppendFormat("<tr><td colspan='4'>{0}</td><td colspan='1'>{1}</td></tr>", adv.TextAdvertisment,
                    adv.TextAdvertisment.Length);
                sb.AppendFormat("<tr><td colspan='5' class='bold'>Быстрые ссылки. С1(30). Всего(66)</td></tr>");

                if (adv.FastLinks.Count == 0)
                {
                    sb.AppendFormat("<tr><td colspan='5'>Быстрых ссылок нет</td></tr>");
                }
                else
                {
                    foreach (var fastLink in adv.FastLinks)
                    {
                        sb.AppendFormat("<tr><td colspan='4'>{0}</td><td colspan='1'>{1}</td></tr>", fastLink,
                            fastLink.Length);
                    }
                    sb.AppendFormat("<tr><td colspan='4'>Всего(66)</td><td colspan='1'>{0}</td></tr>",
                        adv.FastLinks.Sum(_ => _.Length));
                }

                sb.AppendFormat("<tr><td colspan='5' class='bold'>Уточнения. У1(25). Всего(66)</td></tr>");
                if (adv.GraySpecifications.Count == 0)
                {
                    sb.AppendFormat("<tr><td colspan='5'>уточнений нет</td></tr>");
                }
                else
                {
                    foreach (var spec in adv.GraySpecifications)
                    {
                        sb.AppendFormat("<tr><td colspan='4'>{0}</td><td colspan='1'>{1}</td></tr>", spec, spec.Length);
                    }
                    sb.AppendFormat("<tr><td colspan='4'>Всего(66)</td><td colspan='1'>{0}</td></tr>",
                        adv.GraySpecifications.Sum(_ => _.Length));
                }

                sb.AppendFormat("<tr><td colspan='4'>Наличие Яндекс визитки</td><td colspan='1'>{0}</td></tr>",
                    adv.YandexBuisenessCard ? "Да" : "Нет");

                sb.AppendFormat("<tr><td colspan='4'>Наличие отображаемой ссылки</td><td colspan='1'>{0}</td></tr>",
                    adv.GreenUrl ? "Да(" + adv.TitleUrl + ")" : "Нет");
            }
            else
            {
                sb.AppendFormat("<tr><td colspan='5' class='bold'>Предложений нет</td></tr>");
            }
            return sb.ToString();
        }

        private string GetHeader()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendLine("<td class='bold'>№пп</td>");
            sb.AppendLine("<td class='bold'>Ключевой запрос</td>");
            sb.AppendLine("<td class='bold'>Регион</td>");
            sb.AppendLine("<td class='bold'>Частотность</td>");
            sb.AppendLine("<td class='bold'>Сайт ссылка</td>");

            sb.AppendLine("</tr>");

            return sb.ToString();
        }
    }
}