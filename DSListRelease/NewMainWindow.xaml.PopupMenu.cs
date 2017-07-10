using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Data;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;

namespace DSList
{
    public partial class NewMainWindow
    {
        #region Поля

        private ListViewItem NextPlacementTarget;

        internal Popup PopupMenu;
        internal TextBlock PopupNBName;
        internal Border PopupBorder;
        internal ToolBar PopupToolbar;

        private DispatcherTimer OpenPopupTimer = new DispatcherTimer();
        private DispatcherTimer ClosePopupTimer = new DispatcherTimer();

        #endregion

        #region Методы

        private void LoadPopup()
        {

            NextPlacementTarget = new ListViewItem();
            this.OpenPopupTimer.Tick += new EventHandler(this.OpenTimer_Tick);
            this.ClosePopupTimer.Tick += new EventHandler(this.CloseTimer_Tick);

            #region Формирование PopupMenu
            PopupMenu = new Popup();
            PopupNBName = new TextBlock();
            PopupBorder = new Border();
            PopupToolbar = new ToolBar();

            StackPanel sp = new StackPanel();

            PopupNBName.Foreground = System.Windows.Media.Brushes.Aqua;
            PopupNBName.Margin = new Thickness(5, 0, 5, 0);
            sp.Children.Add(PopupNBName);

            PopupToolbar.Margin = new Thickness(-1, 0, -10, 0);
            sp.Children.Add(PopupToolbar);

            PopupBorder.Child = sp;
            PopupBorder.BorderThickness = new Thickness(1);
            PopupBorder.Background = System.Windows.Media.Brushes.DarkBlue;
            PopupMenu.Child = PopupBorder;
            PopupMenu.StaysOpen = true;
            PopupMenu.Placement = PlacementMode.Mouse;
            PopupMenu.VerticalOffset = -83;
            PopupMenu.PopupAnimation = PopupAnimation.Fade;
            PopupMenu.AllowsTransparency = false;

            this.PopupMenu.Opened += new EventHandler(this.PopupMenu_Opened);

            this.PopupBorder.MouseEnter += new System.Windows.Input.MouseEventHandler(this.IPPopupMenu_MouseEnter);
            this.PopupBorder.MouseLeave += new System.Windows.Input.MouseEventHandler(this.IPPopupMenu_MouseLeave);

            this.PopupToolbar.Loaded += new RoutedEventHandler(this.ToolBar_Loaded);

            #endregion

        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ToolBar templatedParent = sender as System.Windows.Controls.ToolBar;
            FrameworkElement element = templatedParent.Template.FindName("OverflowGrid", templatedParent) as FrameworkElement;
            if (element != null)
            {
                element.Visibility = Visibility.Collapsed;
            }
        }

