using UnityEngine.UIElements;

namespace Nebula.Editor
{
    public static class VisualElementUtil
    {
        public static void SetDisplay(this VisualElement element, bool displayValue)
        {
            element.style.display = new StyleEnum<DisplayStyle>(displayValue ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}