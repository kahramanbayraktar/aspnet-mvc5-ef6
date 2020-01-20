using System;
using System.Linq;
using System.Web.Mvc;

namespace CedTests.Extensions
{
    public static class DynamicExtensions
    {
        public static T GetValue<T>(this JsonResult jsonResult, string propertyName)
        {
            var property =
                jsonResult.Data.GetType().GetProperties()
                    .FirstOrDefault(p => string.CompareOrdinal(p.Name, propertyName) == 0);

            if (null == property)
                throw new ArgumentException("propertyName not found", nameof(propertyName));
            return (T)property.GetValue(jsonResult.Data, null);
        }
    }
}
