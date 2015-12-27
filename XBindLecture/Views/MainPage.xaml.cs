using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XBindLecture.Storages;
using XBindLecture.ViewModels;

namespace XBindLecture.Views
{
	public sealed partial class MainPage : Page, INotifyPropertyChanged
	{
		public NavigationHelper NavigationHelper { get; }
		public PersonViewModel[] Persons { get; }

		public object SelectedPerson
		{
			get { return this._SelectedPerson; }
			set
			{
				if (object.ReferenceEquals(this._SelectedPerson, value)) return;

				this._SelectedPerson = (PersonViewModel)value;
				this.RaisePropertyChanged(nameof(this.SelectedPerson));
			}
		}
		private PersonViewModel _SelectedPerson = null;

		public MainPage()
		{
			this.NavigationHelper = new NavigationHelper(this);
			this.Persons = PersonStorage.GetPersons().Select(p => new PersonViewModel(p)).ToArray();

			this.InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
			=> this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.NavigationHelper.OnNavigatedTo(e);

			var param = (string)e.Parameter;
			if (!string.IsNullOrEmpty(param))
			{
				this.SelectedPerson = this.Persons.Where(p => p.Name == param).Single();
			}

			this.UpdateVisualState(this.AdaptiveStates.CurrentState);
			this.DisableContentTransitions();
		}

		private void OnItemClick(object sender, ItemClickEventArgs e)
		{
			this.SelectedPerson = e.ClickedItem;
			if (this.AdaptiveStates.CurrentState == this.NarrowState)
			{
				this.Frame.Navigate(typeof(DetailPage), this._SelectedPerson.Name.ToString(), new SuppressNavigationTransitionInfo());
			}
			else
			{
				this.EnableContentTransitions();
			}
		}

		#region Visual state support

		private void OnAdaptiveStatesCurrentStateChanged(object sender, Windows.UI.Xaml.VisualStateChangedEventArgs e)
		{
			this.UpdateVisualState(e.NewState, e.OldState);
		}

		private void UpdateVisualState(VisualState newState, VisualState oldState = null)
		{
			var isNarrow = newState == this.NarrowState;
			if (isNarrow && oldState != null && oldState != this.NarrowState && this._SelectedPerson != null)
			{
				this.Frame.Navigate(typeof(DetailPage), this._SelectedPerson.Name.ToString(), new SuppressNavigationTransitionInfo());
			}

			EntranceNavigationTransitionInfo.SetIsTargetElement(this.PersonsListView, isNarrow);
			if (this.DetailView != null) EntranceNavigationTransitionInfo.SetIsTargetElement(this.DetailView, !isNarrow);
		}

		private void EnableContentTransitions()
		{
			this.DetailView.ContentTransitions.Clear();
			this.DetailView.ContentTransitions.Add(new EntranceThemeTransition());
		}

		private void DisableContentTransitions()
		{
			if (this.DetailView != null) this.DetailView.ContentTransitions.Clear();
		}

		#endregion
	}
}