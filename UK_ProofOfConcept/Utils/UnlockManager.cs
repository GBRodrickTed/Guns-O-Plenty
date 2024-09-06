using GunsOPlenty.Stuff;
using GunsOPlenty.Trials;
using GunsOPlenty.Trials.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    public static class UnlockManager
    {
        //We want a file with an id that corrosponse to the unlock
        //We want the unlocks to corrospond to the current save file
        //For trials, we want to save the highest difficulty it was beat on
        //Will be similar to how Bepinex handled saving/retreiving values
        //Yep, it's morbin time
        static int currentSaveSlot;
        static Dictionary<string, object> saveDictionary = new Dictionary<string, object>();
        public static Dictionary<string, SaveData<int>> trialsCompletedCount;
        public static Dictionary<string, SaveData<bool>> weaponsUnlocked;
        public static Dictionary<string, SaveData<int>> trialDifs;
        private static bool setup = false;
        public static void Setup()
        {
            trialsCompletedCount = new Dictionary<string, SaveData<int>>();
            trialDifs = new Dictionary<string, SaveData<int>>();
            weaponsUnlocked = new Dictionary<string, SaveData<bool>>();
            currentSaveSlot = GameProgressSaver.currentSlot;
            for (int i = 0; i < TrialManager.trials.Count; i++)
            {
                trialsCompletedCount.Add(TrialManager.trials[i].ID, new SaveData<int>(UnlockSaveFilePath(), "Trials Completed", TrialManager.trials[i].ID, 0));
                trialDifs.Add(TrialManager.trials[i].ID, new SaveData<int>(UnlockSaveFilePath(), "Trial Difficulties", TrialManager.trials[i].ID, -1));
                weaponsUnlocked.Add(WeaponHandler.weapons[i].ID, new SaveData<bool>(UnlockSaveFilePath(), "Weapons Unlocked", WeaponHandler.weapons[i].ID, false));
            }
            GetSave();
            setup = true;
        }
        public static bool IsSetup()
        {
            return setup;
        }
        public static void GetSave()
        {
            currentSaveSlot = GameProgressSaver.currentSlot;
            for (int i = 0; i < TrialManager.trials.Count; i++)
            {
                trialsCompletedCount[TrialManager.trials[i].ID].Rebind(UnlockSaveFilePath(), "Trials Completed", TrialManager.trials[i].ID);
                trialDifs[TrialManager.trials[i].ID].Rebind(UnlockSaveFilePath(), "Trial Difficulties", TrialManager.trials[i].ID);
                weaponsUnlocked[WeaponHandler.weapons[i].ID].Rebind(UnlockSaveFilePath(), "Weapons Unlocked", WeaponHandler.weapons[i].ID);
            }
            if (TrialManager.trialMenu != null)
            {
                TrialManager.trialMenu.UpdateDisplays();
            }
            Debug.Log("Current Slot: " + currentSaveSlot);
        }
        public static string UnlockSaveFilePath()
        {
            return Path.Combine(GOPUtils.ModSaveDir(), "slot"+ currentSaveSlot+".goop");
        }
        public static void TrialCompleted(string trial_id)
        {
            if (trialsCompletedCount.ContainsKey(trial_id))
            {
                Debug.Log("start:" + trialsCompletedCount[trial_id].value);
                trialsCompletedCount[trial_id].SetValue(trialsCompletedCount[trial_id].value + 1);
                Debug.Log("end:" + trialsCompletedCount[trial_id].value);
                if (PrefsManager.instance.GetInt("difficulty") > trialDifs[trial_id].value)
                {
                    trialDifs[trial_id].SetValue(PrefsManager.instance.GetInt("difficulty"));
                }
            }
        }
        public static void WeaponUnlocked(string weapon_id)
        {
            if (weaponsUnlocked.ContainsKey(weapon_id))
            {
                weaponsUnlocked[weapon_id].SetValue(true);
            }
        }
        public static void UnlockAllWeapons()
        {
            foreach(GOPWeapon weapon in WeaponHandler.weapons)
            {
                if (weaponsUnlocked.ContainsKey(weapon.ID))
                {
                    weaponsUnlocked[weapon.ID].SetValue(true);
                }
            }
        }
        [HarmonyPatch(typeof(SaveSlotMenu), nameof(SaveSlotMenu.ConfirmReload))]
        [HarmonyPostfix]
        public static void UpdateSlot(SaveSlotMenu __instance)
        {
            GetSave();
        }
        
    }
}
