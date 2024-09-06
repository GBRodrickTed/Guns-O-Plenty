 using Discord;
using GunsOPlenty.Trials.UI;
using GunsOPlenty.Utils;
using GunsOPlenty.Weapons;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR;

namespace GunsOPlenty.Data
{
    public static class AssetHandler
    {
        public static AssetBundle bundle;
        public static bool bundleLoaded = false;
        public static void LoadBundle()
        {
            if (bundleLoaded)
            {
                Debug.Log("Bundle already loaded");
                return;
            }
            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(GOPUtils.ModDir(), "Data"), "gop_assets"));
            if (bundle != null )
            {
                Debug.Log("Bundle successfully loaded");
                bundleLoaded = true;
            } else
            {
                Debug.Log("Bundle failed to loaded");
                bundleLoaded = false;
            }
        }

        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            if (!bundleLoaded)
            {
                Debug.Log("Bundle is not loaded");
                return null;
            }

            T asset = bundle.LoadAsset<T>(name);
            if (asset == null)
            {
                Debug.Log(name + " didn't load");
            }
            return asset;
        }

        public static void UnloadBundle()
        {
            if (!bundleLoaded)
            {
                Debug.Log("Bundle already unloaded");
                return;
            }
            Debug.Log("Bundle succesfully unloaded");
            bundle.Unload(true);
            bundle = null;
        }

        public static void CreateCustomPrefabs()
        {
            coinShot = LoadAsset<GameObject>("Testo");
            coinShot.name = "Coin Shot";
            coinShot.AddComponent<CoinShot>();

            LayerMask testLM = 0;

            generator = LoadAsset<GameObject>("Test Thing");
            //generator.AddComponent<Generator>();
            foreach (GameObject thing in GOPUtils.DescendantsList(generator))
            {
                MeshRenderer meshR;
                if (thing.TryGetComponent<MeshRenderer>(out meshR))
                {
                    meshR.material.shader = ShaderBox.vertexlit;
                    thing.AddComponent<OutdoorsChecker>();
                }
            }

            beething = LoadAsset<GameObject>("Test Creature");
            //beething.AddComponent<BeeThing>();
            foreach (GameObject thing in GOPUtils.DescendantsList(beething))
            {
                MeshRenderer meshR;
                if (thing.TryGetComponent<MeshRenderer>(out meshR))
                {
                    meshR.material.shader = ShaderBox.vertexlit;
                    thing.AddComponent<OutdoorsChecker>();
                }
            }
            //generator.GetComponent<Generator>().fred = beething;
            EffectBox.fire = LoadAsset<GameObject>("FlameThrower");
            EffectBox.epicFire = LoadAsset<GameObject>("EpicThrower");
            EffectBox.bubble = LoadAsset<GameObject>("BubbleThrower");
            EffectBox.epicBubble = LoadAsset<GameObject>("EpicBubbleThrower");

            GameObject tempPrefab;
            tempPrefab = LoadAsset<GameObject>("Unlock Message");
            tempPrefab.AddComponent<AchievementPopup>();
            customPrefabs.Add(tempPrefab.name, tempPrefab);

            tempPrefab = LoadAsset<GameObject>("Wisp Orb");
            MeshRenderer tempMeshR;
            if (tempPrefab.TryGetComponent<MeshRenderer>(out tempMeshR))
            {
                tempMeshR.material.shader = ShaderBox.unlit_alphatest_nocull;
            }
            tempPrefab.AddComponent<WispOrb>();
            tempPrefab.AddComponent<WaterCheck>();
            customPrefabs.Add(tempPrefab.name, tempPrefab);

            tempPrefab = LoadAsset<GameObject>("Wisp Baby");
            tempPrefab.AddComponent<WispBuddy>();
            tempPrefab.AddComponent<WaterCheck>();
            customPrefabs.Add(tempPrefab.name, tempPrefab);
        }
        public static Dictionary<string, GameObject> customPrefabs = new Dictionary<string, GameObject>(); // TODO: Fully switch to this;
        public static GameObject coinShot;
        public static GameObject generator;
        public static GameObject beething;

    }

    public static class PrefabBox
    {
        public static GameObject boom = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion.prefab").WaitForCompletion();
        public static GameObject bigBoom = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Big.prefab").WaitForCompletion();
        public static GameObject superBoom = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Super.prefab").WaitForCompletion();
        public static GameObject harmlessBoom = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Rocket Harmless.prefab").WaitForCompletion();
        public static GameObject beam = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam.prefab").WaitForCompletion();
        //public static GameObject beamSharp = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam Sharp.prefab").WaitForCompletion();
        //public static GameObject beamSharpSuper = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam Sharp Alternative.prefab").WaitForCompletion();
        public static GameObject coin = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Coin.prefab").WaitForCompletion();
        public static GameObject grenade = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Grenade.prefab").WaitForCompletion();
        public static GameObject rocket = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Rocket.prefab").WaitForCompletion();
        public static GameObject cannonball = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Cannonball.prefab").WaitForCompletion();
        public static GameObject progHomingExpl = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Homing Explosive.prefab").WaitForCompletion();
        public static GameObject gutterbeam = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Gutterman Beam.prefab").WaitForCompletion();
        public static GameObject gutterRocket = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/RocketEnemy.prefab").WaitForCompletion();
    }
    public static class ShaderBox
    {
        public static Shader vertexlit = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader").WaitForCompletion();
        public static Shader unlit_ambient = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit-ambient.shader").WaitForCompletion();
        public static Shader vertexlit_emissive = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
        public static Shader unlit_alphatest_cull = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/AlphaTest/ULTRAKILL-unlit-alphatest-cull.shader").WaitForCompletion();
        public static Shader unlit_alphatest_nocull = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/AlphaTest/ULTRAKILL-unlit-alphatest-nocull.shader").WaitForCompletion();
        public static Shader vertexlit_alphatest_nocull = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/AlphaTest/ULTRAKILL-vertexlit-alphatest-nocull.shader").WaitForCompletion();
        
    }

    public static class EffectBox // no longer needed :D (for now)
    {
        public static GameObject fire;
        public static GameObject epicFire;
        public static GameObject bubble;
        public static GameObject epicBubble;
    }

    public static class WhyDoesGettingAddressableNamesAtRuntimeHaveToBeSoHard
    {
        //it works, my life is complete
        public static async void GetAddressKeys()
        {
            /*AsyncOperationHandle<IResourceLocator> thingy = Addressables.LoadContentCatalogAsync(GOPUtils.GameDirectory() + "\\ULTRAKILL_Data\\StreamingAssets\\aa\\catalog.json");
            await thingy.Task;
            List<string> addressKeys = new List<string>();
            IResourceLocator resLocator = thingy.Result;
            if (resLocator != null)
            if (resLocator != null)
            {
                Debug.Log("YEAAAAAAH");
                foreach (var key in resLocator.Keys)
                {
                    addressKeys.Add(key.ToString());
                    Debug.Log(key.ToString());
                }
                File.WriteAllLines(GOPUtils.ModDirectory() + "\\addr_catalog.txt", addressKeys);
            } else
            {
                Debug.Log("Aw fiddlesticks");
            }*/
            
        }
    }
}
