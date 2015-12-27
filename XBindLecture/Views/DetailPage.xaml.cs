using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XBindLecture.Storages;
using XBindLecture.ViewModels;

namespace XBindLecture.Views
{
	public sealed partial class DetailPage : Page, INotifyPropertyChanged
	{
		public NavigationHelper NavigationHelper { get; }
		public PersonViewModel Person { get; set; }

		public DetailPage()
		{
			this.NavigationHelper = new NavigationHelper(this);
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.NavigationHelper.OnNavigatedTo(e);

			var param = (string)e.Parameter;
			this.Person = new PersonViewModel(PersonStorage.GetPerson(param));
			this.RaisePropertyChanged(nameof(this.Person));
			
			var depth = this.Frame.BackStackDepth;
			System.Diagnostics.Debug.Assert(depth != 0);
			System.Diagnostics.Debug.Assert(this.Frame.BackStack[depth - 1].SourcePageType == typeof(MainPage));
			var mainPageEntry = this.Frame.BackStack[depth - 1];
			this.Frame.BackStack[depth - 1] = new PageStackEntry(
				mainPageEntry.SourcePageType,
				e.Parameter,
				mainPageEntry.NavigationTransitionInfo);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
			=> this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		#region auto navigation

		private bool ShouldGoback(double width) => width >= 548.0;
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this.ShouldGoback(Window.Current.Bounds.Width))
			{
				this.GoBackWithTransition(true);
			}
		}

		private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
		{
			if (this.ShouldGoback(e.NewSize.Width))
			{
				this.GoBackWithTransition(false);
			}
		}

		private void GoBackWithTransition(bool useTransition)
		{
			this.NavigationCacheMode = NavigationCacheMode.Disabled;
			if (useTransition)
			{
				this.Frame.GoBack(new EntranceNavigationTransitionInfo());
			}
			else
			{
				this.Frame.GoBack(new SuppressNavigationTransitionInfo());
			}
		}

		#endregion
	}
}