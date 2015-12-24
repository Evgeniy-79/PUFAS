using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PUFAS
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new BitmapImage(
                new Uri(
                    System.IO.Directory.GetCurrentDirectory() + "\\img\\folder.png"));
            }
            else
            {
                return new BitmapImage(
                new Uri(
                    System.IO.Directory.GetCurrentDirectory() + "\\img\\file.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
