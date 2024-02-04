using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static BoolField TestCubeEnable;
        public static IntField FunGunSlot;
        public static IntField GoldenGunSlot;
        public static IntField TestCubeSlot;
        public static BoolField StartupCheck;

        public static BoolField TestCubeEpicFire;
        public static void Setup()
        {
            ConfigManager.config = PluginConfigurator.Create(PluginInfo.Name, PluginInfo.GUID);
            new ConfigHeader(config.rootPanel, "<color=white>-General Settings-</color>");
            ConfigManager.StartupCheck = new BoolField(ConfigManager.config.rootPanel, "Startup Load Check", "field.startupcheck", true, true);
            new ConfigHeader(config.rootPanel, "<color=white>-Gun Settings-</color>");
            new ConfigHeader(config.rootPanel, "<color=red>Reload Required For Changes To Appear</color>", 14);
            ConfigManager.FunGunEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Funny Gun", "field.fungunenable", true, true);
            ConfigManager.FunGunSlot = new IntField(ConfigManager.config.rootPanel, "Funny Gun Slot", "field.fungunslot", 7, 1, 69, true, true);
            ConfigManager.GoldenGunEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Golden Gun", "field.goldengunenable", true, true);
            ConfigManager.GoldenGunSlot = new IntField(ConfigManager.config.rootPanel, "Golden Gun Slot", "field.goldengunslot", 7, 1, 69, true, true);
            ConfigManager.TestCubeEnable = new BoolField(ConfigManager.config.rootPanel, "Enable Test Cube", "field.testcubeenable", false, true);
            ConfigManager.TestCubeSlot = new IntField(ConfigManager.config.rootPanel, "Test Cube Slot", "field.testcubeslot", 8, 1, 69, true, true);
            ConfigManager.TestCubeEpicFire = new BoolField(ConfigManager.config.rootPanel, "make fire epic", "field.testcubefireisepic", false, true);
        }

        public static void Update()
        {
            ConfigManager.FunGunSlot.hidden = !ConfigManager.FunGunEnable.value;
            ConfigManager.GoldenGunSlot.hidden = !ConfigManager.GoldenGunEnable.value;
            ConfigManager.TestCubeSlot.hidden = !ConfigManager.TestCubeEnable.value;
            ConfigManager.TestCubeEpicFire.hidden = !ConfigManager.TestCubeEnable.value;
        }
    }
}
