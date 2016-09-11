using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using PageVisitor.Settings;
using PageVisitor.Utils;
using PageVisitor.WebDriverWrapper;

namespace PageVisitor.Visitor
{
    public class VisitorManager
    {
        private PageVisitor _visitor;
        private IWebDriver _driver;
        private VisitorSettings _settings;

        private void ItitPageVisitor()
        {
            _driver = WebDriverProvider.GetWebDriver();
            _visitor = new PageVisitor(_driver);
        }

        private void ItitSettings()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = (VisitorSettings) cfg.Sections["VisitorSettings"];
        }

        public void Start()
        {
            ItitSettings();
            ItitPageVisitor();
            try
            {
                var url = "https://yandex.ru/search/?text=" + _settings.Request;
                _visitor.NavigateToUrl(url);
                var elementsWithOurAdvertisement =
                    _visitor.GetElementsWithOurAdvertisement(_settings.Words, _settings.OurSite);
                if (elementsWithOurAdvertisement.Count != 0)
                {
                    elementsWithOurAdvertisement
                        .ForEach(_visitor.HightlightTheElement);

                    _visitor.MakeScreenshot();

                    var msg = String.Format("Объявление найдено. Скриншот сохранен в папку ScreenShots \n\n\n");
                    Logger.WriteGreen(msg);
                }
                else
                {
                    var msg = String.Format("Объявление не найдено.");
                    Logger.WriteRed(msg);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
                _visitor.Close();
                ItitPageVisitor();
            }
        }

        public void Close()
        {
            _visitor.Close();
        }
    }
}