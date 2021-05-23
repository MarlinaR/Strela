using System.Collections;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public const int testLevelSceneIdx = -1;
    
    public static GameSettings instance;
    public GameObject InGameUI;

    private int myLevelsCnt = 0;
    private int myCurScene = 0;
    
    public string Username;
    public static LevelConfig currentLevel;

    public LevelLayout nextLevel;
    public bool isTestRun;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private AudioSource myAudioSource;
    
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        instance = this;
        myLevelsCnt = SceneManager.sceneCountInBuildSettings;
        myCurScene = SceneManager.GetActiveScene().buildIndex;
        myAudioSource = GetComponent<AudioSource>();

        Debug.Log("Initialized in scene: " + myCurScene + " Total Scenes: " + myLevelsCnt);

        ProcessAudio();
    }

    private void ProcessAudio()
    {
        if (myCurScene == 0)
        {
            if (myAudioSource.clip == null || myAudioSource.clip.name != menuMusic.name)
            {
                myAudioSource.clip = menuMusic;
                myAudioSource.Play();
            }
            
        }
        else if (myCurScene < myLevelsCnt - 2)
        {
            if (myAudioSource.clip == null || myAudioSource.clip.name != gameMusic.name)
            {
                myAudioSource.clip = gameMusic;
                myAudioSource.Play();
            }
        }
        else
        {
            myAudioSource.clip = null;
            myAudioSource.Stop();
        }
    }
    
    public void NextLevel()
    {
        if (isTestRun)
        {
            if (nextLevel != null)
            {
                myCurScene = myLevelsCnt - 1;
                StartCoroutine(LoadScene(myCurScene, LoadSceneMode.Additive));
                return;
            }

            StartCoroutine(UnloadScene());
            return;
        }

        myCurScene++;
        if (myCurScene == myLevelsCnt - 2)
        {
            myCurScene = 0;
        }

        if (nextLevel != null)
        {
            myCurScene = myLevelsCnt - 1;
        }

        StartCoroutine(LoadScene(myCurScene));

        ProcessAudio();
    }

    public void OpenEditor()
    {
        myCurScene = myLevelsCnt - 2;
        StartCoroutine(LoadScene(myCurScene));
        ProcessAudio();
    }

    private IEnumerator UnloadScene()
    {
        var op = SceneManager.UnloadSceneAsync(myCurScene);
        yield return op;
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            root.SetActive(true);
        }
        isTestRun = false;
    }
    
    private IEnumerator LoadScene(int idx, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (mode == LoadSceneMode.Additive)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                root.SetActive(false);
            }
        }

        var op = SceneManager.LoadSceneAsync(idx, mode);
        yield return op;

        if (mode == LoadSceneMode.Additive)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx));
        }

        if (nextLevel != null)
        {
            nextLevel.InstantiateLevel(currentLevel.transform, ref currentLevel.levelMap);
            currentLevel.isTestRun = isTestRun;
            nextLevel = null;
        }
    }

    public void Exit()
    {
        if (isTestRun && nextLevel == null)
        {
            StartCoroutine(UnloadScene());
            return;
        }
        
        myCurScene = 0;
        StartCoroutine(LoadScene(myCurScene));
        ProcessAudio();
    }

    public void Reset()
    {
        if (isTestRun && nextLevel == null)
        {
            StartCoroutine(UnloadScene());
            return;
        }
        SceneManager.LoadScene(myCurScene);
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }
}
