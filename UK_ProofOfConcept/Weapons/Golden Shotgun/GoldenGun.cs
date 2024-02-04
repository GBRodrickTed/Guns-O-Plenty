using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;

namespace GunsOPlenty.Weapons
{
    public class GoldenGunSoul : MonoBehaviour
    {
        private void Start()
        {
            
            this.cam = MonoSingleton<CameraController>.Instance.GetComponent<Camera>();
            this.gc = MonoSingleton<GunControl>.Instance.GetComponent<GunControl>();
            this.camObj = this.cam.gameObject;
            this.cc = MonoSingleton<CameraController>.Instance;
            this.coinShot = AssetHandler.coinShot;
            this.coinShotComp = coinShot.GetComponent<CoinShot>();
            this.coinShotComp.sourceWeapon = base.gameObject;
            this.coinShotComp.ignoreCoins = true;
            this.gunTip = GOPUtils.DescendantByName(this.gameObject, "ShootPoint");
            this.gunTipAud = gunTip.GetComponent<AudioSource>();
            this.pumpAud = GOPUtils.DescendantByName(this.gameObject, "Shotgun_Pump").GetComponent<AudioSource>();
            this.triggerAud = GOPUtils.DescendantByName(this.gameObject, "Shotgun_Trigger").GetComponent<AudioSource>();
            this.targeter = MonoSingleton<CameraFrustumTargeter>.instance;
            this.anim = GOPUtils.DescendantByName(this.gameObject, "golden_shotgun").GetComponent<Animator>();

            this.wid = base.gameObject.GetComponent<WeaponIdentifier>();
            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();
            this.wpos = this.gameObject.AddComponent<WeaponPos>();
            this.wpos.defaultPos = new Vector3(0.6129f, -1.5981f, 1.5654f);//0.7074 -1.4545 1.6654
            this.wpos.middlePos = new Vector3(-0.08f, -1.5981f, 1.5654f);
            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.variationColor = WeaponVariant.GoldVariant;
                this.weaponIcon.weaponDescriptor.icon = AssetHandler.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }

        }

        private void Update()
        {
            //this.transform.localPosition = new Vector3(0.50f, -1.25f, 0.50f);

            if (NoWeaponCooldown.NoCooldown)
            {
                shouldFire1 = MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed;
                shouldFire2 = MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed;
            }
            else
            {
                shouldFire1 = MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame;
                shouldFire2 = MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame;
            }

            if (shouldFire1 && this.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                base.Invoke("Shoot", this.wid.delay);
            }

            if (shouldFire2 && this.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                base.Invoke("Pump", this.wid.delay);
            }
        }

        public void Shoot()
        {
            if (gunReady)
            {
                gunReady = false;
                triggerAud.clip = AssetHandler.LoadAsset<AudioClip>("Click");
                triggerAud.Play();
                if (pumps > 0)
                {
                    anim.SetTrigger("Shoot");
                    gunTipAud.clip = AssetHandler.LoadAsset<AudioClip>("ShotgunBoom");
                    gunTipAud.pitch = Random.Range(1.4f, 1.6f);
                    gunTipAud.Play();
                    switch (pumps)
                    {
                        case 1:
                            shots = 15;
                            spread = 20;
                            this.coinShotComp.power = 1f;
                            break;
                        case 2:
                            shots = 10;
                            spread = 10;
                            this.coinShotComp.power = 1.5f;
                            break;
                        case 3:
                            shots = 5;
                            spread = 5;
                            this.coinShotComp.power = 3f;
                            break;
                        case 4:
                            shots = 1;
                            spread = 0;
                            this.coinShotComp.power = 15f;
                            break;
                        default:
                            shots = 1;
                            spread = 0;
                            this.coinShotComp.power = 15 + pumps - 4;
                            break;
                    }
                    for (int i = 0; i < shots; i++)
                    {
                        GameObject coinBeam = Instantiate<GameObject>(coinShot, this.cam.transform.position + this.cam.transform.forward * 0.1f - this.cam.transform.up * 0.1f, (this.cam.transform.rotation) * GOPUtils.RandRot(spread));
                        if (this.targeter.CurrentTarget && this.targeter.IsAutoAimed)
                        {
                            coinBeam.transform.LookAt(this.targeter.CurrentTarget.bounds.center);
                            coinBeam.transform.rotation *= GOPUtils.RandRot(spread);
                        }
                    }
                    pumps = 0;
                    this.coinShotComp.power = 1;
                }
                else
                {
                    anim.SetTrigger("EmptyShoot");
                }
            }
        }

        public void Pump()
        {
            if (gunReady)
            {
                gunReady = false;
                if (pumps >= 3)
                {
                    anim.SetTrigger("BigReload");
                }
                else
                {
                    anim.SetTrigger("Reload");
                }
            }
        }

        public void ReadyGun()
        {
            gunReady = true;
        }

        public void PumpGun()
        {
            if (pumps >= 3)
            {
                pumpAud.clip = AssetHandler.LoadAsset<AudioClip>("BigCha");
                pumpAud.pitch = 1f;
                pumpAud.Play();
            }
            else
            {
                pumpAud.clip = AssetHandler.LoadAsset<AudioClip>("Cha");
                pumpAud.pitch = 1f - pumps * 0.08f;
                pumpAud.Play();
            }
        }

        public void UnpumpGun()
        {
            if (pumps >= 3)
            {
                pumpAud.clip = AssetHandler.LoadAsset<AudioClip>("BigChing");
                pumpAud.pitch = 1f;
                pumpAud.Play();
            }
            else
            {
                pumpAud.clip = AssetHandler.LoadAsset<AudioClip>("Ching");
                pumpAud.pitch = 1f - pumps * 0.08f;
                pumpAud.Play();
            }
            pumps += 1;
        }

        private int maxShots = 32;
        private float spread = 30;
        private int shots = 16;
        private int damage = 0;
        private bool gunReady = true;
        private int pumps = 0;
        private bool shouldFire1;
        private bool shouldFire2;


        private GameObject gunTip;
        private GameObject rocket = PrefabBox.rocket;
        private GameObject grenade = PrefabBox.grenade;
        private GameObject beam = PrefabBox.beam;
        private GameObject coin = PrefabBox.coin;
        private GameObject camObj;
        private GameObject coinShot;
        private CoinShot coinShotComp;
        private Coin coinComp;
        private Camera cam;
        private CameraController cc;
        private GunControl gc;
        private Animator anim;
        private AudioSource gunTipAud;
        private AudioSource pumpAud;
        private AudioSource triggerAud;
        private CameraFrustumTargeter targeter;
        private WeaponIcon weaponIcon;
        private WeaponIdentifier wid;
        private WeaponPos wpos;
    }
    public class GoldenGun : GOPWeapon
    {
        public override GameObject Asset { get; protected set; }
        public override int Slot { get; set; }
        public override string Name { get; protected set; }
        public override bool IsSetup { get; protected set; }
        public override bool ShouldHave { get; set; }
        public override void Setup()
        {
            if (Asset == null)
            {
                Asset = AssetHandler.LoadAsset<GameObject>("Golden Gun Prefab");
            }

            if (Asset != null && !IsSetup)
            {
                Name = "Golden Shotgun";
                Slot = ConfigManager.GoldenGunSlot.value;
                Asset.AddComponent<GoldenGunSoul>();
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
                Debug.Log("Golden Shotgun Didn't load");
                Name = "Golden Shotgun is NULL";
                Slot = -1;
            }
        }
        /*public override void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }*/
    }
}
