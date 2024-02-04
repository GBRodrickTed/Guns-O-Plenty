using GunsOPlenty.Data;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace GunsOPlenty.Weapons
{
    public class CoinShot : MonoBehaviour
    {
        //punchflection but epic
        private void Start()
        {
            coin = coinPref.GetComponent<Coin>();
            lmask = coin.lmask;
            lmask |= GOPUtils.NameToLayerBit("Limb");
            lmask |= GOPUtils.NameToLayerBit("BigCorpse");
            Shoot();
            base.Invoke("Death", 1f);
        }

        private void Death()
        {
            UObject.Destroy(base.gameObject);
        }

        private void Update()
        {
            
        }
        private void Shoot()
        {
            bool RayHitCheck = Physics.Raycast(base.transform.position, base.transform.forward, out this.hit, float.PositiveInfinity, lmask);
            LineRenderer lr = this.coin.SpawnBeam().GetComponent<LineRenderer>();
            lr.SetPosition(0, base.transform.position);
            int count = 0;
            if (RayHitCheck && ignoreCoins)
            {
                Coin stupidCoin = null;
                RaycastHit rayHit = this.hit;
                while (true)
                {
                    if (rayHit.transform.gameObject.TryGetComponent<Coin>(out stupidCoin))
                    {
                        RayHitCheck = Physics.Raycast(rayHit.point + base.transform.forward, base.transform.forward, out rayHit, float.PositiveInfinity, lmask);
                        if (!RayHitCheck)
                        {
                            break;
                        }
                    } else
                    {
                        this.hit = rayHit;
                        break;
                    }
                    count++;
                    //Debug.Log("Reccursion: " + count);
                    if (count > 20)
                    {
                        break;
                    }
                }
                //Debug.Log("Total Reccursions: " + count);
            }

            if (RayHitCheck)
            {
                Transform hitTrans = this.hit.transform;
                GameObject hitObj = hitTrans.gameObject;
                GameObject shotCoin = Instantiate<GameObject>(coinPref, hit.point - base.transform.forward, Quaternion.identity);
                Coin shotCoinComp = shotCoin.GetComponent<Coin>();
                shotCoinComp.power = power;
                if (hitObj.CompareTag("Enemy") || hitObj.CompareTag("Body") || hitObj.CompareTag("Limb") || hitObj.CompareTag("EndLimb") || hitObj.CompareTag("Head"))
                {
                    EnemyIdentifierIdentifier eidid = this.hit.transform.GetComponentInParent<EnemyIdentifierIdentifier>();
                    this.eid = eidid.eid;
                    if (this.eid && !this.eid.dead)
                    {
                        this.eid.hitter = "coin shot";
                        if (!this.eid.hitterWeapons.Contains("coin shot")) // for arsenal style
                        {
                            this.eid.hitterWeapons.Add("coin shot");
                        }
                        eid.DeliverDamage(hitObj, (hitTrans.position - base.transform.position).normalized * 10000f, this.hit.point, power, false, 1f, this.sourceWeapon, false);
                        if (power >= 10)
                        {
                            MonoSingleton<StyleHUD>.Instance.AddPoints((int)(10*power), "ultrakill.moneyshot", sourceWeapon, eid, -1, "", "");
                        }
                    }
                }
                lr.SetPosition(1, hit.point);
                Rigidbody shotCoinRigid = shotCoin.GetComponent<Rigidbody>();
                shotCoinRigid.velocity = Vector3.zero;
                shotCoinRigid.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
                new GameObject().AddComponent<CoinCollector>().coin = shotCoin;
            } else 
            {
                lr.SetPosition(1, base.transform.position + base.transform.forward * 1000f);
            }
        }
        public Coin coin;
        public float power = 1f;
        private GameObject coinPref = PrefabBox.coin;
        public GameObject sourceWeapon;
        public bool ignoreCoins = true;
        private LayerMask lmask;
        private RaycastHit hit;
        private EnemyIdentifier eid;
    }
}