        private bool PopupAction()
        {
            try
            {
                System.Windows.Controls.ListViewItem placementTarget = this.PopupMenu.PlacementTarget as System.Windows.Controls.ListViewItem;
                if (placementTarget != null)
                {
                    placementTarget.IsSelected = true;
                    this.PopupMenu.IsOpen = false;
                    this.OpenPopupTimer.Stop();
                    this.ClosePopupTimer.Stop();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        //public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        //{
        //    if (depObj != null)
        //    {
        //        foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
        //        {
        //            if (rawChild is DependencyObject)
        //            {
        //                DependencyObject child = (DependencyObject)rawChild;
        //                if (child is T)
        //                {
        //                    yield return (T)child;
        //                }

        //                foreach (T childOfChild in FindLogicalChildren<T>(child))
        //                {
        //                    yield return childOfChild;
        //                }
        //            }
        //        }
        //    }
        //}

        private void PopupMenu_Opened(object sender, EventArgs e)
        {
            //Owner.QuickAccessToolBarMainWindow.Items
            this.PopupToolbar.Items.Clear();
            using (IEnumerator<RibbonButton> enumerator = (from b in FindLogicalChildren<RibbonButton>(RibbonWin)
                                                               //where Owner.RibbonWin.QuickAccessToolBar.Items.Contains(b) /*&& (mainwindow.RibbonWin.QuickAccessToolBar.Items.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)*/
                                                           where RibbonWin.QuickAccessToolBar.Items.Contains(b) /*&& (Owner.RibbonWin.QuickAccessToolBar.Items.Count<RibbonButton>(c => (c.tar == b)) == 0)*/

                                                           select b).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RibbonButton ribbonButton = enumerator.Current;

                    StringReader input = new StringReader(XamlWriter.Save(ribbonButton));
                    RibbonButton newItem = (RibbonButton)XamlReader.Load(XmlReader.Create(input));


                    //newItem.Size = Fluent.RibbonControlSize.Middle;
                    //newItem.LargeImageSource = 
                    if (NewMainWindow.settings.Fields.BigPopupToolbar)
                    {
                        //newItem.LargeImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));
                        //newItem.
                    }

                    newItem.CanAddToQuickAccessToolBarDirectly = false;
                    newItem.Command = ribbonButton.Command;
                    newItem.Click += delegate (object o, RoutedEventArgs a)
                    {
                        if ((ribbonButton.IsEnabled && (ribbonButton.Visibility == Visibility.Visible)) && this.PopupAction())
                        {

                            //ribbonButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                            ribbonButton.RaiseEvent(new RoutedEventArgs(RibbonButton.ClickEvent));
                        }
                    };
                    this.PopupToolbar.Items.Add(newItem);
                }
            }
            //foreach (RibbonMenuItem item in from b in FindLogicalChildren<RibbonMenuItem>(Owner.RibbonWin)
            //                                where Owner.RibbonWin.QuickAccessToolBar.Items.Contains(b) /*&& (mainwindow.RibbonWin.QuickAccessItems.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)*/
            //                                select b)
            //{
            //    RibbonButton button2 = new RibbonButton
            //    {
            //        //SmallImageSource = item.QuickAccessToolBarImageSource,
            //        //Size = Fluent.RibbonControlSize.Middle,
            //        //ToolTip = item.ToolTip,
            //        CanAddToQuickAccessToolBarDirectly = false
            //    };
            //    button2.Click += (o, a) => this.PopupAction();
            //    //button2.ToolTip = item.ToolTip;
            //    button2.Command = item.Command;
            //    button2.CommandParameter = item.CommandParameter;
            //    this.PopupToolbar.Items.Add(button2);
            //}
            if (this.PopupToolbar.Items.Count == 0)
            {
                TextBlock block = new TextBlock
                {
                    Text = "Отсутствуют добаленные команды.\r\nДобавьте команду в панель быстрого доступа.",
                    Margin = new Thickness(3.0)
                };
                this.PopupToolbar.Items.Add(block);
            }
        }

        /// Вариант в TTList
        //private void PopupMenu_Opened(object sender, EventArgs e)
        //{
        //    this.PopupToolbar.Items.Clear();
        //    using (IEnumerator<Fluent.Button> enumerator = (from b in FindLogicalChildren<Fluent.Button>(this.RibbonWin)
        //                                                    where this.RibbonWin.IsInQuickAccessToolBar(b) && (this.RibbonWin.QuickAccessItems.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)
        //                                                    select b).GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            Fluent.Button FluentButton = enumerator.Current;
        //            StringReader input = new StringReader(XamlWriter.Save(FluentButton));
        //            Fluent.Button newItem = (Fluent.Button)XamlReader.Load(XmlReader.Create(input));
        //            newItem.Size = settings.Fields.BigPopupToolbar ? RibbonControlSize.Middle : RibbonControlSize.Small;
        //            newItem.CanAddToQuickAccessToolBar = false;
        //            newItem.Click += delegate (object o, RoutedEventArgs a)
        //            {
        //                if ((FluentButton.IsEnabled && (FluentButton.Visibility == Visibility.Visible)) && this.PopupAction())
        //                {
        //                    FluentButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        //                }
        //            };
        //            this.PopupToolbar.Items.Add(newItem);
        //        }
        //    }
        //    foreach (Fluent.MenuItem item in from b in FindLogicalChildren<Fluent.MenuItem>(this.RibbonWin)
        //                                     where this.RibbonWin.IsInQuickAccessToolBar(b) && (this.RibbonWin.QuickAccessItems.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)
        //                                     select b)
        //    {
        //        Fluent.Button button2 = new Fluent.Button
        //        {
        //            Icon = item.Icon,
        //            Size = settings.Fields.BigPopupToolbar ? RibbonControlSize.Middle : RibbonControlSize.Small,
        //            CanAddToQuickAccessToolBar = false
        //        };
        //        button2.Click += (o, a) => this.PopupAction();
        //        button2.ToolTip = item.Header.ToString();
        //        button2.Command = item.Command;
        //        button2.CommandParameter = item.CommandParameter;
        //        this.PopupToolbar.Items.Add(button2);
        //    }
        //    if (this.PopupToolbar.Items.Count == 0)
        //    {
        //        TextBlock block = new TextBlock
        //        {
        //            Text = "Отсутствуют добаленные команды.\r\nДобавьте команду в панель быстрого доступа.",
        //            Margin = new Thickness(3.0)
        //        };
        //        this.PopupToolbar.Items.Add(block);
        //    }
        //}


        /// Рабочий вариант
        //private void PopupMenu_Opened(object sender, EventArgs e)
        //{
        //    this.PopupToolbar.Items.Clear();

        //    //PopupMenu.Child = new Button { Content = "test" };
        //    using (IEnumerator<Fluent.Button> enumerator = (from b in FindLogicalChildren<Fluent.Button>(mainwindow.RibbonWin)
        //                                                    where mainwindow.RibbonWin.QuickAccessToolBar.IsItemItsOwnContainer(b) /*&& (mainwindow.RibbonWin.QuickAccessToolBar.Items.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)*/
        //                                                    select b).GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            Fluent.Button FluentButton = enumerator.Current;
        //            StringReader input = new StringReader(XamlWriter.Save(FluentButton));
        //            Fluent.Button newItem = (Fluent.Button)XamlReader.Load(XmlReader.Create(input));
        //            newItem.Size = Fluent.RibbonControlSize.Middle;
        //            newItem.CanAddToQuickAccessToolBar = false;
        //            newItem.Click += delegate (object o, RoutedEventArgs a)
        //            {
        //                if ((FluentButton.IsEnabled && (FluentButton.Visibility == Visibility.Visible)) && this.PopupAction())
        //                {
        //                    FluentButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        //                }
        //            };
        //            this.PopupToolbar.Items.Add(newItem);
        //        }
        //    }
        //    foreach (Fluent.MenuItem item in from b in FindLogicalChildren<Fluent.MenuItem>(mainwindow.RibbonWin)
        //                                     where mainwindow.RibbonWin.QuickAccessToolBar.IsItemItsOwnContainer(b) /*&& (mainwindow.RibbonWin.QuickAccessItems.Count<QuickAccessMenuItem>(c => (c.Target == b)) == 0)*/
        //                                     select b)
        //    {
        //        Fluent.Button button2 = new Fluent.Button
        //        {
        //            Icon = item.Icon,
        //            Size = Fluent.RibbonControlSize.Middle,
        //            CanAddToQuickAccessToolBar = false
        //        };
        //        button2.Click += (o, a) => this.PopupAction();
        //        button2.ToolTip = item.Header.ToString();
        //        button2.Command = item.Command;
        //        button2.CommandParameter = item.CommandParameter;
        //        this.PopupToolbar.Items.Add(button2);
        //    }
        //    if (this.PopupToolbar.Items.Count == 0)
        //    {
        //        TextBlock block = new TextBlock
        //        {
        //            Text = "Отсутствуют добаленные команды.\r\nДобавьте команду в панель быстрого доступа.",
        //            Margin = new Thickness(3.0)
        //        };
        //        this.PopupToolbar.Items.Add(block);
        //    }
        //}

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (NewMainWindow.settings.Fields.PopupMenuDelayed)
            {
                this.NextPlacementTarget = e.Source/*(sender as ListView).SelectedItem*/ as ListViewItem;
                this.OpenPopupTimerStart();
            }
        }

        private void ListViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            this.OpenPopupTimer.Stop();
            this.ClosePopupTimerStart();
        }

