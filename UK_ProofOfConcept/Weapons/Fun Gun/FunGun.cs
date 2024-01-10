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
            this.weaponIcon = this.gameObject.AddComponent<WeaponIcon>();
            base.gameObject.TryGetComponent<WeaponIdentifier>(out this.wid);

            if (weaponIcon.weaponDescriptor == null)
            {
                this.weaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
                this.weaponIcon.weaponDescriptor.icon = TrashCan.bundle.LoadAsset<Sprite>("Trollface");
                this.weaponIcon.weaponDescriptor.glowIcon = this.weaponIcon.weaponDescriptor.icon;
            }
            gunBarrelAud.clip = null; // for duel power. Why this happen in this gun specifically, idk.
        }

        private void OnEnable()
        {
            this.gunBarrelAud.Stop();
        }
        private void Update()
        {

            this.transform.localPosition = new Vector3(0.90f, -0.1f, 1.0f);
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
            } else
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
        }

        public void Shoot()
        {
            if (fireDelay < fireTime || NoWeaponCooldown.NoCooldown)
            {
                anim.SetTrigger("Shoot");
                gunBarrelAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("FunShoot"); //scratch dot com type sfx
                gunBarrelAud.pitch = 0.75f + fireMult * 0.02f;
                //gunBarrelAud.volume = 1f;
                gunBarrelAud.Play();
                Instantiate<GameObject>(this.beam, this.gunBarrel.transform.position, this.cc.transform.rotation * GOPUtils.RandRot(1));//this.cc.transform.rotation
                fireTime = 0;
                fireMult += 0.5f;
            }
        }

        public void Ball()
        {
            if (fireDelay * 2.75f < fireTime || NoWeaponCooldown.NoCooldown)
            {
                anim.SetTrigger("Shoot");
                gunBarrelAud.clip = TrashCan.bundle.LoadAsset<AudioClip>("FunBall"); //scratch dot com type sfx
                gunBarrelAud.pitch = 0.5f + fireMult * 0.02f;
                //gunBarrelAud.volume = 15f;
                gunBarrelAud.Play();
                GameObject ball = Instantiate<GameObject>(this.cannonball, this.cam.transform.position + this.cam.transform.forward * 1f, Quaternion.identity);
                Rigidbody ballRig = ball.GetComponent<Rigidbody>();
                ballRig.AddForce(this.cam.transform.forward * (2f * fireMult + 50f), ForceMode.VelocityChange);
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
        private GameObject camObj;
        private Camera cam;
        private CameraController cc;
        private GunControl gc;
        private Animator anim;
        private AudioSource gunBarrelAud;
        private WeaponIcon weaponIcon;
        private WeaponIdentifier wid;
    }
    public class FunnyGun : GOPWeapon
    {
        public static GameObject weaponPrefab;

        public static void LoadAssets()
        {
            weaponPrefab = TrashCan.bundle.LoadAsset<GameObject>("Fun Gun Prefab");
            //weaponPrefab.layer = 13; // the Always on Top layer
            weaponPrefab.AddComponent<FunnyGunSoul>();
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
            return "Fun Gun :D";
        }

        public override string Pref()
        {
            return "fun0";
        }
    }
}
