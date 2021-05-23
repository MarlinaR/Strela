using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditorArrayItem : MonoBehaviour
{
    public EditorInspectorPanel inspectorPanel;

    public List<GameObject> SelectedObjects
    {
        get
        {
            var res = new List<GameObject>();
            foreach (var obj in mySelectedObjects)
            {
                if (obj != null)
                {
                    res.Add(obj);
                }
            }

            return res;
        }
        set
        {
            Awake();
            
            foreach (var obj in value)
            {
                AddObject(obj, buttons.Count - 1);
            }
        }
    }

    private List<GameObject> mySelectedObjects;
    private List<Button> buttons;

    public Image contentHost;
    public Button buttonPrefab;
    
    public UnityEvent<EditorArrayItem> OnChange;

    public Type TypeChecker = typeof (GameObject);

    private int activeButtonIdx = -1;

    private void Awake()
    {
        if (buttons != null)
        {
            return;
        }
        
        buttons = new List<Button>();
        mySelectedObjects = new List<GameObject>();
        
        var btn = Instantiate(buttonPrefab, contentHost.transform);
        buttons.Add(btn);
        mySelectedObjects.Add(null);
        btn.onClick.AddListener(() =>
        {
            activeButtonIdx = 0;
            inspectorPanel.currrentObjectSelection = this;
        });
    }

    public void DisableSelection()
    {
        activeButtonIdx = -1;
        inspectorPanel.currrentObjectSelection = null;
    }

    private void AddObject(GameObject obj, int objIdx) 
    {
        buttons[objIdx].GetComponentInChildren<Text>().text = obj.transform.position.ToString();
        mySelectedObjects[objIdx] = obj;
        
        if (objIdx == buttons.Count - 1)
        {
            var btn = Instantiate(buttonPrefab, contentHost.transform);

            int idx = buttons.Count;
            buttons.Add(btn);
            mySelectedObjects.Add(null);
       
            btn.onClick.AddListener(() =>
            {
                activeButtonIdx = idx;
                inspectorPanel.currrentObjectSelection = this;
            });
        }
    }
    
    public void UpdateContent(GameObject obj)
    {
        if (activeButtonIdx == -1)
        {
            return;
        }

        if (obj.GetComponent(TypeChecker) == null)
        {
            Debug.Log("Tried to add invalid object into array");
            return;
        }
        
        AddObject(obj, activeButtonIdx);
        OnChange.Invoke(this);
    }
}