        private void IPPopupMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ClosePopupTimer.Stop();
        }


        private void IPPopupMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ClosePopupTimerStart();
        }

        private void ClosePopupTimerStart()
        {
            this.ClosePopupTimer.Stop();
            this.ClosePopupTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            this.ClosePopupTimer.Start();
        }

        private void OpenPopupTimerStart()
        {
            this.OpenPopupTimer.Stop();
            this.OpenPopupTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            this.OpenPopupTimer.Start();
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            this.PopupMenu.IsOpen = false;
            this.ClosePopupTimer.Stop();
            if (this.OpenPopupTimer.IsEnabled)
            {
                this.OpenPopupTimerStart();
            }
        }

        private void OpenTimer_Tick(object sender, EventArgs e)
        {
            this.PopupMenu.AllowsTransparency = true;
            this.OpenPopupMenu();
        }

        private void OpenPopupMenu()
        {

            this.PopupMenu.PlacementTarget = this.NextPlacementTarget;
            this.PopupNBName.Text = (this.NextPlacementTarget.Content as IP).NBNameOrIP();
            this.PopupMenu.IsOpen = true;
            this.OpenPopupTimer.Stop();
            this.ClosePopupTimer.Stop();
        }

        

        private void ListViewItem_LeftMouseClick(object sender, MouseEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is ListViewItem)
                {
                    IP selIP = ((ListViewItem)dep).Content as IP;

                    if (!NewMainWindow.settings.Fields.PopupMenuDelayed)
                    {
                        if (this.NextPlacementTarget != (dep as ListViewItem))
                        {
                            this.PopupMenu.IsOpen = false;
                        }
                        this.NextPlacementTarget = dep as ListViewItem;
                        this.PopupMenu.AllowsTransparency = false;
                        this.OpenPopupMenu();
                    }
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        //this.AddSelectedIPToMonitoring();
                    }

                }
            }


        }

        #endregion
    }
}
