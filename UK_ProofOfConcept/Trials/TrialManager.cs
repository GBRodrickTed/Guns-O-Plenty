using GunsOPlenty.Data;
using GunsOPlenty.Trials.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GunsOPlenty.Trials
{
    public static class TrialManager
    {
        public static List<Trial> trials = new List<Trial>();
        public static Dictionary<string, Trial> trialDict = new Dictionary<string, Trial>();

        public static Trial trialCurrent;
        public static TrialMenu trialMenu;
        public static Button trialMenuButton;
        public static GameObject chapterMenu;
        public static bool exclusiveDeveleporMode = false;
        
        
        public static void SetupTrials()
        {

            AddTrial(new VnicTheOnehog());
            AddTrial(new Stylin());
            AddTrial(new AndroidUser());
            foreach (Trial trial in trials)
            {
                trial.Init();
            }
        }
        public static void SetupUI()
        {
            chapterMenu = CanvasController.Instance.transform.Find("Chapter Select").gameObject;
            trialMenu = Object.Instantiate(AssetHandler.LoadAsset<GameObject>("Trial UI"), CanvasController.Instance.transform).AddComponent<TrialMenu>();
            trialMenuButton = Object.Instantiate(AssetHandler.LoadAsset<GameObject>("Trial Menu Button"), chapterMenu.transform).GetComponent<Button>();
            trialMenuButton.onClick.AddListener(TrialMenuButtonOnClick);
            trialMenu.gameObject.AddComponent<MenuEsc>().previousPage = chapterMenu;
            trialMenu.gameObject.AddComponent<HudOpenEffect>();
            trialMenu.gameObject.SetActive(false);
            //Object.Instantiate(AssetHandler.LoadAsset<GameObject>("Trial Bar"), CanvasController.Instance.transform);

        }
        public static void TrialMenuButtonOnClick()
        {
            trialMenu.gameObject.SetActive(true);
            chapterMenu.SetActive(false);
            if (exclusiveDeveleporMode)
            {
                PopupManager.Instance.CreatePopup("Shimmy Shimmyay Shimmyay Shimmyah");
                PopupManager.Instance.CreatePopup("(Drank) Shwalalalah");
                PopupManager.Instance.CreatePopup("(Drank) Shwalalalah");
            }
        }
        public static void StartTrial(string id)
        {
            foreach (Trial trial in trials)
            {
                trial.Disable();
            }
            Trial tempTrial;
            trialDict.TryGetValue(id, out tempTrial);
            if (tempTrial != null)
            {
                trialCurrent = tempTrial;
                trialCurrent.StartTrial();
            }
        }

        public static void CancelTrial()
        {
            foreach (Trial trial in trials)
            {
                trial.Disable();
            }
            trialCurrent = null;
        }

        public static bool InTrial()
        {
            foreach (Trial trial in trials)
            {
                if (trial.IsEnabled)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddTrial(Trial trial)
        {
            trial.Init();
            if (!trialDict.ContainsKey(trial.ID))
            {
                trials.Add(trial);
                trialDict.Add(trial.ID, trial);
            }
            Debug.Log("Trials added");
        }

        public static void RemoveTrial(Trial trial)
        {
            if (trialDict.ContainsKey(trial.Name))
            {
                trials.RemoveAt(trials.IndexOf(trial));
                trialDict.Remove(trial.ID);
                //Debug.Log("Removed Trial: " + trial.Name);

            }
        }
    }
}
