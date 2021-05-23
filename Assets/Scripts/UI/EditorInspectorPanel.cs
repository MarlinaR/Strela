using System;
using System.Collections.Generic;
using System.Globalization;
using Blocks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EditorInspectorPanel : MonoBehaviour
    {
        public Font font;

        public InputField inputFieldPrefab;
        public EditorArrayItem arrayItemPrefab;

        public EditorArrayItem currrentObjectSelection;
        
        private Image panel;
        private GameObject selectedObject;

        private void Awake()
        {
            panel = GetComponent<Image>();
            panel.gameObject.SetActive(false);
        }

        private void ClearPanel()
        {
            foreach (Transform child in panel.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private RectTransform CreateRow(string name, int idx, int size = 1)
        {
            var row = new GameObject(name);
            row.layer = LayerMask.NameToLayer("UI");
            row.transform.SetParent(panel.transform);
            var rowTransform = row.AddComponent<RectTransform>();
            rowTransform.anchoredPosition = new Vector2(0, -30 * size / 2 - idx * 30);
            rowTransform.anchorMin = new Vector2(0, 1);
            rowTransform.anchorMax = new Vector2(1, 1);
            rowTransform.sizeDelta = new Vector2(1, 30 * size);
            return rowTransform;
        }
    
        public void UpdateSelectedObject(GameObject obj)
        {
            if (obj == null)
            {
                DisableCurrenObjectSelection();
                ClearPanel();
                panel.gameObject.SetActive(false);
                return;
            }

            if (currrentObjectSelection != null && selectedObject != null)
            {
                currrentObjectSelection.UpdateContent(obj);
                DisableCurrenObjectSelection();
                return;
            }
            
            if (selectedObject != null)
            {
                ClearPanel();
            }
        
            selectedObject = obj;

            var rowTransform = CreateRow("TitleRow", 0);

            var textObj =  rowTransform.AddUIObject("Title", new Vector2(110, -30),
                new Vector2(200, 30),
                new Vector2(0, 1), new Vector2(0, 1));
            textObj.AddText("Title: " + obj.name, font, Color.black);

            var configurable = obj.GetComponent<IConfigurable>();
            if (configurable != null)
            {
                ProcessConfigurable(configurable);
            } 
            panel.gameObject.SetActive(true);

            DisableCurrenObjectSelection();
        }

        private void DisableCurrenObjectSelection()
        {
            if (currrentObjectSelection != null)
            {
                currrentObjectSelection.DisableSelection();
            }

            currrentObjectSelection = null;
        }
    
        private void ProcessConfigurable(IConfigurable configurable)
        {
            var config = configurable.GetConfiguration();
            int offset = 0;
            for (int i = 0; i < config.Count; i++)
            {
                var rowTransform = CreateRow("PropertyRow_" + i, i + 1 + offset);
                var titleObj = rowTransform.AddUIObject("Property_" + i,
                    new Vector2(110, -30), new Vector2(200, 30), 
                    new Vector2(0, 1), new Vector2(0, 1));
                titleObj.AddText(config[i].Item1, font, Color.black);
                int idx = i;
                switch (config[i].Item2)
                {
                    case PropertyType.Bool:
                    case PropertyType.Number:
                        var inputFieldObj = rowTransform.AddUIObject("Value_" + i,
                            new Vector2(300, -30), new Vector2(200, 30), 
                            new Vector2(0, 1), new Vector2(0, 1));
                        
                        inputFieldObj.AddFloatInputField(inputFieldPrefab, (Convert.ToSingle(config[i].Item3)).ToString(CultureInfo.InvariantCulture), (string str) =>
                        {
                            configurable.SetConfiguration(new Tuple<string, object>(config[idx].Item1, Convert.ToSingle(str, CultureInfo.InvariantCulture)));
                        });
                        break;
                    case PropertyType.ObjectArray:
                        offset++;
                        var objRowTransform = CreateRow("ArrRow_0", i + 1 + offset, 4);
                        offset += 3;

                        var arrayItem = Instantiate(arrayItemPrefab, objRowTransform);
                        var configTuple = (Tuple<Type, List<GameObject>>) config[idx].Item3;
                        arrayItem.SelectedObjects = configTuple.Item2;
                        arrayItem.inspectorPanel = this;
                        arrayItem.TypeChecker = configTuple.Item1;

                        arrayItem.OnChange.AddListener((arraySelector) =>
                        {
                            configurable.SetConfiguration(
                                new Tuple<string, object>(config[idx].Item1, arraySelector.SelectedObjects)
                            );
                        });
                        
                        break;
                    default:
                        throw new ArgumentException("Type " + config[i].Item2 + " is not supported");
                }
            }
        }
    }
}
