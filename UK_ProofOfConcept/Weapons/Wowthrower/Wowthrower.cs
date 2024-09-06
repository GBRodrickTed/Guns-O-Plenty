using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;
using UnityEngine.UIElements;

namespace GunsOPlenty.Weapons
{
    public class WowthrowerSoul : MonoBehaviour
    {
        //TODO: There's an issue where the texture gets a bit fucky from farther away (probably because the uvs are too close together)
        //      Specifically happens on AlwaysOnTop layer
        private void Start()
        {
            //Debug.Log("It begins");
            itbegins = true;
            this.targeter = MonoSingleton<CameraFrustumTargeter>.instance;

            this.wid = base.gameObject.GetComponent<WeaponIdentifier>();
            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();
            this.wpos = this.gameObject.AddComponent<WeaponPos>();
            this.wpos.defaultPos = new Vector3(0.9563f, -1.617f, -0.8131f);
            this.wpos.middlePos = new Vector3(0, -1.9418f, -0.0985f);
            this.anim = GOPUtils.DescendantByName(this.gameObject, "wowthrower").GetComponent<Animator>();
            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.variationColor = WeaponVariant.RedVariant;
                this.weaponIcon.weaponDescriptor.icon = AssetHandler.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }

            shootPoint = GOPUtils.DescendantByName(base.gameObject, "ShootPoint");
            shootPoint.layer = 0;

            gunTipAud = shootPoint.GetComponent<AudioSource>();
            gunTipAud.volume = 1.2f;

            fireAreaObj = GOPUtils.DescendantByName(base.gameObject, "FireArea");
            fireAreaObj.layer = 0;
            fireArea = fireAreaObj.AddComponent<FireArea>();
            fireArea.sourceWeapon = base.gameObject;
            fireAreaCol = fireAreaObj.GetComponent<CapsuleCollider>();
            fireAreaCol.enabled = false;
            
            firePart = GOPUtils.DescendantByName(base.gameObject, "FlameThrower").GetComponent<ParticleSystem>();
            firePartEmissions = firePart.emission;
            firePart.Stop();
            GOPUtils.DescendantByName(base.gameObject, "FlameThrower").TryGetComponent(out firePartRenderer);
            GOPUtils.DescendantByName(base.gameObject, "FlameThrower").TryGetComponent(out fireLight);

            bubblePart = GOPUtils.DescendantByName(base.gameObject, "BubbleThrower").GetComponent<ParticleSystem>();
            bubblePartEmissions = bubblePart.emission;
            bubblePart.Stop();
            GOPUtils.DescendantByName(base.gameObject, "BubbleThrower").TryGetComponent(out bubblePartRenderer);

            fireSimpleMat = AssetHandler.LoadAsset<Material>("fireMat");
            bubbleSimpleMat = AssetHandler.LoadAsset<Material>("bubbleMat");
            fireEpicMat = AssetHandler.LoadAsset<Material>("coolfireMat");
            bubbleEpicMat = AssetHandler.LoadAsset<Material>("coolbubbleMat");

            tehwisporbz = new List<WispOrb>();

