using GunsOPlenty.Stuff;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;

namespace GunsOPlenty.Utils
{
    public static class ConfigManager
    {
        public static PluginConfigurator config;
        public static BoolField FunGunEnable;
        public static BoolField GoldenGunEnable;
        public static BoolField WowthrowerEnable;
        public static IntField FunGunSlot;
        public static IntField GoldenGunSlot;
        public static IntField WowthrowerSlot;
        public static BoolField StartupCheck;

        public static BoolField WowthrowerEpicFire;

        public static void Setup()
        {
            ConfigManager.config = PluginConfigurator.Create(PluginInfo.Name, PluginInfo.GUID);
            new ConfigHeader(config.rootPanel, "<color=white>-General Settings-</color>");
            ConfigManager.StartupCheck = new BoolField(ConfigManager.config.rootPanel, "Startup Load Check", "field.startupcheck", true, true);
            new ConfigHeader(config.rootPanel, "<color=white>-Gun Settings-</color>");
            //new ConfigHeader(config.rootPanel, "<color=red>Mission Reload Required For Changes To Appear</color>", 14);
            ConfigManager.FunGunEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Funny Gun", "field.fungunenable", true, true);
            ConfigManager.FunGunSlot = new IntField(ConfigManager.config.rootPanel, "Funny Gun Slot", "field.fungunslot", 7, 1, 69, true, true); // this isn't me being funny this is r/funiswedishgamedev being funny
            ConfigManager.GoldenGunEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Golden Gun", "field.goldengunenable", true, true);
            ConfigManager.GoldenGunSlot = new IntField(ConfigManager.config.rootPanel, "Golden Gun Slot", "field.goldengunslot", 7, 1, 69, true, true);
            ConfigManager.WowthrowerEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Wowthrower", "field.wowthrowerenable", true, true);
            ConfigManager.WowthrowerSlot = new IntField(ConfigManager.config.rootPanel, "Wowthrower Slot", "field.wowthrowerslot", 8, 1, 69, true, true);
            ConfigManager.WowthrowerEpicFire = new BoolField(ConfigManager.config.rootPanel, "make fire epic", "field.wowthrowerfireisepic", false, true);

            ConfigManager.FunGunEnable.onValueChange += (e) =>
            {
                ConfigManager.FunGunSlot.hidden = !e.value;
                if (Plugin.IsInLevel())
                {
                    if (e.value)
                    {
                        WeaponHandler.AddWeaponToInventory(WeaponHandler.weaponDict["funny_gun"]);
                    } else
                    {
                        WeaponHandler.RemoveWeaponFromInventory(WeaponHandler.weaponDict["funny_gun"]);
                    }
                }
                
            };

            ConfigManager.GoldenGunEnable.onValueChange += (e) =>
            {
                ConfigManager.GoldenGunSlot.hidden = !e.value;
                if (Plugin.IsInLevel())
                {
                    if (e.value)
                    {
                        WeaponHandler.AddWeaponToInventory(WeaponHandler.weaponDict["golden_shotgun"]);
                    }
                    else
                    {
                        WeaponHandler.RemoveWeaponFromInventory(WeaponHandler.weaponDict["golden_shotgun"]);
                    }
                }
            };

            ConfigManager.WowthrowerEnable.onValueChange += (e) =>
            {
                ConfigManager.WowthrowerSlot.hidden = !e.value;
                ConfigManager.WowthrowerEpicFire.hidden = !e.value;
                if (Plugin.IsInLevel())
                {
                    if (e.value)
                    {
                        WeaponHandler.AddWeaponToInventory(WeaponHandler.weaponDict["wowthrower"]);
                    }
                    else
                    {
                        WeaponHandler.RemoveWeaponFromInventory(WeaponHandler.weaponDict["wowthrower"]);
                    }
                }
            };

            ConfigManager.FunGunEnable.TriggerValueChangeEvent();
            ConfigManager.GoldenGunEnable.TriggerValueChangeEvent();
            ConfigManager.WowthrowerEnable.TriggerValueChangeEvent();

            string workingDirectory = GOPUtils.ModDir();
            string iconFilePath = Path.Combine(Path.Combine(workingDirectory, "Data"), "icon.png");
            ConfigManager.config.SetIconWithURL("file://" + iconFilePath);
        }

        public static bool CheatCheck()
        {
            //TODO: Make this dynamic
            return (ConfigManager.FunGunEnable.value | ConfigManager.GoldenGunEnable.value | ConfigManager.WowthrowerEnable.value);
        }
    }
}
