using System.Text;
using WebAuto.Resources;

namespace WebAuto
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // System.NotSupportedException: 'No data is available for encoding 932. For information on defining a custom encoding, see the documentation for the Encoding.RegisterProvider method.'
            // エラーが出るので、コードページのエンコードプロバイダーへの登録を行う
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            CookieSessionStore cookieSessionStore = CookieSessionStore.Instance;
            _ = cookieSessionStore.Load();
            mainForm = new FormBrowser();
            Application.Run(mainForm);
            _ = cookieSessionStore.Save();
        }
        private static Form? mainForm;
        public static Form? GetForm()
        {
            return mainForm;
        }
        public static void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (null == mainForm)
            {
                return;
            }
            NotifyWindow.EnmType enmType = NotifyWindow.EnmType.Info;
            switch (icon)
            {
                case ToolTipIcon.Info:
                    enmType = NotifyWindow.EnmType.Info;
                    break;
                case ToolTipIcon.Warning:
                    enmType = NotifyWindow.EnmType.Warning;
                    break;
                case ToolTipIcon.Error:
                    enmType = NotifyWindow.EnmType.Error;
                    break;
                case ToolTipIcon.None:
                    enmType = NotifyWindow.EnmType.Success;
                    break;
            }
            NotifyWindow frm = new();
            frm.ShowAlert(mainForm, title, text, enmType);
        }
    }
}