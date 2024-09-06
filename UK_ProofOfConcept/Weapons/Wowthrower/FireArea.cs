using GunsOPlenty.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GunsOPlenty.Weapons
{
    public class FireArea : MonoBehaviour
    {
        EnemyColliderDetector ecd;
        public GameObject sourceWeapon;
        public List<WispOrb> orblist = new List<WispOrb>();
        public List<WispBuddy> buddylist = new List<WispBuddy>();
        public void Start()
        {
            ecd = base.gameObject.AddComponent<EnemyColliderDetector>();
            if (ecd == null)
            {
                Debug.Log("Dang");
            }
            SlowUpdate();
        }
        public void SlowUpdate()
        {
            //Debug.Log(MonoSingleton<StyleHUD>.Instance.currentMeter + ", " + MonoSingleton<StyleHUD>.Instance.rankIndex);
            // 1500 * 7 + 1500 or 12000 is max style 
            if (base.GetComponent<CapsuleCollider>().enabled == false) //TODO: This is needed for DuelWeild. Find a way to not need it
            {
                ClearStuff();
            }
            float stylefactor = (MonoSingleton<StyleHUD>.Instance.currentMeter + (MonoSingleton<StyleHUD>.Instance.rankIndex * 1500f));
            for (int i = 0; i < ecd.hitList.Count; i++)//(EnemyIdentifier eid in ecd.hitList)ecd.hitList[i]
            {
                if (ecd.hitList[i] != null && !ecd.hitList[i].dead)
                {
                    //TODO: specifically sisyphinian solders don't bleed or die when on fire (does take damage tho)
                    Flammable flameComp = ecd.hitList[i].GetComponentInChildren<Flammable>();
                    if (flameComp != null)
                    {
                        flameComp.Burn(5f, true);
                    }
                    GameObject hitLimb = ecd.GetLimbs(ecd.hitList[i])[UnityEngine.Random.Range(0, ecd.GetLimbs(ecd.hitList[i]).Count - 1)];
                    if (ecd.hitList[i].enemyType == EnemyType.Streetcleaner)
                    {
                        ecd.hitList[i].DeliverDamage(hitLimb, Vector3.zero, hitLimb.transform.position, 0.25f, false, 0f, sourceWeapon, false);
                    }
                    else
                    {
                        ecd.hitList[i].DeliverDamage(hitLimb, Vector3.zero, hitLimb.transform.position, 1f, false, 0f, sourceWeapon, false);
                    }

                }
            }
            if (this.orblist.Count > 0)
            {
                for (int i = 0; i < orblist.Count; i++)
                {

                    if (NoWeaponCooldown.NoCooldown)
                    {
                        orblist[i].heat += 80f;
                    }
                    else
                    {
                        orblist[i].heat += 50f + stylefactor * 0.0025f;//0.002 * 12000 + 26 
                    }
                }
            }
            if (this.buddylist.Count > 0)
            {
                for (int i = 0; i < buddylist.Count; i++)
                {

                    if (NoWeaponCooldown.NoCooldown)
                    {
                        buddylist[i].speed += 55f;
                    }
                    else
                    {
                        buddylist[i].speed += 25f + stylefactor * 0.0025f;
                    }
                }
            }
            //Debug.Log(this.beelist.Count + ", " + this.genlist.Count);//
            if (NoWeaponCooldown.NoCooldown)
            {
                Invoke("SlowUpdate", 0.07f);
            }
            else
            {
                Invoke("SlowUpdate", 0.25f - stylefactor * 0.000015f);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            Enter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Exit(other);
        }

        private void OnTriggerEnter(Collision other)
        {
            Exit(other.collider);
        }

        private void OnTriggerExit(Collision other)
        {
            Exit(other.collider);
        }


        private void Enter(Collider other)
        {
            WispOrb orb;
            WispBuddy buddy;
            //GasolineStain gasoline = other.transform.GetComponentInChildren<GasolineStain>(); // TODO: figure out how to properly light gasoline
            if (other.transform.TryGetComponent<WispOrb>(out orb))
            {
                //Debug.Log("Enter: Gen");
                orblist.Add(orb);

            }
            if (other.transform.TryGetComponent<WispBuddy>(out buddy))
            {
                //Debug.Log("Enter: Bee");
                buddylist.Add(buddy);
            }
            /*if (gasoline != null)
            {
                StainVoxelManager.Instance.TryIgniteAt(gasoline.transform.position, 2);
            }*/
        }
        private void Exit(Collider other)
        {
            WispOrb orb;
            WispBuddy buddy;
            if (other.transform.TryGetComponent<WispOrb>(out orb))
            {
                if (this.orblist.Contains(orb))
                {
                    orblist.Remove(orb);
                }
                //Debug.Log("Exit: Gen");
            }
            if (other.transform.TryGetComponent<WispBuddy>(out buddy))
            {
                if (this.buddylist.Contains(buddy))
                {
                    buddylist.Remove(buddy);
                }
                //Debug.Log("Exit: Gen");
            }
        }

        public void ClearStuff()
        {
            if (ecd != null)
            {
                ecd.ClearStuff();
            }
            if (orblist != null)
            {
                orblist.Clear();
            }
            if (buddylist != null)
            {
                buddylist.Clear();
            }
        }

        private void OnDisable()
        {
            ClearStuff();
        }
    }
}
