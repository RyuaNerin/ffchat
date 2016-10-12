using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FFChat.Windows.Converters
{
    internal class ForegroundConverter : IValueConverter
    {
        private static readonly Brush[] brushes =
        {
            // 말하기
            CreateBrushFromHtml("#f7f7f7"),
            // 떠들기
            CreateBrushFromHtml("#ffff00"),
            // 외치기
            CreateBrushFromHtml("#ffa666"),
            // 귓속말
            CreateBrushFromHtml("#ffb8de"),
            // 파티
            CreateBrushFromHtml("#66e5ff"),
            // 연합파티
            CreateBrushFromHtml("#ff7f00"),
            // 자유부대
            CreateBrushFromHtml("#abdbe5"),
            // 링크셸
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
            CreateBrushFromHtml("#d4ff7d"),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ChatType.FilterList[(int)value].Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
