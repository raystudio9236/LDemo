using UnityEngine;

namespace RayStudio.UtilScripts.Extension
{
    public static class ComponentExtensions
    {
        public static RectTransform RectTransform(this Component comp)
        {
            return comp.transform as RectTransform;
        }
    }
}