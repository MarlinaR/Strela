using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public static class UIUtil
    {
        public static GameObject AddUIObject(this Transform host, string objectName, Vector2 pos, Vector2 size,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var label = new GameObject(objectName) {layer = LayerMask.NameToLayer("UI")};
            label.transform.SetParent(host);
            var rectTransform = label.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = pos;
            rectTransform.sizeDelta = size;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            return label;
        }
        
        public static Text AddText(this GameObject host, string text, Font font, Color textColor, int fontSize = 18)
        {
            var textComponent = host.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.font = font;
            textComponent.color = textColor;
            return textComponent;
        }
        
        public static InputField AddFloatInputField(this GameObject host, InputField inputFieldPrefab, string text, UnityAction<string> callback)
        {
            var inputField = Object.Instantiate(inputFieldPrefab, host.transform);
            inputField.contentType = InputField.ContentType.DecimalNumber;
            inputField.text = text;
            inputField.onEndEdit.AddListener(callback);
            return inputField;
        }

        public static Button AddButton(this GameObject host, Button buttonPrefab, UnityAction clickCallback)
        {
            var btn = Object.Instantiate(buttonPrefab, host.transform);
            btn.onClick.AddListener(clickCallback);
            return btn;
        }
    }
}