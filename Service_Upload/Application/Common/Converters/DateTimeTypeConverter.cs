using AutoMapper;
using Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Converters
{
    public class DateTimeTypeConverter : ITypeConverter<DateTime?, int?>, ITypeConverter<DateTime, int>
    {
        public int? Convert(DateTime? source, int? destination, ResolutionContext context)
        {
            return source != null ? ConvertDateTime.DateTimeToUnixTimeStamp(source.Value) : (int?)null;
        }

        public int Convert(DateTime source, int destination, ResolutionContext context)
        {
            return ConvertDateTime.DateTimeToUnixTimeStamp(source);
        }
    }
}
