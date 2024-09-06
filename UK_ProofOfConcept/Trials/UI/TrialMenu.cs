using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Object = UnityEngine.Object;

namespace GunsOPlenty.Trials.UI
{
    public class TrialMenu : MonoBehaviour, IPointerClickHandler
    {
        public TMPro.TextMeshProUGUI desc;
        public Text title;
        public Image icon;
        public Image medal;
        public Sprite[] medal_sprites;
        public Trial trial;
        public List<TrialButton> trialButtons;
        public Dropdown trialDifficulty;
        public Button trialStart;
        public TrialInfoButton trialInfoButton;

        public GameObject trialButtonContent;
        public GameObject trialDescContent;
        public GameObject trialDescBar;
        public GameObject trialSelectBar;

        public void Awake()
        {
            trialButtons = new List<TrialButton>();
            trialDescBar = transform.Find("Trial Desc Bar").gameObject;
            trialSelectBar = transform.Find("Trial Select Bar").gameObject;
            trialButtonContent = transform.Find("Trial Select Bar/Background/Panel/Scroll View/Viewport/Content").gameObject;
            trialDescContent = transform.Find("Trial Desc Bar/Panel/Scroll View/Viewport/Content").gameObject;
            trialInfoButton = Instantiate<GameObject>(AssetHandler.LoadAsset<GameObject>("Trial Info Button"), CanvasController.Instance.transform).AddComponent<TrialInfoButton>();
            trialInfoButton.thingymajig = Instantiate<GameObject>(AssetHandler.LoadAsset<GameObject>("Trial Info Desc"), CanvasController.Instance.transform);
            trialInfoButton.thingymajig.AddComponent<HudOpenEffect>();
            trialInfoButton.gameObject.AddComponent<HudOpenEffect>();
            trialInfoButton.thingymajig.SetActive(false);
            trialInfoButton.gameObject.SetActive(false);
            title = transform.Find("Trial Desc Bar/Panel/Title").GetComponent<Text>();
            icon = transform.Find("Trial Desc Bar/Panel/Icon").GetComponent<Image>();

            //Medalians
            medal = transform.Find("Trial Desc Bar/Panel/Icon/Medal").GetComponent<Image>();
            medal.enabled = false;
            medal_sprites = new Sprite[6];
            medal_sprites[0] = AssetHandler.LoadAsset<Sprite>("medal_harmless");
            medal_sprites[1] = AssetHandler.LoadAsset<Sprite>("medal_lenient");
            medal_sprites[2] = AssetHandler.LoadAsset<Sprite>("medal_standard");
            medal_sprites[3] = AssetHandler.LoadAsset<Sprite>("medal_violent");
            medal_sprites[4] = AssetHandler.LoadAsset<Sprite>("medal_brutal");
            medal_sprites[5] = AssetHandler.LoadAsset<Sprite>("medal_ultrakill_must_die");

            desc = trialDescContent.GetComponent<TMPro.TextMeshProUGUI>();
            trialDifficulty = transform.Find("Trial Desc Bar/Panel/Dropdown").GetComponent<Dropdown>();
            trialDifficulty.value = PrefsManager.Instance.GetInt("difficulty");
            trialStart = transform.Find("Trial Desc Bar/Panel/Button").GetComponent<Button>();
            trialStart.onClick.AddListener(StartTrial);

            foreach (Trial trial in TrialManager.trials)
            {
                GameObject button = Object.Instantiate(AssetHandler.LoadAsset<GameObject>("Trial Button"), trialButtonContent.transform);
                TrialButton trialButton = button.AddComponent<TrialButton>();
                trialButton.trial = trial;
                trialButtons.Add(trialButton);
            }
            trialDescBar.SetActive(false);
        }

        public void OnEnable()
        {
            trialDifficulty.value = PrefsManager.Instance.GetInt("difficulty");
            trialInfoButton.thingymajig.SetActive(false);
            trialInfoButton.gameObject.SetActive(true);
        }

