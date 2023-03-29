using Microsoft.UI.Xaml;

namespace ItemChecker
{
    public partial class App : Application
    {
        Window m_window;
        internal static Window MainWindow;

        public App()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }
    }
}
