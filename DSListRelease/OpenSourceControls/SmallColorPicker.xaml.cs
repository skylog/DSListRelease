using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace DSList.OpenSourceControls
{
    /// <summary>
    /// Логика взаимодействия для SmallColorPicker.xaml
    /// </summary>
    public partial class SmallColorPicker : UserControl
    {
        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(SmallColorPicker));
        //internal ComboBox Picker;
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(SmallColorPicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(SmallColorPicker.OnSelectedBrushChanged)));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(SmallColorPicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(SmallColorPicker.OnSelectedColorChanged)));


        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add
            {
                base.AddHandler(ColorChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(ColorChangedEvent, value);
            }
        }

        public SmallColorPicker()
        {
            this.InitializeComponent();
            this.InitializeColors();
        }


        private void AddColor(Color c)
        {
            
            this.Picker.Items.Add(c);
        }

        public void InitializeColors()
        {
            this.Picker.Items.Clear();
            this.AddColor(Colors.Black);
            this.AddColor(Colors.Brown);
            this.AddColor(Colors.Maroon);
            this.AddColor(Colors.Red);
            this.AddColor(Colors.IndianRed);
            this.AddColor(Colors.Goldenrod);
            this.AddColor(Colors.YellowGreen);
            this.AddColor(Colors.Green);
            this.AddColor(Colors.DodgerBlue);
            this.AddColor(Colors.Blue);
            this.AddColor(Colors.Navy);
            this.AddColor(Colors.DarkSlateBlue);
            this.AddColor(Colors.Fuchsia);
            this.AddColor(Colors.DarkMagenta);
        }

        
        protected virtual void OnColorChanged(Color oldValue, Color newValue)
        {
            RoutedPropertyChangedEventArgs<Color> e = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue)
            {
                RoutedEvent = ColorChangedEvent
            };
            base.RaiseEvent(e);
        }

        private static void OnSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((obj != null) && (args.NewValue != null))
            {
                SmallColorPicker picker = (SmallColorPicker)obj;
                SolidColorBrush newValue = (SolidColorBrush)args.NewValue;
                if (picker.SelectedColor != newValue.Color)
                {
                    picker.SelectedColor = newValue.Color;
                }
            }
        }

        private static void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SmallColorPicker picker = obj as SmallColorPicker;
            Color newValue = (Color)args.NewValue;
            Color oldValue = (Color)args.OldValue;
            if (newValue != oldValue)
            {
                if (((picker.Picker.SelectedValue == null) || (((Color)picker.Picker.SelectedValue) != newValue)) && !picker.Picker.Items.Contains(newValue))
                {
                    picker.Picker.Items.Add(newValue);
                }
                picker.SelectedBrush = new SolidColorBrush(newValue);
                picker.OnColorChanged(oldValue, newValue);
            }
        }



        public Brush SelectedBrush
        {
            get
            {
                return (Brush)base.GetValue(SelectedBrushProperty);
            }
            set
            {
                base.SetValue(SelectedBrushProperty, value);
            }
        }

        public Color SelectedColor
        {
            get
            {
                return (Color)base.GetValue(SelectedColorProperty);
            }
            set
            {
                base.SetValue(SelectedColorProperty, value);
            }
        }
    }
}
