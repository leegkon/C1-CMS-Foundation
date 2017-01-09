﻿using System;
using System.Globalization;

namespace Composite.C1Console.Search.Crawling
{
    /// <summary>
    /// The default field processor for <see cref="DateTime"/> fields.
    /// </summary>
    public class DateTimeDataFieldProcessor: DefaultDataFieldProcessor
    {
        /// <exclude />
        public override object GetIndexValue(object fieldValue)
        {
            return ((DateTime?) fieldValue)?.ToString("s");
        }

        /// <exclude />
        protected override DocumentFieldPreview.ValuePreviewDelegate GetPreviewFunction()
        {
            return value =>
            {
                if (value == null) return null;
                var date = DateTime.ParseExact((string)value, "s", CultureInfo.InvariantCulture);

                return date.ToString("yyyy MMM d");
            };
        }

        /// <exclude />
        public override string[] GetFacetValues(object value)
        {
            var str = ((DateTime?) value)?.ToString("yyyy MM");

            return str != null ? new [] {str} : null;
        }

        /// <exclude />
        protected override DocumentFieldFacet.FacetValuePreviewDelegate GetFacetValuePreviewFunction()
        {
            return value =>
            {
                if (value == null) return null;
                var parts = value.Split(' ');

                int month = int.Parse(parts[1]);

                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                return $"{parts[0]} {monthName}";
            };
        }
    }
}