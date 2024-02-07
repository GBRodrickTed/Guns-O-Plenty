using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;
using UnityEngine.Assertions;

namespace GunsOPlenty.Weapons
{ 
    public class TestCubeSoul : MonoBehaviour
    {
        private void Start()
        {
            //Debug.Log("It begins");
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
            fireArea.GetComponent<FireArea>().sourceWeapon = base.gameObject;
            fireAreaCol = fireArea.GetComponent<CapsuleCollider>();
            fireAreaCol.enabled = false;
            ChangeFireAreaSize(15f);
            isFireEpic = ConfigManager.TestCubeEpicFire.value;
            if (this.wid.delay > 0)
            {
                GameObject oldFlameThrower;
                foreach (string name in fireNames)
                {
                    oldFlameThrower = GOPUtils.DescendantByName(shootPoint, name+"(Clone)");
                    if (oldFlameThrower != null)
                    {
                        Destroy(oldFlameThrower);
                    }
                }
            }
            //note to self: use wav files for seemless looping
            gunTipAud = shootPoint.GetComponent<AudioSource>();
            if (isFireEpic)
            {
                firePartObj = Instantiate(EffectBox.epicFire, shootPoint.transform);
                bubblePartObj = Instantiate(EffectBox.epicBubble, shootPoint.transform);
            } else
            {
                firePartObj = Instantiate(EffectBox.fire, shootPoint.transform);
                bubblePartObj = Instantiate(EffectBox.bubble, shootPoint.transform);
            }
            firePart = firePartObj.GetComponent<ParticleSystem>();
            bubblePart = bubblePartObj.GetComponent<ParticleSystem>();
            firePartObj.TryGetComponent<Light>(out fireLight);
            TurnFireOff();
            this.thingy = null;
        }
        public void OnEnable()
        {
            TurnFireOff();
            if (thingy != null)
            {
                //Debug.Log("UnDoing it");
                thingy.deathTime = 30f;
            }

        }
        public void OnDisable()
        {
            if (thingy != null)
            {
                //Debug.Log("Doing it");
                thingy.deathTime = 5f;
            }
        }
        public void Update()
        {
            /*Color color = GOPUtils.LerpColorArrayNorm(GOPUtils.RainbowGrad, Time.time);
            Color color2 = GOPUtils.LerpColorArrayNorm(GOPUtils.DarkBlueGrad, Time.time);
            MonoSingleton<HMRPlus>.Instance.SendHudMessage(
                "<color=#" + GOPUtils.ColorToHexString(color) + ">>:) yabadabado</color>\n" +
                "<color=#" + GOPUtils.ColorToHexString(color2) + ">>:( graaahrarara</color>"
                , 0, 0.1f, true);*/
            //Debug.Log(isFireOn);
            float stylefactor = (MonoSingleton<StyleHUD>.Instance.currentMeter + (MonoSingleton<StyleHUD>.Instance.rankIndex * 1500f));
            if (NoWeaponCooldown.NoCooldown)
            {
                gunTipAud.pitch = 1.7f;
            }
            else
            {
                gunTipAud.pitch = 0.5f + stylefactor * 0.0001f;
            }
            
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                base.Invoke("TurnFireOn", this.wid.delay);
            } else
            {
                base.Invoke("TurnFireOff", this.wid.delay);
            }

            if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                if (this.wid.delay == 0) base.Invoke("CreateThing", this.wid.delay);

            }
        }

        public void TurnFireOn()
        {
            if (MonoSingleton<UnderwaterController>.Instance.inWater)
            {
                if (!bubblePart.isEmitting)
                {
                    bubblePart.Play();
                    firePart.Stop();
                    gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("BubbleThrowerBubble");
                    gunTipAud.time = UnityEngine.Random.Range(0f, 2.5f);
                    gunTipAud.Play();
                }
                ChangeFireAreaSize(15f);
            }
            else
            {
                if (!firePart.isEmitting)
                {
                    //Debug.Log("Wing");
                    firePart.Play();
                    bubblePart.Stop();
                    gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("FlameThrowerFire");
                    gunTipAud.time = UnityEngine.Random.Range(0f, 2.5f);
                    gunTipAud.Play();
                }
                ChangeFireAreaSize(25f);
            }
            isFireOn = true;
            if (fireLight != null) fireLight.enabled = true;
            fireAreaCol.enabled = true;
        }

        public void TurnFireOff()
        {
            firePart.Stop();
            bubblePart.Stop();
            gunTipAud.Stop();
            isFireOn = false;
            if (fireLight != null) fireLight.enabled = false;
            fireAreaCol.enabled = false;
            fireArea.GetComponent<FireArea>().ClearStuff();
        }

        public void CreateThing()
        {
            //Instantiate<GameObject>(AssetHandler.beething, base.transform.position, Quaternion.identity);
            if (this.thingy == null)
            {
                thingyObj = Instantiate<GameObject>(AssetHandler.generator, base.transform.position, Quaternion.identity);
                thingyObj.TryGetComponent<Generator>(out thingy);
                thingy.sourceWeapon = base.gameObject;
            }
            else
            {
                Instantiate<GameObject>(PrefabBox.harmlessBoom, thingy.transform.position, Quaternion.identity);
                thingy.Die();
                thingy = null;
                Destroy(thingyObj);
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
        private bool isFireOn;
        private GameObject firePartObj;
        private GameObject bubblePartObj;
        private ParticleSystem firePart;
        private ParticleSystem bubblePart;
        private Light fireLight;
        private CameraFrustumTargeter targeter;
        private AudioSource gunTipAud;

        private GameObject thingyObj;
        private Generator thingy;

        private string[] fireNames = // TODO: there must be a better way
            {
            "FlameThrower",
            "EpicThrower",
            "BubbleThrower",
            "EpicBubbleThrower",
            };
        private GameObject fireArea;
        private CapsuleCollider fireAreaCol;
        public bool isFireEpic;

        CoinShot coinShotComp;
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