        public void OnDisable()
        {
            trialDescBar.SetActive(false);
            trialInfoButton.thingymajig.SetActive(false);
            trialInfoButton.gameObject.SetActive(false);
        }

        public void StartTrial()
        {
            if (trial != null)
            {
                trial.StartTrial();
                PrefsManager.Instance.SetInt("difficulty", trialDifficulty.value);
            }
        }
        public void UpdateDisplays()
        {
            foreach(TrialButton tbutt in trialButtons)
            {
                tbutt.UpdateDisplay();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(desc, Input.mousePosition, null);
                if (linkIndex > -1)
                {
                    Application.OpenURL(desc.textInfo.linkInfo[linkIndex].GetLinkID());
                }
            }
        }

        public void SetDesc(Trial trial)
        {
            trialInfoButton.gameObject.SetActive(false);
            trialDescBar.SetActive(true);
            title.text = trial.Name;
            this.trial = trial;
            if (trial.Icon)
            {
                icon.sprite = trial.Icon;
            }
            if (UnlockManager.trialDifs[trial.ID].value < 0 || (UnlockManager.trialsCompletedCount[trial.ID].value <= 0))
            {
                medal.enabled = false;
            } else
            {
                medal.enabled = true;
                int medal_index = UnlockManager.trialDifs[trial.ID].value;
                if (medal_index > 5) medal_index = 5;
                medal.sprite = medal_sprites[medal_index];
            }
            desc.text = "";
            if (trial.Tribulations != null && trial.Tribulations.Count > 0)
            {
                desc.text += "<color=red>Tribulations:</color>";
                for (int i = 0; i < trial.Tribulations.Count; i++)
                {
                    desc.text += "\n-<indent=1em><margin-left=0em>" + trial.Tribulations[i] + "</margin></indent>";
                }
            }

            if (trial.TipsAndTricks != null && trial.TipsAndTricks.Count > 0)
            {
                desc.text += "\n\n";
                desc.text += "Pro Tip:";
                for (int i = 0; i < trial.TipsAndTricks.Count; i++)
                {
                    desc.text += "\n-<indent=1em><margin-left=0em>" + trial.TipsAndTricks[i] + "</margin></indent>";
                }
            }

            if (trial.Reward != null && trial.Reward.Count > 0)
            {
                desc.text += "\n\n";
                desc.text += "<color=#ffc500>Reward:</color>";
                for (int i = 0; i < trial.Reward.Count; i++)
                {
                    desc.text += "\n-<indent=1em><margin-left=0em>" + trial.Reward[i] + "</margin></indent>";
                }
            }

            if (trial.FunFacts != null)
            {
                int facks = Math.Min(trial.FunFacts.Count, UnlockManager.trialsCompletedCount[trial.ID].value - 1);
                if (facks > 0)
                {
                    desc.text += "\n\n";
                    desc.text += "<color=#44ff00>Fun Fact:</color>";
                    desc.text += "\n-<indent=1em><margin-left=0em>" + trial.FunFacts[0] + "</margin></indent>";
                }

                if (facks > 1)
                {
                    desc.text += "\n\n";
                    desc.text += "<color=#44ff00>Funner Fact:</color>";
                    desc.text += "\n-<indent=1em><margin-left=0em>" + trial.FunFacts[1] + "</margin></indent>";
                }

                if (facks > 2)
                {
                    for (int i = 2; i < trial.FunFacts.Count; i++)
                    {
                        desc.text += "\n\n";
                        desc.text += "<color=#44ff00>Funner";
                        for (int j = 1; j < i; j++)
                        {
                            desc.text += "er";
                        }
                        desc.text += " Fact:</color>";
                        desc.text += "\n-<indent=1em><margin-left=0em>" + trial.FunFacts[i] + "</margin></indent>";
                    }
                }
            }
        }
    }
}
