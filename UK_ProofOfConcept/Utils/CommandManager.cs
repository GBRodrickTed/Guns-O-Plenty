using BepInEx.Logging;
using GameConsole;
using GameConsole.Commands;
using GunsOPlenty.Stuff;
using GunsOPlenty.Trials;
using GunsOPlenty.Trials.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunsOPlenty.Utils
{
    public static class CommandManager
    {
        public static void RegisterAll()
        {
            GameConsole.Console.Instance.RegisterCommand(new GOPCommand());
        }
        public class GOPCommand : ICommand
        {
            public string Name
            {
                get
                {
                    return "GOP Command";
                }
            }

            public string Description
            {
                get
                {
                    return "Runs commands for Guns O' Plenty";
                }
            }
            public string Command
            {
                get
                {
                    return "gop";
                }
            }

            public void Execute(GameConsole.Console console, string[] args)
            {
                switch(args[0])
                {
                    case "hi":
                        UnityEngine.Debug.Log("hello");
                        break;
                    case "level":
                        SceneHelper.LoadScene("Level "+ args[1], true);
                        if (args.Length >= 3)
                        {
                            int difficulty = 2;
                            if (Int32.TryParse(args[2], out difficulty))
                            {
                                PrefsManager.Instance.SetInt("difficulty", difficulty);
                            } else
                            {
                                UnityEngine.Debug.Log("Invalid difficulty");
                                PrefsManager.Instance.SetInt("difficulty", 2);
                            }
                        }
                        break;
                    case "idk":
                        UnityEngine.Debug.Log("bruh");
                        break;
                    case "trial":
                        Trial trial;
                        TrialManager.trialDict.TryGetValue(args[1], out trial);
                        if (trial == null)
                        {
                            UnityEngine.Debug.Log("Could not find trial with ID: " + args[1]);
                            break;
                        }
                        else
                        {
                            if (args[2] == "start")
                            {
                                trial.StartTrial();
                            }
                            else if (args[2] == "disable")
                            {
                                trial.Disable();
                            }
                        }
                        
                        break;
                    case "unlockallweapons":
                        switch(args[1])
                        {
                            case "NOOOOOOOOOW!!!!1!":
                            case "plz":
                            case "please":
                                bool somethingcameinthemailtoday = false;
                                foreach(GOPWeapon weapon in WeaponHandler.weapons)
                                {
                                    if (UnlockManager.weaponsUnlocked.ContainsKey(weapon.ID))
                                    {
                                        if (UnlockManager.weaponsUnlocked[weapon.ID].value == false)
                                        {
                                            somethingcameinthemailtoday = true;
                                            PopupManager.Instance.CreatePopup(weapon.Name + " Unlocked");
                                            UnlockManager.WeaponUnlocked(weapon.ID);
                                        }
                                    }
                                }
                                if (!somethingcameinthemailtoday)
                                {
                                    UnityEngine.Debug.Log("All the weapons are already unlocked dummy :)");
                                }
                                break;
                            case null:
                                UnityEngine.Debug.Log("What's the magic word.");
                                break;
                            default:
                                UnityEngine.Debug.Log("I don't believe that's the magic word.");
                                break;
                        }
                        break;
                }
            }
        }
    }
}
