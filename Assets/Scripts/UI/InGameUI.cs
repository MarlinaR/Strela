using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        public Font font;
        public GameObject endGameMenu;
        public Button nextLevelBtn;
        public Image highscorePanel;
        public GameObject loseGameMenu;
        
        public Text arrowsLabel;
        public Text scoreLabel;
        public AudioClip audioClip;

        private AudioSource myAudioSource;

        public void LoadHighscores(List<ScoreInfo> highscores, int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                var row = new GameObject("Row_" + i);
                row.layer = LayerMask.NameToLayer("UI");
            
                row.transform.SetParent(highscorePanel.gameObject.transform);
            
                var rowTransform = row.AddComponent<RectTransform>();
                var parentRect = highscorePanel.rectTransform.rect;
                rowTransform.sizeDelta = new Vector2(parentRect.width, 40);

                var color = Color.white;
                if (highscores[i].isLast)
                {
                    color = Color.cyan;
                }
                
                var usernameObj = row.transform.AddUIObject("Row_" + i + "_username",
                    new Vector2(150, -15), new Vector2(200, 30),
                    new Vector2(0, 1), new Vector2(0, 1));
                usernameObj.AddText(highscores[i].Username, font, color);
                var scoreObj = row.transform.AddUIObject("Row_" + i + "_score",
                    new Vector2(-30, -20), new Vector2(75, 30),
                    new Vector2(1, 1), new Vector2(1, 1));
                scoreObj.AddText(highscores[i].Score.ToString(), font, color);
            }
        }

        private void Awake()
        {
            myAudioSource = gameObject.AddComponent<AudioSource>();
            myAudioSource.clip = audioClip;
            myAudioSource.playOnAwake = false;
            myAudioSource.loop = false;
            myAudioSource.volume = 0.4f;
        }

        private void Start()
        {
            nextLevelBtn.onClick.AddListener(delegate
            {
                myAudioSource.Play();
                GameSettings.instance.NextLevel();
            });
        }

        public void Restart() {
            GameSettings.instance.Reset();
        }

        public void Exit() {
            GameSettings.instance.Exit();
        }

        private void Update()
        {
            if (GameSettings.currentLevel != null)
            {
                scoreLabel.text = "Time: " + GameSettings.currentLevel.score;
                arrowsLabel.text = "Arrows: " + GameSettings.currentLevel.arrows;
            }
        }
    }
}
