using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XBindLecture.ViewModels;

#if WINDOWS_APP || WINDOWS_UWP
using Windows.System;
using Windows.UI.Core;
#endif

namespace XBindLecture.Views
{
	public sealed class NavigationHelper
	{
		private Page Page { get; }
		private Frame Frame { get { return this.Page.Frame; } }

		public NavigationHelper(Page page)
		{
			this.Page = page;
			this.Page.Loaded += this.OnLoaded;
			this.Page.Unloaded += this.OnUnloaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
#if WINDOWS_PHONE_APP
			Windows.Phone.UI.Input.HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;
#else
			Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += this.OnCoreDispatcherAcceleratorKeyActivated;
			Window.Current.CoreWindow.PointerPressed += this.OnCoreWindowPointerPressed;
#endif

#if WINDOWS_UWP
			SystemNavigationManager.GetForCurrentView().BackRequested += this.OnBackRequested;
#endif
		}

#if WINDOWS_UWP
		public void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.CanGoBack
				? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}
#endif

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
#if WINDOWS_PHONE_APP
			Windows.Phone.UI.Input.HardwareButtons.BackPressed -= OnHardwareButtonsBackPressed;
#else
			Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= this.OnCoreDispatcherAcceleratorKeyActivated;
			Window.Current.CoreWindow.PointerPressed -= this.OnCoreWindowPointerPressed;
#endif
#if WINDOWS_UWP
			SystemNavigationManager.GetForCurrentView().BackRequested -= this.OnBackRequested;
#endif
		}

		#region ナビゲーション サポート

		public RelayCommand GoHomeCommand
		{
			get { return this._GoHomeCommand ?? (this._GoHomeCommand = new RelayCommand(() => this.GoHome(), () => this.CanGoHome)); }
		}
		private RelayCommand _GoHomeCommand = null;

		public RelayCommand GoBackCommand
		{
			get { return this._GoBackCommand ?? (this._GoBackCommand = new RelayCommand(() => this.GoBack(), () => this.CanGoBack)); }
		}
		private RelayCommand _GoBackCommand = null;

		public RelayCommand GoForwardCommand
		{
			get { return this._GoForwardCommand ?? (this._GoForwardCommand = new RelayCommand(() => this.GoForward(), () => this.CanGoForward)); }
		}
		private RelayCommand _GoForwardCommand = null;

		public bool CanGoHome => this.Frame?.CanGoBack ?? false;
		public bool CanGoBack => this.Frame?.CanGoBack ?? false;
		public bool CanGoForward => this.Frame?.CanGoForward ?? false;

		public bool GoHome()
		{
			var depth = this.Frame.BackStackDepth;
			if (depth-- != 0)
			{
				do
				{
					this.Frame.GoBack();
				} while (depth-- != 0);
			}
			return false;
		}

		public bool GoBack()
		{
			if (!this.CanGoBack) return false;

			this.Frame.GoBack();
			return true;
		}

		public bool GoForward()
		{
			if (this.CanGoForward) return false;

			this.Frame.GoForward();
			return true;
		}

#if WINDOWS_PHONE_APP
		private void OnHardwareButtonsBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
		{
			if (this.GoBackCommand.CanExecute(null))
			{
				e.Handled = true;
				this.GoBackCommand.Execute(null);
			}
		}
#else
		private void OnCoreDispatcherAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
		{
			var virtualKey = e.VirtualKey;
			if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown || e.EventType == CoreAcceleratorKeyEventType.KeyDown)
				&& (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right
				|| virtualKey == VirtualKey.GoForward || virtualKey == VirtualKey.GoForward || virtualKey == VirtualKey.GoHome))
			{
				var coreWindow = Window.Current.CoreWindow;
				var downState = CoreVirtualKeyStates.Down;
				bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
				bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
				bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
				bool noModifiers = !menuKey && !controlKey && !shiftKey;
				bool onlyAlt = menuKey && !controlKey && !shiftKey;

				if (virtualKey == VirtualKey.GoBack && noModifiers || virtualKey == VirtualKey.Left && onlyAlt)
				{
					e.Handled = this.GoBack();
				}
				else if (virtualKey == VirtualKey.GoForward && noModifiers || virtualKey == VirtualKey.Right && onlyAlt)
				{
					e.Handled = this.GoForward();
				}
				else if (virtualKey == VirtualKey.GoHome && noModifiers)
				{
					e.Handled = this.GoForward();
				}
			}
		}

		private void OnCoreWindowPointerPressed(CoreWindow sender, PointerEventArgs e)
		{
			var properties = e.CurrentPoint.Properties;
			bool backPressed = properties.IsXButton1Pressed;
			bool forwardPressed = properties.IsXButton2Pressed;
			if (backPressed ^ forwardPressed)
			{
				if (backPressed) e.Handled = this.GoBack();
				else e.Handled = this.GoForward();
			}
		}
#endif
#if WINDOWS_UWP

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			e.Handled = this.GoBack();
		}

#endif
		#endregion
	}
}