using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace FrequencyPageVisitor.WebDriverWrapper
{
    public class WebDriverProvider
    {
        public static IWebDriver GetFFWebDriver()
        {


            var profile = new FirefoxProfile();
            profile.SetPreference("browser.download.folderList", 2);

            //profile.SetPreference("browser.download.dir", SaveDirectoryPath);
            //"C:\\Users\\Administrator\\workspace\\autoTest\\screenshot\\" + screenshotFolderName + "\\" + screenshotFileName + "\\");
            profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk",
                "application/msword, application/csv, application/ris, text/csv, image/png, application/pdf, text/html, text/plain, application/zip, application/x-zip, application/x-zip-compressed, application/download, application/octet-stream");
            //profile.SetPreference("browser.download.manager.showWhenStarting", false);
            profile.SetPreference("browser.download.manager.focusWhenStarting", false);
            profile.SetPreference("browser.download.useDownloadDir", true);
            profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            profile.SetPreference("browser.download.manager.closeWhenDone", true);
            profile.SetPreference("browser.download.manager.showAlertOnComplete", false);
            profile.SetPreference("browser.download.manager.useWindow", false);
            profile.SetPreference("services.sync.prefs.sync.browser.download.manager.showWhenStarting", false);
            profile.SetPreference("pdfjs.disabled", true);


            IWebDriver driver = new FirefoxDriver(profile);
            driver.Manage().Window.Maximize();

            return driver;
        }
        public static IWebDriver GetWebDriver()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
