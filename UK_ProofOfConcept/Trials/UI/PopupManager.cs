using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GunsOPlenty.Trials.UI
{
    // Note to self; when using monosingletons, use Instance instead of instance
    public class PopupManager : MonoSingleton<PopupManager> // so. many. managers.
    {
        public GameObject canvas;
        public void Start()
        {
            PopupManager.Instance.transform.parent = PersistentCanvas.instance.transform;
        }
        public List<GameObject> popups = new List<GameObject>();
        public void Update()
        {
            if (popups.Count > 0)
            {
                if (popups[0] == null)
                {
                    List<GameObject> newList = new List<GameObject>();
                    for (int i = 0; i < popups.Count; i++)
                    {
                        if (popups[i] != null)
                        {
                            newList.Add(popups[i]);
                        }
                    }
                    popups = newList;
                }
                else if (popups[0].activeSelf == false)
                {
                    popups[0].SetActive(true);
                }
            }
        }
        public void CreatePopup(string text, float speed = 200, float time = 2.5f)
        {
            GameObject popup = GameObject.Instantiate<GameObject>(AssetHandler.customPrefabs["Unlock Message"], PersistentCanvas.instance.transform);
            popup.GetComponent<AchievementPopup>().text.text = text;
            popup.GetComponent<AchievementPopup>().speed = speed;
            popup.GetComponent<AchievementPopup>().timeToGo = time;
            popup.SetActive(false);
            popups.Add(popup);
        }

        public void ClearPopups()
        {
            foreach(GameObject popup in popups)
            {
                GameObject.Destroy(popup);
            }
            popups = new List<GameObject>();
        }
    }
}
