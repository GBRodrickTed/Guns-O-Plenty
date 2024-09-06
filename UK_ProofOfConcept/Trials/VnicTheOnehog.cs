using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GunsOPlenty.Stuff;
using System.Runtime.CompilerServices;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using GunsOPlenty.Trials.UI;

namespace GunsOPlenty.Trials
{
    public class VnicTheOnehog : Trial
    {
        static VnicTheOnehog instance;
        public override string Name { get; protected set; }
        public override string ID { get; protected set; }
        public override string GUID { get; protected set; }
        public override Sprite Icon { get; protected set; }
        public override List<string> Tribulations { get; protected set; }
        public override List<string> TipsAndTricks { get; protected set; }
        public override List<string> Reward { get; protected set; }
        public override bool IsEnabled { get; protected set; }
        Harmony harmony;
        string level;
        public bool shouldHaveTrial = false;
        public bool levelStarted = false;
        public bool levelCompleted = false;
        public override void Init()
        {
            instance = this;
            Name = "Vnic The Onehog";
            ID = "vnic_the_onehog";
            GUID = (PluginInfo.TrialGUID + "." + ID);
            Icon = AssetHandler.LoadAsset<Sprite>("vnic_the_onehog_icon");
            Tribulations = new List<string> {
                "Beat level 0-1",
                "Beat the level in Crash Mode",
                "Beat the level in 1 minute 30 seconds or less"
            };
            TipsAndTricks = new List<string>();
            FunFacts = new List<string> {
                "This mod was specifically made for the Golden Shotgun",
                "This mod's true name is \"UKProofOfConcept\"" // because idk how to change visual studio project names
            };
            Reward = new List<string> {
                "Golden Shotgun"
            };
            IsEnabled = false;
            level = "Level 0-1";
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
            }
        }
        [HarmonyPatch(typeof(LevelStatsEnabler), nameof(LevelStatsEnabler.Start))]
        [HarmonyPrefix]
        public static void Crash(LevelStatsEnabler __instance)
        {
            instance.levelStarted = true;
            MonoSingleton<PlayerTracker>.Instance.ChangeToPlatformer();
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.StopTimer))]
        [HarmonyPrefix]
        public static void LevelComplete(StatsManager __instance)
        {
            if (!instance.levelCompleted && !__instance.asscon.cheatsEnabled && !__instance.majorUsed) // this can call twice specifically on harmless for some unknown reason
            {
                instance.levelCompleted = true;
                UnlockManager.TrialCompleted(instance.ID);
                Debug.Log(instance.Name + " Completed");
                string weaponID = "golden_shotgun";
                //TODO: Should probably turn this into a universal function
                if (UnlockManager.weaponsUnlocked.ContainsKey(weaponID))
                {
                    if (UnlockManager.weaponsUnlocked[weaponID].value == false)
                    {
                        UnlockManager.WeaponUnlocked(weaponID);
                        PopupManager.Instance.CreatePopup("Golden Shotgun Unlocked");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Update))]
        [HarmonyPrefix]
        public static void Crash1(StatsManager __instance)
        {
            if (__instance.seconds > 90)
            {
                instance.StartTrial();
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
