using System;
using System.Reflection;
using UnityEngine;

namespace Crossover;

static class Utils {
    // https://github.com/EladNLG/UltrakillHealthbars/blob/main/ReflectionUtils.cs
    public static T GetPrivateField<T>(this object obj, string field) {
        Type t = obj.GetType();
        if (t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance) == null) {
            throw new ArgumentException($"The field {field} does not exist in target class {t.Name}!");
        }

        return (T)t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
    }

    public static Color Transparentize(Color color) {
        return new Color(color.r, color.g, color.b, 0);
    }

    public static Color Opacitize(Color color) {
        return new Color(color.r, color.g, color.b, 1);
    }
}
