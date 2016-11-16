using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Reports.Helpers;

namespace FrequencyPageVisitor.Reports
{
    public class RivalReport2
    {
        private readonly List<YandexPage> _yaPages;
        private readonly string _reportDir;
        private readonly string _htmlRootTemplate;
        private readonly string _htmlRowTemplate;
        public List<CompanyAdverisment> Companies { get; set; }

        public RivalReport2(List<YandexPage> yaPages, string reportDir)
        {
             _htmlRootTemplate = File.ReadAllText("HTMLTemplates/ReportRoot.html");
             _htmlRowTemplate = File.ReadAllText("HTMLTemplates/ReportRow.html");

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
            var rows = GetRows(adv);
            
            var dirPath = Path.Combine(_reportDir, "Companies");
            var path = Path.Combine(dirPath, string.Format("{0}-{1}.html", companyNum, adv.CompanyName));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var result = _htmlRootTemplate.Replace("{Rows}", rows);

            File.WriteAllText(path, result);

        }

        private void ReplaceMarker(ref string template, string marker, string value)
        {
            template = template.Replace("{" + marker + "}", value);
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
            var result = _htmlRowTemplate.ToString();
            ReplaceMarker(ref result, "RowNum", rowNum.ToString());
            ReplaceMarker(ref result, "QueryText", yaPage.Query);
            ReplaceMarker(ref result, "Region", "");
            ReplaceMarker(ref result, "Frequency", yaPage.Frequency);
            ReplaceMarker(ref result, "CompanyName", company.CompanyName);
            
            var adv = company.Advertisments.ContainsKey(yaPage.Query) ? company.Advertisments[yaPage.Query] : null;
            if (adv != null)
            {
                ReplaceMarker(ref result, "TitleLink", adv.TitleLink);
                ReplaceMarker(ref result, "TitleLinkLength", adv.TitleLink.Length.ToString());

                ReplaceMarker(ref result, "TextAdvertisment", adv.TextAdvertisment);
                ReplaceMarker(ref result, "TextAdvertismentLength", adv.TextAdvertisment.Length.ToString());

                for (int i = 0; i < 4; i++)
                {
                    var isLinkExists = adv.FastLinks.Count > i;

                    var linknum = i + 1;
                    ReplaceMarker(ref result, "FastLink" + linknum, isLinkExists ? adv.FastLinks[i] : "");
                    ReplaceMarker(ref result, "FastLinkLength" + linknum, isLinkExists ? adv.FastLinks[i].Length.ToString() : "");
                }

                ReplaceMarker(ref result, "FastLinkTotal", adv.FastLinks.Sum(_ => _.Length).ToString());


                var graySpecifications = string.Join("|", adv.GraySpecifications.ToArray());
                var graySpecificationsLength = adv.GraySpecifications.Sum(_ => _.Length);
                ReplaceMarker(ref result, "GraySpecifications", graySpecifications);
                ReplaceMarker(ref result, "GraySpecificationsTotal", graySpecificationsLength.ToString());

                ReplaceMarker(ref result, "YandexBuisenessCard", adv.YandexBuisenessCard ? "Да" : "Нет");
                ReplaceMarker(ref result, "GreenUrl", adv.GreenUrl ? "Да(" + adv.TitleUrl + ")" : "Нет");
                ReplaceMarker(ref result, "IsUtm", adv.IsUtm);
                ReplaceMarker(ref result, "YandexMarket", adv.YandexMarket);

            }
            else
            {
                ReplaceMarker(ref result, "TitleLink", "-");
                ReplaceMarker(ref result, "TitleLinkLength", "-");
                ReplaceMarker(ref result, "TextAdvertisment", "-");
                ReplaceMarker(ref result, "TextAdvertismentLength", "-");

                for (int i = 0; i < 4; i++)
                {
                    var linknum = i + 1;
                    ReplaceMarker(ref result, "FastLink" + linknum, "-");
                    ReplaceMarker(ref result, "FastLinkLength" + linknum, "-");
                }

                ReplaceMarker(ref result, "FastLinkTotal", "-");
                ReplaceMarker(ref result, "YandexBuisenessCard", "-");
                ReplaceMarker(ref result, "GreenUrl", "-");
                ReplaceMarker(ref result, "IsUtm", "-");
                ReplaceMarker(ref result, "YandexMarket", "-");
              }
            return result;
        }
    }
}