using System.Collections;
using System.Collections.Generic;
using System.IO;
using Blocks;
using Levels;
using SimpleFileBrowser;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using File = System.IO.File;

public class LevelEditor : MonoBehaviour
{
    public Image blockPanel;
    public GameObject[] blocks;

    public GameObject blockBtnPrefab;

    public LevelTheme levelTheme;
    
    private GameObject currentObject;
    private EditorBlockBtn[] myBlockButtons;
    private Camera myCamera;
    private Dictionary<Vector2Int, GameObject> levelMap;

    private GameObject selectedObject;
    public EditorInspectorPanel myInspector;
    
    private void Awake()
    {
        levelMap = new Dictionary<Vector2Int, GameObject>();
        myCamera = Camera.main;
        blockPanel.gameObject.SetActive(false);

        int xCoord = 25;
        int yCoord = -25;
        
        myBlockButtons = new EditorBlockBtn[blocks.Length];
        
        for (int i = 0; i < blocks.Length; i++)
        {
            var btn = Instantiate(blockBtnPrefab, new Vector2(xCoord, yCoord), Quaternion.identity);
            btn.transform.SetParent(blockPanel.transform, false);
            var btnComponent = btn.GetComponent<EditorBlockBtn>();
            btnComponent.blockType = blocks[i];
            myBlockButtons[i] = btnComponent;

            int idx = i;
            
            btnComponent.AddListener(() =>
            {
                for (int j = 0; j < myBlockButtons.Length; j++)
                {
                    myBlockButtons[j].Deselect();
                }
                
                myBlockButtons[idx].Select();
                blockPanel.gameObject.SetActive(false);
                UpdateCurrentObject(myBlockButtons[idx].blockType);
            });

            yCoord -= 50;
        }

        var levelAsset = ScriptableObject.CreateInstance<LevelLayout>();
        levelAsset.theme = levelTheme;
        levelAsset.InstantiateGrass(transform);
    }

    private void UpdateCurrentObject(GameObject newObject)
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
        }
        
        InstantiateAsCurrentObject(newObject);
    }

    private void InstantiateAsCurrentObject(GameObject obj)
    {
        currentObject = Instantiate(obj, new Vector2(0, 0), Quaternion.identity);
        currentObject.transform.SetParent(transform);
        var blockComp = currentObject.GetComponent<ILevelBlock>() as MonoBehaviour;

        if (blockComp != null)
        {
            blockComp.enabled = false;
        }
    }

    private void ProcessObjectPlacement()
    {
        var newPos = myCamera.ScreenToWorldPoint(Input.mousePosition);
        newPos.x = Mathf.Round(newPos.x);
        newPos.y = Mathf.Round(newPos.y);
        newPos.z = 0;

        if (Mathf.Abs(newPos.x) <= LevelLayout.MinLevelWidth / 2 && Mathf.Abs(newPos.y) <= LevelLayout.MinLevelHeight / 2)
        {
            currentObject.transform.position = newPos;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var posInMap = new Vector2Int(Mathf.RoundToInt(currentObject.transform.position.x), Mathf.RoundToInt(currentObject.transform.position.y));
            var blockComp = currentObject.GetComponent<ILevelBlock>();
            if (blockComp != null)
            {
                if (levelMap.ContainsKey(posInMap) && levelMap[posInMap] != null)
                {
                    Destroy(levelMap[posInMap]);
                    levelMap[posInMap] = currentObject;
                }
                else
                {
                    levelMap.Add(posInMap, currentObject);
                }

                if (!(blockComp is Archer) && blockComp is MonoBehaviour behaviour)
                {
                    behaviour.enabled = true;
                }

                InstantiateAsCurrentObject(currentObject);
            }
            else
            {
                if (levelMap.ContainsKey(posInMap) && levelMap[posInMap] != null)
                {
                    Destroy(levelMap[posInMap]);
                    levelMap.Remove(posInMap);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Destroy(currentObject);
            currentObject = null;
        }
    }
    
    private void Update()
    {
        if (currentObject != null)
        {
            ProcessObjectPlacement();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var mousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
            var posInMap = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

            if (levelMap.ContainsKey(posInMap) && levelMap[posInMap] != null)
            {
                myInspector.UpdateSelectedObject(levelMap[posInMap]);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            myInspector.UpdateSelectedObject(null);
        }
    }

    public void ShowPanelClick()
    {
        blockPanel.gameObject.SetActive(!blockPanel.gameObject.activeSelf);
    }

    public void SaveLevelClick()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
            currentObject = null;
        }
        
        StartCoroutine(SaveLevel());
    }

    public void LoadLevelClick()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        if (Application.isEditor)
        {
            FileBrowser.SetFilters(true, ".asset", ".lvl");
        }
        else
        {
            FileBrowser.SetFilters(true, ".lvl");
        }

        yield return FileBrowser.WaitForLoadDialog(initialPath: Application.dataPath);
        
        if (FileBrowser.Success)
        {
            var path = FileBrowser.Result[0].Replace("\\", "/");
            LevelLayout levelLayout;
            if (Application.isEditor)
            {
                if (path.ToLower().EndsWith(".asset"))
                {
                    path = path.Substring(Application.dataPath.Length - "Assets".Length);
                    levelLayout = LevelLayout.LoadFromAsset(path);
                }
                else
                {
                    levelLayout = LevelLayout.LoadFromJson(path);
                }
            }
            else
            {
                levelLayout = LevelLayout.LoadFromJson(path);
            }

            if (levelLayout == null)
            {
                Debug.Log("Cant load level from path: " + path);
            }
            else
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                levelMap = new Dictionary<Vector2Int, GameObject>();
                levelLayout.InstantiateLevel(transform, ref levelMap);
            }
        }
    }

    public void TestRun()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
            currentObject = null;
        }
        
        var levelLayout = ScriptableObject.CreateInstance<LevelLayout>();
        levelLayout.theme = levelTheme;
        levelLayout.GetFromMap(levelMap);
        GameSettings.instance.isTestRun = true;
        GameSettings.instance.nextLevel = levelLayout;
        GameSettings.instance.NextLevel();
    }

    private IEnumerator SaveLevel()
    {
        if (Application.isEditor)
        {
            FileBrowser.SetFilters(true, ".asset", ".lvl");
        }
        else
        {
            FileBrowser.SetFilters(true, ".lvl");
        }

        yield return FileBrowser.WaitForSaveDialog(initialPath: Application.dataPath);
        if (FileBrowser.Success)
        {
            var levelLayout = ScriptableObject.CreateInstance<LevelLayout>();
            levelLayout.theme = levelTheme;
            levelLayout.GetFromMap(levelMap);

            var path = FileBrowser.Result[0].Replace("\\", "/");

            if (Application.isEditor && path.ToLower().EndsWith(".asset"))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    path = path.Substring(Application.dataPath.Length - "Assets".Length);
                }
            
                if (path.StartsWith("Assets/"))
                {
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(levelLayout, path);
#endif
                }
                else
                {
                    Debug.Log("Invalid asset path: " + path);
                }
            }
            else
            {
                var json = JsonUtility.ToJson(levelLayout);
                File.WriteAllText(path, json);
            }
        }
    }
}
