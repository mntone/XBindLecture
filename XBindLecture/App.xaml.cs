using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XBindLecture.Views;

namespace XBindLecture
{
	public sealed partial class App : Application
	{
		public App()
		{
			this.InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) this.DebugSettings.EnableFrameRateCounter = true;
#endif
			var currentAppView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
			currentAppView.SetPreferredMinSize(new Windows.Foundation.Size(200, 180)); // minimum

			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
			{
				rootFrame = new Frame();
				rootFrame.CacheSize = 1;
				Window.Current.Content = rootFrame;
			}
			if (rootFrame.Content == null)
			{
				rootFrame.Navigate(typeof(MainPage), e.Arguments);
			}
			Window.Current.Activate();
		}
	}
}