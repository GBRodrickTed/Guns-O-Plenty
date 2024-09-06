using GunsOPlenty.Data;
using GunsOPlenty.Trials.UI;
using GunsOPlenty.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GunsOPlenty.Trials
{
    public class Stylin : Trial
    {
        static Stylin instance;
        public override string Name { get; protected set; }
        public override string ID { get; protected set; }
        public override string GUID { get; protected set; }
        public override Sprite Icon { get; protected set; }
        public override List<string> Tribulations { get; protected set; }
        public override List<string> TipsAndTricks { get; protected set; }
        public override List<string> Reward { get; protected set; }
        public override bool IsEnabled { get; protected set; }
        Harmony harmony;
        UnityEvent styleRankChange;
        int styleRank;
        string level;
        public bool shouldHaveTrial = false;
        public bool levelStarted = false;
        public bool styleThresholdReached = false;
        public bool styleDeadlineMet;
        public bool styleCritical;
        public float styleCriticalTiemr;
        public int rankRequired = 4; // 0 - D, 1 - C, 2 - B, 3 - A, 4 - S, 5 - SS, 6 - SSS, 7 - United Kingdom
        public bool levelCompleted = false;
        public bool itsover = false;
        public float lamenessTime = 1.5f;
        public override void Init()
        {
            instance = this;
            Name = "Stylin";
            ID = "stylin";
            GUID = (PluginInfo.TrialGUID + "." + ID);
            Icon = AssetHandler.LoadAsset<Sprite>("stylin_icon");
            Tribulations = new List<string> {
                "Beat level 2-2",
                "Get a style meter rank of S in 30 seconds or less",
                "Once getting a style meter rank of S, mantain it to the end of the level",
                "Finish the level with a P rank"
            };
            TipsAndTricks = new List<string> {
                "For the second tribulation, marksman is your friend"
            };
            FunFacts = new List<string>
            {
                "The \"Wow\" in Wowthrower stands for \"Will O' the Wisp\"",
                "If you listen to the Wowthrower closely, you can hear the screams of the damned"
            };
            Reward = new List<string> {
                "Wowthrower"
            };
            IsEnabled = false;
            level = "Level 2-2";
            harmony = new Harmony(GUID);
        }
        public override void StartTrial()
        {
            shouldHaveTrial = true;
            SceneHelper.LoadScene(level);
            Enable();
        }
        public override void Enable()
        {
            if (IsEnabled == false)
            {
                IsEnabled = true;
                harmony.PatchAll(this.GetType());
            }
        }

        public override void Disable()
        {
            if (IsEnabled == true)
            {
                IsEnabled = false;
                harmony.UnpatchSelf();
                SetupTrial();
            }
        }
        public void YouMustDIE()
        {
            if (!itsover)
            {
                itsover = false;
                StartTrial();
            }
        }
        public void SetupTrial()
        {
            styleRankChange = new UnityEvent();
            styleRankChange.AddListener(StyleStuff);
            levelStarted = true;
            styleRank = 0;
            styleCriticalTiemr = 0;
            styleDeadlineMet = false;
            levelCompleted = false;
            itsover = false;
        }
        void StyleStuff()
        {
            if (MonoSingleton<StyleHUD>.Instance.rankIndex < instance.rankRequired && instance.styleDeadlineMet && instance.levelCompleted == false)
            {
                MonoSingleton<HMRPlus>.Instance.SendHudMessageEffect(
                        "<grad=fire>Warning: Critical Lameness!</grad>"
                        , 0, lamenessTime, false, 5f);
            } else
            {
                MonoSingleton<HMRPlus>.Instance.Done();
            }
        }
        [HarmonyPatch(typeof(LevelStatsEnabler), nameof(LevelStatsEnabler.Start))]
        [HarmonyPrefix]
        public static void Crash(LevelStatsEnabler __instance)
        {
            instance.SetupTrial();
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Update))]
        [HarmonyPrefix]
        public static void Crash1(StatsManager __instance)
        {
            if (instance.styleRank != MonoSingleton<StyleHUD>.Instance.rankIndex)
            {
                instance.styleRank = MonoSingleton<StyleHUD>.Instance.rankIndex;
                instance.styleRankChange.Invoke();
            }

            if (MonoSingleton<StyleHUD>.Instance.rankIndex < instance.rankRequired && instance.styleDeadlineMet && instance.levelCompleted == false)
            {
                instance.styleCriticalTiemr += Time.deltaTime;
                
                if (instance.styleCriticalTiemr > instance.lamenessTime || MonoSingleton<StyleHUD>.Instance.rankIndex < (instance.rankRequired - 1))
                {
                    instance.styleCriticalTiemr = 0;
                    //Debug.Log("boi vionda: "+instance.styleCriticalTiemr);
                    instance.YouMustDIE();
                }
            } else
            {
                instance.styleCriticalTiemr = 0;
            }
            

            if (MonoSingleton<StyleHUD>.Instance.rankIndex < instance.rankRequired)
            {
                if (StatsManager.instance.seconds > 30 && instance.styleDeadlineMet == false)
                {
                    instance.YouMustDIE();
                }
            }
            else
            {
                instance.styleDeadlineMet = true;
            }
            
        }

        //corolates to the exact moment you enter the exit in this specific level
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.StopTimer))]
        [HarmonyPrefix]
        public static void LevelComplete(StatsManager __instance)
        {
            instance.levelCompleted = true;
            
        }
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.SendInfo))]
        [HarmonyPostfix]
        public static void RankCheck(StatsManager __instance)
        {
            if (__instance.rankScore >= 12 && !__instance.asscon.cheatsEnabled && !__instance.majorUsed)
            {
                Debug.Log(instance.Name + " Completed");
                UnlockManager.TrialCompleted(instance.ID);
                string weaponID = "wowthrower";
                if (UnlockManager.weaponsUnlocked.ContainsKey(weaponID))
                {
                    if (UnlockManager.weaponsUnlocked[weaponID].value == false)
                    {
                        PopupManager.Instance.CreatePopup("Wowthrower Unlocked");
                        UnlockManager.WeaponUnlocked(weaponID);
                    }
                }
            }
            else
            {
                Debug.Log(instance.Name + " Failed");
            }
        }

        [HarmonyPatch(typeof(OptionsManager), nameof(OptionsManager.QuitMission))]
        [HarmonyPrefix]
        public static void QuitCheck(OptionsManager __instance)
        {
            instance.Disable();
        }
        [HarmonyPatch(typeof(SceneHelper), nameof(SceneHelper.LoadScene))]
        [HarmonyPrefix]
        public static bool LevelChangeCheck(SceneHelper __instance)
        {
            if (instance.shouldHaveTrial)
            {
                instance.shouldHaveTrial = false;
            }
            instance.Disable();

            instance.levelStarted = false;
            return true;
        }
    }
}
