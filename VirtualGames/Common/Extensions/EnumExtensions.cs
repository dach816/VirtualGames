using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VirtualGames.Common.Extensions
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList<T>(this T obj)
        {
            return new SelectList(
                Enum.GetValues(typeof(T))
                    .OfType<Enum>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.GetDescription(),
                        Value = Convert.ToInt32(x).ToString()
                    }),
                "Value",
                "Text");
        }

        public static string GetDescription(this Enum enumValue)
        {
            var member = enumValue
                ?.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>();
            return member == null ? string.Empty : member.Description;
        }

        public static IEnumerable<string> ToStringList<T>(this T obj)
        {
            return Enum.GetValues(typeof(T))
                .OfType<Enum>()
                .Select(x => x.ToString("G"));
        }
    }
}