            TurnFireOff();
            EpicnessVerification();
        }
        public void Update()
        {
            if (ConfigManager.WowthrowerEpicFire.value != amIEpic)
            {
                amIEpic = ConfigManager.WowthrowerEpicFire.value;
                EpicnessVerification();
            }

            stylefactor = (MonoSingleton<StyleHUD>.Instance.currentMeter + (MonoSingleton<StyleHUD>.Instance.rankIndex * 1500f));
            if (NoWeaponCooldown.NoCooldown)
            {
                stylefactor = 12000f;
            }
            if (!noFirin)
            {
                if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
                {
                    base.Invoke("TurnFireOn", this.wid.delay);
                } else if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && MonoSingleton<GunControl>.Instance.GetComponent<GunControl>().activated && !GameStateManager.Instance.PlayerInputLocked)
                {
                    base.Invoke("BigShoot", this.wid.delay);
                } else if (weFirin)
                {
                    base.Invoke("TurnFireOff", this.wid.delay);
                }
            }

            
        }
        public void SlowUpdate()
        {
            
        }
        public void OnEnable()
        {
            if (itbegins)
            {
                //Debug.Log("Darin Marquette");
                TurnFireOff();
            }
            noFirin = false;
        }
        public void EpicnessVerification()
        {
            if (ConfigManager.WowthrowerEpicFire.value == true)
            {
                firePartRenderer.material = fireEpicMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = Color.white;
                bubblePartRenderer.material = bubbleEpicMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = Color.white;
            }
            else
            {
                firePartRenderer.material = fireSimpleMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = GOPUtils.HexToColor("#FF7600");
                bubblePartRenderer.material = bubbleSimpleMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = GOPUtils.HexToColor("#99E7F8");
            }
        }
        public void CreateWispOrb()
        {
            for (int i = 0; i < tehwisporbz.Count; i++)
            {
                //
                if (tehwisporbz[i] == null)
                {
                    tehwisporbz.RemoveAt(i);
                    i--;
                }
            }
            if (tehwisporbz.Count < wispOrbMax)
            {
                tehwisporbz.Add(Instantiate<GameObject>(AssetHandler.customPrefabs["Wisp Orb"], shootPoint.transform.position + shootPoint.transform.forward, Quaternion.identity).GetComponent<WispOrb>());
            } else
            {
                tehwisporbz[0].Die();
                tehwisporbz.RemoveAt(0);
                tehwisporbz.Add(Instantiate<GameObject>(AssetHandler.customPrefabs["Wisp Orb"], shootPoint.transform.position + shootPoint.transform.forward, Quaternion.identity).GetComponent<WispOrb>());
            }
        }
        public void BigShoot()
        {
            //anim.Play("BigShoot");
            TurnFireOff();
            anim.Rebind();
            anim.Play("Idle");
            anim.SetTrigger("BigFire");
        }
        public void TurnFireOn()
        {
            if (!weFirin)
            {
                anim.Rebind();
                anim.Play("Idle");
                anim.SetTrigger("Fire");
            }
            weFirin = true;

            if (MonoSingleton<UnderwaterController>.Instance.inWater)
            {
                if (!bubblePart.isEmitting)
                {
                    bubblePart.Play();
                    firePart.Stop();
                    
                    gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("BubbleThrowerBubble");
                    gunTipAud.time = UnityEngine.Random.Range(0f, 2.5f);
                    gunTipAud.loop = true;
                    gunTipAud.Play();
                }
                bubblePartEmissions.rateOverTime = 5f * (1 + (stylefactor / 12000f) * 10);
                ChangeFireAreaSize(15f);
            }
            else
            {
                if (!firePart.isEmitting)
                {
                    firePart.Play();
                    bubblePart.Stop();
                    
                    gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("FlameThrowerFire");
                    gunTipAud.time = UnityEngine.Random.Range(0f, 2.5f);
                    gunTipAud.loop = true;
                    gunTipAud.Play();
                }
                firePartEmissions.rateOverTime = 10f * (1 + (stylefactor / 12000f) * 10);
                ChangeFireAreaSize(25f);
            }
            if (fireLight != null) fireLight.enabled = true;
            fireAreaCol.enabled = true;
        }
        public void TurnFireOff()
        {
            if (weFirin)
            {
                anim.Rebind();
                anim.Play("Fire");
                anim.SetTrigger("NoFire");
            }
            weFirin = false;

            firePart.Stop();
            bubblePart.Stop();
            gunTipAud.Stop();
            if (fireLight != null) fireLight.enabled = false;
            fireAreaCol.enabled = false;
            if (fireArea != null)
            {
                fireArea.ClearStuff();
            }
        }
        public void BigShootIsTalking()
        {
            gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("FireFlare");
            gunTipAud.loop = false;
            gunTipAud.Play();
            CreateWispOrb();
            noFirin = true;
        }
        public void BigShootEnd()
        {
            noFirin = false;
        }
        public void ChangeFireAreaSize(float height, float radius = 1f)
        {
            if (fireAreaCol != null)
            {
                fireAreaCol.height = height;
                fireAreaCol.center = new Vector3(0, height / 2f, 0);
                fireAreaCol.radius = radius;
            }
        }
        bool weFirin = false;
        bool noFirin = false;
        private string[] fireNames = // TODO: there must be a better way
            {
            "FlameThrower",
            "EpicThrower",
            "BubbleThrower",
            "EpicBubbleThrower",
            };

        private CameraFrustumTargeter targeter;
        private WeaponIcon weaponIcon;
        private WeaponIdentifier wid;
        private WeaponPos wpos;
        private Animator anim;
        private bool itbegins = false; //Used because OnEnabled runs before Start
        private float stylefactor = 0;
        private bool amIEpic = false;

        private GameObject shootPoint;
        private AudioSource gunTipAud;
        private GameObject fireAreaObj;
        private FireArea fireArea;
        private CapsuleCollider fireAreaCol;
        private ParticleSystem firePart;
        private ParticleSystem.EmissionModule firePartEmissions;
        private Light fireLight;
        private ParticleSystem bubblePart;
        private ParticleSystem.EmissionModule bubblePartEmissions;

        private ParticleSystemRenderer firePartRenderer;
        private ParticleSystemRenderer bubblePartRenderer;

        private Material fireSimpleMat;
        private Material bubbleSimpleMat;
        private Material fireEpicMat;
        private Material bubbleEpicMat;

        private List<WispOrb> tehwisporbz;
        int wispOrbMax = 1; //it's very laggy

    }
    public class Wowthrower : GOPWeapon
    {
        public override GameObject Asset { get; protected set; }
        public override int Slot { get; set; }
        public override void Setup()
        {
            Debug.Log("Setting up Wowthrower");
            ID = "wowthrower";
            if (Asset == null)
            {
                Asset = AssetHandler.LoadAsset<GameObject>("Wowthrower Prefab");
            }

            if (Asset != null && !IsSetup)
            {
                Name = "Wowthrower";
                Slot = ConfigManager.WowthrowerSlot.value;
                Asset.AddComponent<WowthrowerSoul>();
                foreach (GameObject thing in GOPUtils.DescendantsList(Asset))
                {
                    if (thing.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                    {
                        mr.material.shader = ShaderBox.vertexlit_emissive;
                    }
                }
                IsSetup = true;
            }
            else
            {
                Debug.Log("Wowthrower Didn't load");
                Name = "Wowthrower is NULL";
                Slot = -1;
            }
        }

        public override void UpdateConfigSettings()
        {
            ShouldHave = ConfigManager.WowthrowerEnable.value;
            Slot = ConfigManager.WowthrowerSlot.value;
            WeaponHandler.isCheating |= ConfigManager.WowthrowerEnable.value;
        }

        
        /*public override void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }*/
    }
}
