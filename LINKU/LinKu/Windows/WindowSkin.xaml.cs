using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LinKu.Windows
{
    /// <summary>
    /// WindowSpeed.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSkin : UserControl
    {
        public WindowSkin()
        {
            InitializeComponent();
        }

        public void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }
        private object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        private void X_Click(object sender, RoutedEventArgs e)
        {
            App.app.Init();
            DoEvents();
           // Application.Current.MainWindow.ApplyTemplate();
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        private void FontBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ColorPicker.SelectedColor = ((SolidColorBrush)FontBack.Background).Color;
            ColorPopup.PlacementTarget = FontBack;
            ColorPopup.IsOpen = true;
            SelectBorder = FontBack;
        }

        private void FormTitleColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ColorPicker.SelectedColor = ((SolidColorBrush)FormTitleColor.Background).Color;
            ColorPopup.PlacementTarget = FormTitleColor;
            ColorPopup.IsOpen = true;
            SelectBorder = FormTitleColor;
        }

        private void MainColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ColorPicker.SelectedColor = ((SolidColorBrush)MainColor.Background).Color;
            ColorPopup.PlacementTarget = MainColor;
            ColorPopup.IsOpen = true;
            SelectBorder = MainColor;
        }

        Border SelectBorder;

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (SelectBorder == FontBack)//字体颜色
            {
               App.app.Resources.Remove("MainTextColor");
               App.app.Resources.Add("MainTextColor", new SolidColorBrush(ColorPicker.SelectedColor));
               App.app.SetMainTextColor(ColorPicker.SelectedColor);
            }
            if (SelectBorder == FormBackColor)
            {
                App.app.Resources.Remove("MainBackColor");
                App.app.Resources.Add("MainBackColor", new SolidColorBrush(ColorPicker.SelectedColor));
                App.app.SetMainBackColor(ColorPicker.SelectedColor);
            }
            if (SelectBorder == FormTitleColor)
            {
                App.app.Resources.Remove("MainLeftBackColor");
                App.app.Resources.Add("MainLeftBackColor", new SolidColorBrush(ColorPicker.SelectedColor));
                App.app.SetMainLeftBack(ColorPicker.SelectedColor);
            }
            if (SelectBorder == MainColor)
            {
                App.app.SetMainColor(ColorPicker.SelectedColor);
            }
        }

        /// <summary>
        /// 窗体背景色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBackColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ColorPicker.SelectedColor = ((SolidColorBrush)FormBackColor.Background).Color;
            ColorPopup.PlacementTarget = FormBackColor;
            ColorPopup.IsOpen = true;
            SelectBorder = FormBackColor;
        }

        private void FormBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".jpg",
                Filter = "jpg|*.jpg|png|*.png"
            };
            if (ofd.ShowDialog() == true&&File.Exists(ofd.FileName))
            {
                //此处做你想做的事 ...=ofd.FileName; 
                Uri uri = new Uri(ofd.FileName, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage(uri);
                FormBack.Source = bitmap;
                App.BackgroundPath = ofd.FileName;
            }
        }

        /// <summary>
        /// 修改透明度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderBackgroundOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        int loadtime = 0;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (loadtime == 0)
            {
                SliderBackgroundOpacity.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(SliderBackgroundOpacity_MouseLeftButtonUp), true);
                DMSlider.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DMSlider_MouseLeftButtonUp), true);
            }
            SliderBackgroundOpacity.Value = App.BackgroundOpacity * 100;
            DMSlider.Value = App.MainCornerRadius * 100;
            if (App.BackgroundPath != null&& App.BackgroundPath!=""&&FormBack.Source==null)
            {
                Uri uri = new Uri(App.BackgroundPath, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage(uri);
                FormBack.Source = bitmap;
            }
        }

        private void DMSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.app.SetMainCornerRadius(DMSlider.Value);
        }

        private void SliderBackgroundOpacity_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.BackgroundOpacity = SliderBackgroundOpacity.Value / 100;
        }

      
    }
}
