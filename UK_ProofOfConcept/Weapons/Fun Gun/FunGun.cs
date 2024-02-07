using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GunsOPlenty.Stuff;
using UnityEngine.Diagnostics;
using System.Runtime.CompilerServices;
using ULTRAKILL.Cheats;
using GunsOPlenty.Utils;
using GunsOPlenty.Data;

namespace GunsOPlenty.Weapons
{
    public class FunnyGunSoul : MonoBehaviour
    {
        private void Start()
        {
            this.cam = MonoSingleton<CameraController>.Instance.GetComponent<Camera>();
            this.gc = MonoSingleton<GunControl>.Instance.GetComponent<GunControl>();
            this.camObj = this.cam.gameObject;
            this.cc = MonoSingleton<CameraController>.Instance;
            this.gunBarrel = GOPUtils.DescendantByName(this.gameObject, "ShootPoint");
            this.gunBarrelAud = gunBarrel.GetComponent<AudioSource>();
            this.anim = this.GetComponent<Animator>();
            this.targeter = MonoSingleton<CameraFrustumTargeter>.instance;

            this.wid = base.gameObject.GetComponent<WeaponIdentifier>();
            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();
            this.wpos = this.gameObject.AddComponent<WeaponPos>();
            this.wpos.defaultPos = new Vector3(0.731f, -0.3491f, 1.2509f);
            this.wpos.middlePos = new Vector3(0f, -0.3491f, 1.2509f);
            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.variationColor = WeaponVariant.BlueVariant;
                this.weaponIcon.weaponDescriptor.icon = AssetHandler.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }
            gunBarrelAud.clip = null; // for duel power. Why this happen in this gun specifically, idk.
            Debug.Log("BEFORE RETREIVE");
            Debug.Log(fireDelay);
            Debug.Log(fireTime);
            Debug.Log(fireMult);
            Debug.Log(fireMultTime);
            Debug.Log(fireMultDelay);
            RetreiveCharges();
            Debug.Log("AFTER RETREIVE");
            Debug.Log(fireDelay);
            Debug.Log(fireTime);
            Debug.Log(fireMult);
            Debug.Log(fireMultTime);
            Debug.Log(fireMultDelay);
        }

        private void OnEnable()
        {
            this.gunBarrelAud.Stop();
            RetreiveCharges();
            Debug.Log("ON ENABLED");
            Debug.Log(fireDelay);
            Debug.Log(fireTime);
            Debug.Log(fireMult);
            Debug.Log(fireMultTime);
            Debug.Log(fireMultDelay);
        }

        private void RetreiveCharges()
        {
            fireDelay = ChargeManager.funGun.fireDelay;
            fireTime = ChargeManager.funGun.fireTime;
            fireMult = ChargeManager.funGun.fireMult;
            fireMultTime = ChargeManager.funGun.fireMultTime;
            fireMultDelay = ChargeManager.funGun.fireMultDelay;
        }

        private void StoreCharges()
        {
            ChargeManager.funGun.fireDelay = fireDelay;
            ChargeManager.funGun.fireTime = fireTime;
            ChargeManager.funGun.fireMult = fireMult;
            ChargeManager.funGun.fireMultTime = fireMultTime;
            ChargeManager.funGun.fireMultDelay = fireMultDelay;
        }

