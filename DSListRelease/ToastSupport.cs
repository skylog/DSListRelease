using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DSList
{
    internal class ToastSupport
    {
        public static Storyboard GetAnimation(ToasterAnimation animation, ref Grid toaster)
        {
            Storyboard storyboard = new Storyboard();
            SplineDoubleKeyFrame frame = new SplineDoubleKeyFrame();
            switch (animation)
            {
                case ToasterAnimation.FadeIn:
                    {
                        DoubleAnimation element = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.5))
                        {
                            BeginTime = new TimeSpan?(TimeSpan.FromSeconds(0.0))
                        };
                        Storyboard.SetTargetProperty(element, new PropertyPath("(UIElement.Opacity)", new object[0]));
                        storyboard.Children.Add(element);
                        DoubleAnimation animation4 = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(0.5))
                        {
                            BeginTime = new TimeSpan?(TimeSpan.FromSeconds((double)(ToastManager.ToastTime - 1)))
                        };
                        Storyboard.SetTargetProperty(animation4, new PropertyPath("(UIElement.Opacity)", new object[0]));
                        storyboard.Children.Add(animation4);
                        return storyboard;
                    }
                case ToasterAnimation.SlideInFromRight:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(1.0, 0.0);
                        toaster.RenderTransform = new TranslateTransform(300.0, 0.0);
                        DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(300.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(300.0, TimeSpan.FromSeconds((double) ToastManager.ToastTime))
                        }
                        };
                        Storyboard.SetTargetProperty(frames, new PropertyPath("RenderTransform.(TranslateTransform.X)", new object[0]));
                        storyboard.Children.Add(frames);
                        return storyboard;
                    }
                case ToasterAnimation.SlideInFromLeft:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(0.0, 1.0);
                        toaster.RenderTransform = new TranslateTransform(300.0, 0.0);
                        DoubleAnimationUsingKeyFrames frames2 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(-300.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(-300.0, TimeSpan.FromSeconds((double) ToastManager.ToastTime))
                        }
                        };
                        Storyboard.SetTargetProperty(frames2, new PropertyPath("RenderTransform.(TranslateTransform.X)", new object[0]));
                        storyboard.Children.Add(frames2);
                        return storyboard;
                    }
                case ToasterAnimation.SlideInFromTop:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(0.0, 1.0);
                        toaster.RenderTransform = new TranslateTransform(0.0, 300.0);
                        DoubleAnimationUsingKeyFrames frames3 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(-300.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds((double) ((ToastManager.ToastTime / 2) - 1))),
                            new SplineDoubleKeyFrame(-300.0, TimeSpan.FromSeconds((double) ToastManager.ToastTime))
                        }
                        };
                        Storyboard.SetTargetProperty(frames3, new PropertyPath("RenderTransform.(TranslateTransform.Y)", new object[0]));
                        storyboard.Children.Add(frames3);
                        return storyboard;
                    }
                case ToasterAnimation.SlideInFromBottom:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(0.0, 1.0);
                        toaster.RenderTransform = new TranslateTransform(0.0, -300.0);
                        DoubleAnimationUsingKeyFrames frames4 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(300.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds((double) ((ToastManager.ToastTime / 2) - 1))),
                            new SplineDoubleKeyFrame(300.0, TimeSpan.FromSeconds((double) ToastManager.ToastTime))
                        }
                        };
                        Storyboard.SetTargetProperty(frames4, new PropertyPath("RenderTransform.(TranslateTransform.Y)", new object[0]));
                        storyboard.Children.Add(frames4);
                        return storyboard;
                    }
                case ToasterAnimation.GrowFromRight:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(1.0, 0.0);
                        DoubleAnimationUsingKeyFrames frames5 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime + 0.5))
                        }
                        };
                        Storyboard.SetTargetProperty(frames5, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)", new object[0]));
                        storyboard.Children.Add(frames5);
                        return storyboard;
                    }
                case ToasterAnimation.GrowFromLeft:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(0.0, 1.0);
                        DoubleAnimationUsingKeyFrames frames6 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime + 0.5))
                        }
                        };
                        Storyboard.SetTargetProperty(frames6, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)", new object[0]));
                        storyboard.Children.Add(frames6);
                        return storyboard;
                    }
                case ToasterAnimation.GrowFromTop:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(1.0, 0.0);
                        DoubleAnimationUsingKeyFrames frames7 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime + 0.5))
                        }
                        };
                        Storyboard.SetTargetProperty(frames7, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)", new object[0]));
                        storyboard.Children.Add(frames7);
                        return storyboard;
                    }
                case ToasterAnimation.GrowFromBottom:
                    {
                        toaster.RenderTransformOrigin = new System.Windows.Point(0.0, 1.0);
                        DoubleAnimationUsingKeyFrames frames8 = new DoubleAnimationUsingKeyFrames
                        {
                            KeyFrames = {
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(0.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(1.0)),
                            new SplineDoubleKeyFrame(1.0, TimeSpan.FromSeconds(ToastManager.ToastTime - 0.5)),
                            new SplineDoubleKeyFrame(0.0, TimeSpan.FromSeconds(ToastManager.ToastTime + 0.5))
                        }
                        };
                        Storyboard.SetTargetProperty(frames8, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)", new object[0]));
                        storyboard.Children.Add(frames8);
                        return storyboard;
                    }
            }
            return storyboard;
        }

        public static Dictionary<string, double> GetTopandLeft(ToasterPosition positionSelection, Window windowRef, double margin)
        {
            System.Windows.Point point;
            System.Windows.Point point2;
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            Window mainWindow = System.Windows.Application.Current.MainWindow;
            Screen screen = Screen.FromHandle(new WindowInteropHelper(mainWindow).Handle);
            ToasterPosition primaryScreenBottomRight = positionSelection;
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                switch (positionSelection)
                {
                    case ToasterPosition.ApplicationBottomRight:
                        primaryScreenBottomRight = ToasterPosition.PrimaryScreenBottomRight;
                        break;

                    case ToasterPosition.ApplicationTopRight:
                        primaryScreenBottomRight = ToasterPosition.PrimaryScreenTopRight;
                        break;

                    case ToasterPosition.ApplicationBottomLeft:
                        primaryScreenBottomRight = ToasterPosition.PrimaryScreenBottomLeft;
                        break;

                    case ToasterPosition.ApplicationTopLeft:
                        primaryScreenBottomRight = ToasterPosition.PrimaryScreenTopLeft;
                        break;
                }
            }
            Matrix matrix = getTransform(windowRef);
            dictionary.Add("Left", 0.0);
            dictionary.Add("Top", 0.0);
            switch (primaryScreenBottomRight)
            {
                case ToasterPosition.PrimaryScreenBottomRight:
                    workingArea = screen.WorkingArea;
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    dictionary["Left"] = (point.X - windowRef.ActualWidth) - margin;
                    dictionary["Top"] = (point.Y - windowRef.ActualHeight) - margin;
                    return dictionary;

                case ToasterPosition.PrimaryScreenTopRight:
                    workingArea = screen.WorkingArea;
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    point2 = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Top));
                    dictionary["Left"] = (point.X - windowRef.ActualWidth) - margin;
                    dictionary["Top"] = point2.Y + margin;
                    return dictionary;

                case ToasterPosition.PrimaryScreenBottomLeft:
                    workingArea = screen.WorkingArea;
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Left, (double)workingArea.Bottom));
                    dictionary["Left"] = point.X + margin;
                    dictionary["Top"] = (point.Y - windowRef.ActualHeight) - margin;
                    return dictionary;

                case ToasterPosition.PrimaryScreenTopLeft:
                    workingArea = screen.WorkingArea;
                    point2 = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Top));
                    dictionary["Left"] = margin;
                    dictionary["Top"] = point2.Y + margin;
                    return dictionary;

                case ToasterPosition.ApplicationBottomRight:
                    workingArea = new Rectangle((int)mainWindow.Left, (int)mainWindow.Top, (int)mainWindow.ActualWidth, (int)mainWindow.ActualHeight);
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    dictionary["Left"] = (point.X - windowRef.ActualWidth) - 5.0;
                    dictionary["Top"] = point.Y - windowRef.ActualHeight;
                    return dictionary;

                case ToasterPosition.ApplicationTopRight:
                    workingArea = new Rectangle((int)mainWindow.Left, (int)mainWindow.Top, (int)mainWindow.ActualWidth, (int)mainWindow.ActualHeight);
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    point2 = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Top));
                    dictionary["Left"] = (point.X - windowRef.ActualWidth) - 5.0;
                    dictionary["Top"] = point2.Y + 25.0;
                    return dictionary;

                case ToasterPosition.ApplicationBottomLeft:
                    workingArea = new Rectangle((int)mainWindow.Left, (int)mainWindow.Top, (int)mainWindow.ActualWidth, (int)mainWindow.ActualHeight);
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    dictionary["Left"] = mainWindow.Left + 5.0;
                    dictionary["Top"] = point.Y - windowRef.ActualHeight;
                    return dictionary;

                case ToasterPosition.ApplicationTopLeft:
                    workingArea = new Rectangle((int)mainWindow.Left, (int)mainWindow.Top, (int)mainWindow.ActualWidth, (int)mainWindow.ActualHeight);
                    point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
                    point2 = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Top));
                    dictionary["Left"] = mainWindow.Left;
                    dictionary["Top"] = point2.Y + 25.0;
                    return dictionary;
            }
            workingArea = Screen.PrimaryScreen.WorkingArea;
            point = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
            point2 = matrix.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Top));
            dictionary["Left"] = (point.X - windowRef.ActualWidth) - 100.0;
            dictionary["Top"] = point.Y - windowRef.ActualHeight;
            return dictionary;
        }

        private static Matrix getTransform(Window windowRef)
        {
            PresentationSource source = PresentationSource.FromVisual(windowRef);
            if ((source != null) && (source.CompositionTarget != null))
            {
                return source.CompositionTarget.TransformFromDevice;
            }
            return new Matrix();
        }
    }
}
