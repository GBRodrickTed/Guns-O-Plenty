using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GunsOPlenty.Trials.UI
{
    public class TrialButton : MonoBehaviour
    {
        public Trial trial;
        public Text title;
        public Image icon;
        public Image panel;
        public Button button;
        public void Start()
        {
            button = GetComponent<Button>();
            title = transform.Find("Title").GetComponent<Text>();
            icon = transform.Find("Icon").GetComponent<Image>();
            panel = transform.GetComponent<Image>();
            UpdateDisplay();
            if (trial != null)
            {
                title.text = trial.Name;
                button.onClick.AddListener(SetDesc);
                if (trial.Icon)
                {
                    icon.sprite = trial.Icon;
                }
            }
        }
        public void UpdateDisplay()
        {
            if (trial != null && UnlockManager.trialsCompletedCount[trial.ID].value > 0)
            {
                panel.color = Color.green;
            }
        }
        public void SetDesc()
        {
            TrialManager.trialMenu.SetDesc(trial);
        }
    }
}
