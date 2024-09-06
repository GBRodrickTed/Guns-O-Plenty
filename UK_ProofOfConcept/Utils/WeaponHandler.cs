using GunsOPlenty.Trials;
using GunsOPlenty.Utils;
using GunsOPlenty.Weapons;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GunsOPlenty.Utils.ChargeManager;
using GoldenGun = GunsOPlenty.Weapons.GoldenGun;

namespace GunsOPlenty.Stuff
{
    public static class WeaponHandler
    {
        public static List<GOPWeapon> weapons = new List<GOPWeapon>();
        public static Dictionary<string, GOPWeapon> weaponDict = new Dictionary<string, GOPWeapon>();
        public static GameObject thingy;
        private static GameObject initObj;
        public static bool isCheating = false;
        public static void AddWeapons()
        {
            
        }
        public static void SetupWeapons()
        {
            ChargeManager.Setup();
            Debug.Log("Setting up weapon");
            /*if (ConfigManager.FunGunEnable.value) 
            { 
                GOPWeapon weapon = new FunnyGun();
                weapons.Add(weapon); 
            }
            if (ConfigManager.GoldenGunEnable.value)
            {
                GOPWeapon weapon = new GoldenGun();
                weapons.Add(weapon);
            }
            if (ConfigManager.TestCubeEnable.value)
            {
                GOPWeapon weapon = new TestCube();
                weapons.Add(weapon);
            }*/
            weapons.Add(new FunnyGun());
            weapons.Add(new GoldenGun());
            weapons.Add(new Wowthrower());
            foreach (GOPWeapon weapon in weapons)
            {
                if (!weapon.IsSetup)
                {
                    weapon.Setup();
                    weaponDict.Add(weapon.ID, weapon);
                }
            }
            /*foreach (GOPWeapon weapon in weapons)
            {
                foreach(Component comp in weapon.Asset.GetComponents(typeof(Component)))
                {
                    Debug.Log(weapon.Name + ": " + comp.GetType().Name);
                }
            }*/
        }
        public static void RemoveWeapons()
        {
            //Debug.Log("Removing weapons");
            foreach (GOPWeapon gopWeapon in weapons)
            {
                foreach(var slot in MonoSingleton<GunControl>.Instance.slots)
                {
                    foreach(var weapon in slot)
                    {
                        if (weapon == gopWeapon.LoadedAsset)
                        {
                            //Debug.Log("Removing: " + weapon.name);
                            slot.Remove(weapon);
                        }
                    }
                    //gopWeapon.UnSetup();
                }
            }
            foreach (var slot in MonoSingleton<GunControl>.Instance.slots)
            {
                if (slot.Count == 0)
                {
                    MonoSingleton<GunControl>.Instance.slots.Remove(slot);
                    //Debug.Log("Removing an empty weapon slot");
                }
            }
            weapons.Clear();
        }
        public static void AddWeaponToInventory(GOPWeapon weapon)
        {
            if (!UnlockManager.IsSetup() || !UnlockManager.weaponsUnlocked[weapon.ID].value || TrialManager.InTrial())
            {
                return;
            }
            bool gotit = false;
            foreach (List<GameObject> slot in MonoSingleton<GunControl>.Instance.slots)
            {
                for (int i = slot.Count; i >= 0; i--)
                {
                    if (slot.Contains(weapon.LoadedAsset))
                    {
                        gotit = true;
                        break;
                    }
                }
                if (gotit) break;
            }
            if (gotit) return;
            weapon.Create(MonoSingleton<GunControl>.Instance.transform);
            MonoSingleton<GunControl>.Instance.shud.weaponFreshness.Add(weapon.LoadedAsset, 10f);
            weapon.LoadedAsset.SetActive(false);
            int slots = (MonoSingleton<GunControl>.Instance.slots.Count);
            if (weapon.Slot > slots)
            {
                for (int i = 0; i < (weapon.Slot - slots); i++)
                {
                    MonoSingleton<GunControl>.Instance.slots.Add(new List<GameObject>());
                }
            }
            MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Add(weapon.LoadedAsset);
            MonoSingleton<GunControl>.Instance.UpdateWeaponList(true);
        }
        public static void RemoveWeaponFromInventory(GOPWeapon weapon)
        {
            bool gotit = false;
            foreach (List<GameObject> slot in MonoSingleton<GunControl>.Instance.slots)
            {
                for (int i = slot.Count; i >= 0; i--)
                {
                    if (slot.Contains(weapon.LoadedAsset))
                    {
                        gotit = true;
                        break;
                    }
                }
                if (gotit) break;
            }
            if (!gotit) return;
            MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Remove(weapon.LoadedAsset);
            while (MonoSingleton<GunControl>.Instance.slots[MonoSingleton<GunControl>.Instance.slots.Count - 1].Count <= 0)
            {
                MonoSingleton<GunControl>.Instance.slots.RemoveAt(MonoSingleton<GunControl>.Instance.slots.Count - 1);
            }
            MonoSingleton<GunControl>.Instance.shud.weaponFreshness.Remove(weapon.LoadedAsset);
            GameObject.Destroy(MonoSingleton<GunControl>.Instance.transform.Find(weapon.LoadedAsset.name).gameObject);
            MonoSingleton<GunControl>.Instance.UpdateWeaponList(true);
        }
        public static void AddWeaponsToInventoy()
        {
            isCheating = false;
            foreach (GOPWeapon weapon in weapons)
            {
                weapon.UpdateConfigSettings();
                RemoveWeaponFromInventory(weapon);
                if (weapon.ShouldHave)
                {
                    AddWeaponToInventory(weapon);
                }
                
                //Debug.Log(weapons.Count);
            }
        }

        public static void AddStyle()
        {
            foreach (GOPWeapon weapon in weapons)
            {
                AddToStyleDict(weapon.LoadedAsset);
            }
        }

        public static void AddToStyleDict(GameObject obj)
        {
            if (obj != null && !MonoSingleton<StyleHUD>.Instance.weaponFreshness.ContainsKey(obj))
            {
                MonoSingleton<StyleHUD>.Instance.weaponFreshness.Add(obj, 10f);
                Debug.Log(obj.name + " has been added to dictionary");
            }
            else if (MonoSingleton<StyleHUD>.Instance.weaponFreshness.ContainsKey(obj))
            {
                Debug.Log(obj.name + " is already in dictionary");
            }
            else
            {
                Debug.Log(obj.name + " isn't real");
            }
        }
    }
}