        private void Update()
        {
            //this.transform.localPosition = new Vector3(0.90f, -0.1f, 1.0f);
            if (NoWeaponCooldown.NoCooldown)
            {
                fireMult = 100f;
            }
            fireTime += Time.deltaTime * fireMult;
            fireMultTime += Time.deltaTime;
            //Debug.Log(fireDelay + " : " + fireTime + " : " + fireMult);
            if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed && this.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                base.Invoke("Shoot", this.wid.delay);
            }
            else if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed && this.gc.activated && !GameStateManager.Instance.PlayerInputLocked)
            {
                base.Invoke("Ball", this.wid.delay);
            }
            else
            {
                if (fireMultDelay < fireMultTime)
                {
                    fireMult *= 0.9f;
                    fireMultTime = 0;
                }
            }
            if (fireMult > 100f)
            {
                fireMult = 100f;
            }
            if (fireMult < 1f)
            {
                fireMult = 1f;
            }
            if (this.wid.delay == 0)
            {
                StoreCharges();
            }
        }

        public void Shoot()
        {
            if (fireDelay < fireTime || NoWeaponCooldown.NoCooldown)
            {
                anim.SetTrigger("Shoot");
                gunBarrelAud.clip = AssetHandler.LoadAsset<AudioClip>("FunShoot"); //scratch dot com type beat
                gunBarrelAud.pitch = 0.75f + fireMult * 0.02f;
                //gunBarrelAud.volume = 1f;
                gunBarrelAud.Play();
                GameObject funbeam = Instantiate<GameObject>(this.beam, this.gunBarrel.transform.position, this.cc.transform.rotation * GOPUtils.RandRot(fireMult / 100f));
                RevolverBeam gutRevBeam = funbeam.GetComponent<RevolverBeam>();
                funbeam.GetComponent<RevolverBeam>().damage = 0.5f;
                if (this.targeter.CurrentTarget && this.targeter.IsAutoAimed)
                {
                    funbeam.transform.LookAt(this.targeter.CurrentTarget.bounds.center);
                    funbeam.transform.rotation *= GOPUtils.RandRot(fireMult / 100f);
                }
                fireTime = 0;
                fireMult += 0.5f;
            }
        }

        public void Ball()
        {
            if (fireDelay * 2.75f < fireTime || NoWeaponCooldown.NoCooldown)
            {
                anim.SetTrigger("Shoot");
                gunBarrelAud.clip = AssetHandler.LoadAsset<AudioClip>("FunBall"); //scratch dot com type beat
                gunBarrelAud.pitch = 0.5f + fireMult * 0.02f;
                //gunBarrelAud.volume = 15f;
                gunBarrelAud.Play();
                GameObject ball = Instantiate<GameObject>(this.cannonball, this.gunBarrel.transform.position + this.gunBarrel.transform.forward * 1f, this.cc.transform.rotation);
                Rigidbody ballRig = ball.GetComponent<Rigidbody>();
                if (this.targeter.CurrentTarget && this.targeter.IsAutoAimed)
                {
                    ballRig.transform.LookAt(this.targeter.CurrentTarget.bounds.center);
                }
                ballRig.AddForce(ballRig.transform.forward * (2f * fireMult + 50f), ForceMode.VelocityChange);
                fireTime = 0;
                fireMult -= 1.25f + fireMult * 0.05f;

                if (fireMultDelay < fireMultTime)
                {
                    fireMult *= 0.9f;
                    fireMultTime = 0;
                }
            }
        }

        private float fireDelay = 1f;
        private float fireTime = 0f;
        private float fireMult = 1f;
        private float fireMultTime = 0f;
        private float fireMultDelay = 0.1f;


        private GameObject gunBarrel;
        private GameObject rocket = PrefabBox.rocket;
        private GameObject grenade = PrefabBox.grenade;
        private GameObject beam = PrefabBox.beam;
        private GameObject coin = PrefabBox.coin;
        private GameObject cannonball = PrefabBox.cannonball;
        private GameObject gutterbeam = PrefabBox.gutterbeam;
        private GameObject camObj;
        private Camera cam;
        private CameraController cc;
        private GunControl gc;
        private Animator anim;
        private CameraFrustumTargeter targeter;
        private AudioSource gunBarrelAud;
        private WeaponIcon weaponIcon;
        private WeaponPos wpos;
        private WeaponIdentifier wid;
    }

    public class FunnyGun : GOPWeapon
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
                Asset = AssetHandler.LoadAsset<GameObject>("Fun Gun Prefab");
            }
            
            if (Asset != null && !IsSetup)
            {
                Name = "Funny Gun :D";
                Slot = ConfigManager.FunGunSlot.value;
                Asset.AddComponent<FunnyGunSoul>();
                //Asset.AddComponent<WeaponIdentifier>();
                foreach (GameObject thing in GOPUtils.DescendantsList(Asset))
                {
                    if (thing.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
                    {
                        mr.material.shader = ShaderBox.vertexlit_emissive;
                    }
                    else if (thing.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr))
                    {
                        smr.material.shader = ShaderBox.vertexlit_emissive;
                    }
                }
                IsSetup = true;
            }
            else
            {
                Debug.Log("Fun Gun Didn't load");
                Name = "Fun Gun is NULL";
                Slot = -1;
            }
        }
        /*public override void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }*/
    }
}
