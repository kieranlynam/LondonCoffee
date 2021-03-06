﻿using System;
using Windows.UI.Xaml.Data;

namespace CoffeeClientPrototype.Converters
{
    public sealed class VotesCountToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0} ratings", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
