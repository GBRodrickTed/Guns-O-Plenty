using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Diagnostics;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static UnityEngine.ResourceManagement.Util.BinaryStorageBuffer.TypeSerializer;

namespace GunsOPlenty.Stuff
{
    public static class TrashCan
    {
        public static AssetBundle bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GunsOPlenty.assets.gop_assets"));
    }
    public static class PrefabBox
    {
        public static GameObject beam = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam.prefab").WaitForCompletion();
        public static GameObject beamSharp = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam Sharp.prefab").WaitForCompletion();
        public static GameObject beamSharpSuper = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Revolver Beam Sharp Alternative.prefab").WaitForCompletion();
        public static GameObject coin = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Coin.prefab").WaitForCompletion();
        public static GameObject grenade = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Grenade.prefab").WaitForCompletion();
        public static GameObject rocket = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Rocket.prefab").WaitForCompletion();
        public static GameObject cannonball = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Cannonball.prefab").WaitForCompletion();
        public static GameObject progHomingExpl = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Homing Explosive.prefab").WaitForCompletion();
        public static GameObject gutterbeam = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Hitscan Beams/Gutterman Beam.prefab").WaitForCompletion();
        
    }
    public static class ShaderBox
    {
        public static Shader vertexlit = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit.shader").WaitForCompletion();
        public static Shader psx_abient = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-unlit-ambient.shader").WaitForCompletion();
        public static Shader vertexlit_emissive = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
    }

    public static class WhyDoesGettingAddressableNamesAtRuntimeHaveToBeSoHard
    {
        //it works, my life is complete
        public static async void GetAddressKeys()
        {
            AsyncOperationHandle<IResourceLocator> thingy = Addressables.LoadContentCatalogAsync(GOPUtils.GameDirectory() + "\\ULTRAKILL_Data\\StreamingAssets\\aa\\catalog.json");
            await thingy.Task;
            List<string> addressKeys = new List<string>();
            IResourceLocator resLocator = thingy.Result;
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
            }
            
        }
    }
}
