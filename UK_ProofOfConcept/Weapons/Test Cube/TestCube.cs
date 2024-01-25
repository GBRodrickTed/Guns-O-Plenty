using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace GunsOPlenty.Weapons
{ 
    public class TestCubeSoul : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("It begins");
            base.transform.localPosition = new Vector3(0.8836f, -0.9345f, 1.4161f); 
            rotSpeed = 10f;
            coinShotPref.AddComponent<CoinShot>();
        }
        public void OnEnable()
        {
            /*MonoSingleton<HMRPlus>.Instance.SendHudMessageEffect(
                "<grad=rainbow>Mobomba</grad> or <grad=fire>Sicko Mode</grad>?"
                , 0, 4f, true, 1 / 15f, UnityEngine.Random.Range(0f, 1f));*/
        }
        public void Update()
        {
            Color color = GOPUtils.LerpColorArrayNorm(GOPUtils.RainbowGrad, Time.time);
            Color color2 = GOPUtils.LerpColorArrayNorm(GOPUtils.DarkBlueGrad, Time.time);
            MonoSingleton<HMRPlus>.Instance.SendHudMessage(
                "<color=#" + GOPUtils.ColorToHexString(color) + ">>:) yabadabado</color>\n" +
                "<color=#" + GOPUtils.ColorToHexString(color2) + ">>:( graaahrarara</color>"
                , 0, 0.1f, true);

            if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                GameObject coin = Instantiate<GameObject>(AssetHandler.coinShot, base.transform.position - new Vector3(0, 0.1f, 0), MonoSingleton<CameraController>.instance.transform.rotation);
            }
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                GameObject rocket = Instantiate<GameObject>(PrefabBox.gutterRocket, base.transform.position, MonoSingleton<CameraController>.instance.transform.rotation);
                rocket.GetComponent<Rigidbody>().AddForce(MonoSingleton<CameraController>.instance.transform.forward * 100f, ForceMode.VelocityChange);
            }
            
            base.transform.localRotation *= GOPUtils.RandRot(rotSpeed * Time.deltaTime);
        }
        GameObject coinShotPref;
        CoinShot coinShotComp;
        public float rotSpeed;
        Color color = Color.red;
        List<Color> randGrad = new List<Color>();
    }
    public class TestCube : GOPWeapon
    {
        public override GameObject Asset { get; protected set; }
        public override int Slot { get; protected set; }
        public override string Name { get; protected set; }
        public override void Setup()
        {
            Asset = AssetHandler.LoadAsset<GameObject>("TestCube Prefab");
            if (Asset != null)
            {
                Name = Asset.name;
                Slot = 8;
                Asset.AddComponent<TestCubeSoul>();
                foreach (GameObject thing in GOPUtils.DescendantsList(Asset))
                {
                    if (thing.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                    {
                        mr.material.shader = ShaderBox.vertexlit_emissive;
                    }
                }
            } else
            {
                Debug.Log("Test Cube Didn't load");
                Name = "Test Cube is NULL";
                Slot = -1;
            }
        }
        /*public override void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }*/
    }
}
