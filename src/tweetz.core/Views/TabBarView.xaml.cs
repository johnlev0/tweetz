﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using tweetz.core.Commands;
using tweetz.core.ViewModels;

namespace tweetz.core.Views
{
    public partial class TabBarView : UserControl
    {
        public TabBarView()
        {
            InitializeComponent();
        }

        private TabBarControlViewModel ViewModel => (TabBarControlViewModel)DataContext;

        private void OnIsVisibleChanged(object _, DependencyPropertyChangedEventArgs __)
        {
            if (Visibility == Visibility.Visible)
            {
                ViewModel.ShowComposeControl = false;
                TabControl.SelectedIndex = 0;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // there's no uniform size option in tabcontrol.
            var width = (e.NewSize.Width / TabControl.Items.Count) - 1.25;
            ViewModel.TabWidth = width;
        }

        /// <summary>
        /// Hocus pocus, try to set the focus when switching tabs so page up/dn, home/end
        /// keyboard shortcuts for scrolling work.
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = TabControl.Items[TabControl.SelectedIndex] as HeaderedContentControl;
            var timeline = item?.Content as TimelineView;

            if (timeline?.FindName("ItemsControl") is ItemsControl itemsControl
                && VisualTreeHelper.GetChildrenCount(itemsControl) > 0
                && VisualTreeHelper.GetChild(itemsControl, 0) is ScrollViewer scrollViewer)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => scrollViewer.Focus()));
            }
        }

        private void TabItemHeaderWithIndicators_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = TabControl.Items[TabControl.SelectedIndex] as HeaderedContentControl;
            var timeline = item?.Content as TimelineView;
            ScrollToHomeCommand.Command.Execute(timeline, this);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            e.Effects =
                e.Data.GetDataPresent(DataFormats.Dib) ||
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent(DataFormats.StringFormat)
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
        }
    }
}