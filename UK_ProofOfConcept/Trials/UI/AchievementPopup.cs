using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GunsOPlenty.Trials.UI
{
    public class AchievementPopup : MonoBehaviour
    {
        public static List<GameObject> popups = new List<GameObject>();
        public bool timeToShine = false;
        public float timeToGo = 2.5f;
        public Text text;
        public float stayTimer = 0;
        public RectTransform rectTransform;
        public float speed = 200f;
        private int phase = 0; // 0 - Comming in, 1 - Staying, 2 - Leaving
        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            text = transform.Find("Text").GetComponent<Text>();
        }
        public void Start()
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x, rectTransform.anchoredPosition.y); // moves it to the end of the screen
        }
        public void Update()
        {
            //if (!timeToShine) return;
            switch (phase)
            {
                case 0:
                    if (rectTransform.anchoredPosition.x > 0)
                    {
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - (speed * Time.unscaledDeltaTime), rectTransform.anchoredPosition.y);
                    }
                    if (rectTransform.anchoredPosition.x <= 0)
                    {
                        rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                        phase = 1;
                    }
                    break;
                case 1:
                    stayTimer += Time.unscaledDeltaTime;
                    if (stayTimer >= timeToGo)
                    {
                        phase = 2;
                    }
                    break;
                case 2:
                    if (rectTransform.anchoredPosition.x < rectTransform.sizeDelta.x)
                    {
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + (speed * Time.unscaledDeltaTime), rectTransform.anchoredPosition.y);
                    }
                    if (rectTransform.anchoredPosition.x >= rectTransform.sizeDelta.x)
                    {
                        rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                        Destroy(gameObject);
                    }
                    break;
                default:
                    Debug.Log("Um... Dafuq??? (AchievementPopup)");
                    break;
            }
            
        }
    }
}
