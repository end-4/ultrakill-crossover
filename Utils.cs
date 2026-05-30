using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Crossover;

internal static class Utils {
    // https://github.com/EladNLG/UltrakillHealthbars/blob/main/ReflectionUtils.cs
    internal static T GetPrivateField<T>(this object obj, string field) {
        Type t = obj.GetType();
        if (t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance) == null) {
            throw new ArgumentException($"The field {field} does not exist in target class {t.Name}!");
        }

        return (T)t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
    }

    internal static Color Transparentize(Color color) {
        return new Color(color.r, color.g, color.b, 0);
    }

    internal static Color Opacitize(Color color) {
        return new Color(color.r, color.g, color.b, 1);
    }

    internal static string ToSnakeCase(this string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }

        string result = Regex.Replace(input, @"(?<!^)(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[A-Z])", "_");
        return result.ToLowerInvariant();
    }
}
