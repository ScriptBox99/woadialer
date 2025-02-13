﻿using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#nullable enable

namespace Dialer.UI.Controls
{
    public sealed partial class TitlebarControl : UserControl
    {
        private static string AppName = Package.Current.DisplayName;

        public event EventHandler<RoutedEventArgs> BackButtonClick;

        private string AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ?
            ApplicationView.GetForCurrentView().Title + " - " : "") +
            AppName;

        public TitlebarControl()
        {
            InitializeComponent();
            Loaded += TitleBarControl_Loaded;

            CoreApplicationView? coreApplicationView = CoreApplication.GetCurrentView();
            if (coreApplicationView.TitleBar != null)
            {
                Window.Current.SetTitleBar(TitlebarCanvas);

                SetVisibility(coreApplicationView.TitleBar.IsVisible);
                coreApplicationView.TitleBar.ExtendViewIntoTitleBar = true;
                Height = coreApplicationView.TitleBar.Height != 0 ? coreApplicationView.TitleBar.Height : 0;

                Thickness margin = CustomTitleBar.Margin;
                margin.Right = coreApplicationView.TitleBar.SystemOverlayRightInset;
                CustomTitleBar.Margin = margin;

                coreApplicationView.TitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
                coreApplicationView.TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            }

#if DEBUG
            if (Debugger.IsAttached)
            {
                RedIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                AttachDebuggerButton.Visibility = Visibility.Visible;
            }

            if (Application.Current.DebugSettings.FailFastOnErrors)
            {
                YellowIndicator.Visibility = Visibility.Visible;
            }
#endif

            RefreshColor();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            BackButtonClick?.Invoke(this, e);
        }

        public void RefreshColor()
        {
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            if (titlebar != null)
            {
                SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
                Color transparentColor = transparentColorBrush.Color;

                titlebar.BackgroundColor = transparentColor;
                titlebar.ButtonBackgroundColor = transparentColor;
                titlebar.InactiveBackgroundColor = transparentColor;
                titlebar.ButtonInactiveBackgroundColor = transparentColor;

                SolidColorBrush foregroundThemeBrush = (SolidColorBrush)Resources["ApplicationForegroundThemeBrush"];

                titlebar.ButtonForegroundColor = foregroundThemeBrush.Color;
                titlebar.ForegroundColor = foregroundThemeBrush.Color;

                Color color = foregroundThemeBrush.Color;
                color.A = 16;
                titlebar.InactiveForegroundColor = color;
                titlebar.ButtonInactiveForegroundColor = color;

                Color hovercolor = foregroundThemeBrush.Color;
                hovercolor.A = 32;
                titlebar.ButtonHoverBackgroundColor = hovercolor;
                titlebar.ButtonHoverForegroundColor = foregroundThemeBrush.Color;
                hovercolor.A = 64;
                titlebar.ButtonPressedBackgroundColor = hovercolor;
                titlebar.ButtonPressedForegroundColor = foregroundThemeBrush.Color;
            }
        }

        private async void TitleBarControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AppName = (await Package.Current.GetAppListEntriesAsync())[0].DisplayInfo.DisplayName;
            }
            catch { }

            AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ?
                ApplicationView.GetForCurrentView().Title + " - " : "") +
                AppName;
#if DEBUG
            AppTitle += " [DEBUG]";
#endif
            WindowTitle.Text = AppTitle;
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Height = sender.Height != 0 ? sender.Height : 0;
            Thickness margin = CustomTitleBar.Margin;
            margin.Right = sender.SystemOverlayRightInset;
            CustomTitleBar.Margin = margin;
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            SetVisibility(sender.IsVisible);
        }

        private void SetVisibility(bool IsVisible)
        {
            if (IsVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private AppViewBackButtonVisibility _BackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

        public AppViewBackButtonVisibility BackButtonVisibility
        {
            get => _BackButtonVisibility;
            set
            {
                _BackButtonVisibility = value;
                switch (_BackButtonVisibility)
                {
                    case AppViewBackButtonVisibility.Visible:
                        BackButton.Visibility = Visibility.Visible;
                        break;

                    case AppViewBackButtonVisibility.Collapsed:
                        BackButton.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        public void UpdateTitle()
        {
            AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ?
                ApplicationView.GetForCurrentView().Title + " - " : "") +
                AppName;
#if DEBUG
            AppTitle += " [DEBUG]";
#endif
            WindowTitle.Text = AppTitle;
        }

        public string Title
        {
            get => ApplicationView.GetForCurrentView().Title;
            set
            {
                ApplicationView.GetForCurrentView().Title = value;
                AppTitle = (!string.IsNullOrEmpty(value) ?
                    value + " - " : "") +
                    AppName;

#if DEBUG
                AppTitle += " [DEBUG]";
#endif
                WindowTitle.Text = AppTitle;
            }
        }

        private void AttachDebuggerButton_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Launch();

            if (Debugger.IsAttached)
            {
                RedIndicator.Visibility = Visibility.Visible;

                AttachDebuggerButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AttachDebuggerButton.Visibility = Visibility.Visible;
            }

            if (Application.Current.DebugSettings.FailFastOnErrors)
            {
                YellowIndicator.Visibility = Visibility.Visible;
            }
        }
    }
}