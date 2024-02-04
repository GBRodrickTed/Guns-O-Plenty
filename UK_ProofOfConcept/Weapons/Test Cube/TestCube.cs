using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            this.targeter = MonoSingleton<CameraFrustumTargeter>.instance;

            this.wid = base.gameObject.GetComponent<WeaponIdentifier>();
            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();
            this.wpos = this.gameObject.AddComponent<WeaponPos>();
            this.wpos.defaultPos = new Vector3(0.8836f, -0.9345f, 1.4161f);
            this.wpos.middlePos = new Vector3(0f, -0.9345f, 1.4161f);
            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.variationColor = WeaponVariant.RedVariant;
                this.weaponIcon.weaponDescriptor.icon = AssetHandler.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }
            shootPoint = GOPUtils.DescendantByName(base.gameObject, "ShootPoint");
            shootPoint.layer = 0;
            fireArea = GOPUtils.DescendantByName(base.gameObject, "FireArea");
            fireArea.layer = 0;
            fireArea.AddComponent<FireArea>();
            fireAreaCol = fireArea.GetComponent<CapsuleCollider>();
            fireAreaCol.enabled = false;
            ChangeFireAreaSize(20f);
            isFireEpic = ConfigManager.TestCubeEpicFire.value;
            if (isFireEpic)
            {
                firePartObj = Instantiate(EffectBox.epicFire, shootPoint.transform);
            } else
            {
                firePartObj = Instantiate(EffectBox.fire, shootPoint.transform);
            }
            firePart = firePartObj.GetComponent<ParticleSystem>();
            firePartObj.TryGetComponent<Light>(out fireLight);
            TurnFireOn(false);
        }
        public void OnEnable()
        {
            TurnFireOn(false);
            /*MonoSingleton<HMRPlus>.Instance.SendHudMessageEffect(
                "<grad=rainbow>Mobomba</grad> or <grad=fire>Sicko Mode</grad>?"
                , 0, 4f, false, 1 / 15f, UnityEngine.Random.Range(0f, 1f));*/
        }
        public void Update()
        {
            /*Color color = GOPUtils.LerpColorArrayNorm(GOPUtils.RainbowGrad, Time.time);
            Color color2 = GOPUtils.LerpColorArrayNorm(GOPUtils.DarkBlueGrad, Time.time);
            MonoSingleton<HMRPlus>.Instance.SendHudMessage(
                "<color=#" + GOPUtils.ColorToHexString(color) + ">>:) yabadabado</color>\n" +
                "<color=#" + GOPUtils.ColorToHexString(color2) + ">>:( graaahrarara</color>"
                , 0, 0.1f, true);*/

            if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                TurnFireOn(true);
            }

            if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasCanceledThisFrame && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                TurnFireOn(false);
            }

            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                Instantiate<GameObject>(AssetHandler.generator, base.transform.position, Quaternion.identity);
            }

            base.transform.localRotation *= GOPUtils.RandRot(rotSpeed * Time.deltaTime);
        }

        public void TurnFireOn(bool turnOn)
        {
            if (turnOn)
            {
                firePart.Play();
                if (fireLight != null) fireLight.enabled = true;
                fireAreaCol.enabled = true;
            } else
            {
                firePart.Stop();
                if (fireLight != null) fireLight.enabled = false;
                fireAreaCol.enabled = false;
                fireArea.GetComponent<FireArea>().ClearStuff();
            }
        }

        public void ChangeFireAreaSize(float height, float radius = 1f)
        {
            fireAreaCol.height = height;
            fireAreaCol.center = new Vector3(0, height / 2f, 0);
            fireAreaCol.radius = radius;
        }

        private WeaponIcon weaponIcon;
        private WeaponIdentifier wid;
        private WeaponPos wpos;

        private GameObject shootPoint;
        private GameObject firePartObj;
        private ParticleSystem firePart;
        private Light fireLight;
        private CameraFrustumTargeter targeter;

        private GameObject fireArea;
        private CapsuleCollider fireAreaCol;
        public bool isFireEpic;

        CoinShot coinShotComp;
        public float rotSpeed;
        Color color = Color.red;
        List<Color> randGrad = new List<Color>();
    }
    public class TestCube : GOPWeapon
    {
        public override GameObject Asset { get; protected set; }
        public override int Slot { get; set; }
        public override string Name { get; protected set; }
        public override bool IsSetup { get; protected set; }
        public override bool ShouldHave { get; set; }
        public override void Setup()
        {
            Debug.Log("Setting up Test Cube");
            if (Asset == null)
            {
                Asset = AssetHandler.LoadAsset<GameObject>("TestCube Prefab");
            }
            
            if (Asset != null && !IsSetup)
            {
                Name = "Test Cube";
                Slot = ConfigManager.TestCubeSlot.value;
                Asset.AddComponent<TestCubeSoul>();
                foreach (GameObject thing in GOPUtils.DescendantsList(Asset))
                {
                    if (thing.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                    {
                        mr.material.shader = ShaderBox.vertexlit_emissive;
                    }
                }
                IsSetup = true;
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
