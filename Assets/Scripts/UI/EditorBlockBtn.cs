using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class EditorBlockBtn : MonoBehaviour
    {
        public GameObject blockType;
        public Button btn;

        private Image myImage;

        private void Awake()
        {
            btn = GetComponent<Button>();
            myImage = GetComponent<Image>();
        }

        private void Start()
        {
            myImage.sprite = blockType.GetComponent<SpriteRenderer>().sprite;
            Deselect();
        }

        public void AddListener(UnityAction action)
        {
            btn.onClick.AddListener(action);
        }
    
        public void Select()
        {
            myImage.color = Color.white;
        }

        public void Deselect()
        {
            myImage.color = Color.gray;
        }
    }
}
