using GunsOPlenty.Stuff;
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
            // it literally only works here, I'll polish this later
            if (!MonoSingleton<StyleHUD>.instance.idNameDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            }

            if (!MonoSingleton<StyleHUD>.instance.freshnessDecayMultiplierDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);
            }

            this.cam = MonoSingleton<CameraController>.Instance.GetComponent<Camera>();
            this.gc = MonoSingleton<GunControl>.Instance.GetComponent<GunControl>();
            this.camObj = this.cam.gameObject;
            this.cc = MonoSingleton<CameraController>.Instance;
            this.coinShot = TrashCan.bundle.LoadAsset<GameObject>("CoinShot");
            this.coinShotComp = coinShot.GetComponent<CoinShot>();
            this.coinShotComp.sourceWeapon = base.gameObject;
            this.coinShotComp.ignoreCoins = true;
            this.gunTip = GOPUtils.DescendantByName(this.gameObject, "ShootPoint");
            this.gunTipAud = gunTip.GetComponent<AudioSource>();
            this.pumpAud = GOPUtils.DescendantByName(this.gameObject, "Shotgun_Pump").GetComponent<AudioSource>();
            this.triggerAud = GOPUtils.DescendantByName(this.gameObject, "Shotgun_Trigger").GetComponent<AudioSource>();
            this.targeter = MonoSingleton<CameraFrustumTargeter>.instance;
            this.anim = GOPUtils.DescendantByName(this.gameObject, "golden_shotgun").GetComponent<Animator>();

            base.gameObject.TryGetComponent<WeaponIdentifier>(out this.wid);

            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();

            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.variationColor = WeaponVariant.GoldVariant;
                this.weaponIcon.weaponDescriptor.icon = TrashCan.bundle.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }

        }

        private void Update()
        {
            this.transform.localPosition = new Vector3(0.50f, -1.25f, 0.50f);

            if (NoWeaponCooldown.NoCooldown)
            {
                shouldFire1 = MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed;
                shouldFire2 = MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed;
            } else
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
                triggerAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("Click");
                triggerAud.Play();
                if (pumps > 0)
                {
                    anim.SetTrigger("Shoot");
                    gunTipAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("ShotgunBoom");
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
                pumpAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("BigCha");
                pumpAud.pitch = 1f;
                pumpAud.Play();
            }
            else
            {
                pumpAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("Cha");
                pumpAud.pitch = 1f - pumps * 0.08f;
                pumpAud.Play();
            }
        }

        public void UnpumpGun()
        {
            if (pumps >= 3)
            {
                pumpAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("BigChing");
                pumpAud.pitch = 1f;
                pumpAud.Play();
            }
            else
            {
                pumpAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("Ching");
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
    }
    public class GoldenGun : GOPWeapon
    {
        public static GameObject weaponPrefab;

        public static void LoadAssets()
        {
            // probably an infinitly better way to do this but idk how
            weaponPrefab = TrashCan.bundle.LoadAsset<GameObject>("Golden Gun Prefab");
            weaponPrefab.AddComponent<GoldenGunSoul>();
            foreach (GameObject go in GOPUtils.DescendantsList(weaponPrefab))
            {
                if (go.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                {
                    mr.material.shader = ShaderBox.vertexlit_emissive;
                }
                else if (go.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr))
                {
                    smr.material.shader = ShaderBox.vertexlit_emissive;
                }
            }
        }

        public override GameObject Create(Transform parent)
        {
            GameObject Asset = GameObject.Instantiate(weaponPrefab, parent);

            return Asset;
        }
        public override int Slot()
        {
            return 0;
        }

        public override int WheelOrder()
        {
            return 7;
        }
        public override string Name()
        {
            return "Golden Shotgun";
        }

        public override string Pref()
        {
            return "ggn0";
        }
    }
}
