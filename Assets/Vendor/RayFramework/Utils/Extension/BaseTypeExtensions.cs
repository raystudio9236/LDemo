using UnityEngine;

namespace RayStudio.UtilScripts.Extension
{
    public static class BaseTypeExtensions
    {
        /// <summary>
        /// string to color
        /// "#B79E00FF"
        public static Color HexToColor(this string hexString)
        {
            ColorUtility.TryParseHtmlString(hexString, out var c);
            return c;
        }
    }
}