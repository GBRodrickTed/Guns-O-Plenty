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

namespace GunsOPlenty.Weapons
{
    public class FireArea : MonoBehaviour
    {
        EnemyColliderDetector ecd;
        public GameObject sourceWeapon;
        public List<Generator> genlist = new List<Generator>();
        public List<BeeThing> beelist = new List<BeeThing>();
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
            if (this.genlist.Count > 0)
            {
                for (int i = 0; i < genlist.Count; i++)
                {

                    if (NoWeaponCooldown.NoCooldown)
                    {
                        genlist[i].rotVel += 80f;
                    }
                    else
                    {
                        genlist[i].rotVel += 50f + stylefactor * 0.0025f;//0.002 * 12000 + 26 
                    }
                }
            }
            if (this.beelist.Count > 0)
            {
                for (int i = 0; i < beelist.Count; i++)
                {

                    if (NoWeaponCooldown.NoCooldown)
                    {
                        beelist[i].speed += 55f;
                    }
                    else
                    {
                        beelist[i].speed += 25f + stylefactor * 0.0025f;
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
            Generator gen = other.transform.GetComponentInParent<Generator>();//other.transform.GetComponentInParent<Generator>();;
            BeeThing bee = other.transform.GetComponentInParent<BeeThing>();
            if (gen != null)
            {
                //Debug.Log("Enter: Gen");
                genlist.Add(gen);

            }
            if (bee != null)
            {
                //Debug.Log("Enter: Bee");
                beelist.Add(bee);
            }
        }
        private void Exit(Collider other)
        {
            Generator gen = other.transform.GetComponentInParent<Generator>();//other.transform.GetComponentInParent<Generator>();
            BeeThing bee = other.transform.GetComponentInParent<BeeThing>();
            if (gen != null && this.genlist.Contains(gen))
            {
                //Debug.Log("Exit: Gen");
                genlist.Remove(gen);

            }
            if (bee != null && this.beelist.Contains(bee))
            {
                //Debug.Log("Exit: Bee");
                beelist.Remove(bee);
            }
        }

        public void ClearStuff()
        {
            ecd.ClearStuff();
            this.genlist.Clear();
            this.beelist.Clear();
        }

        private void OnDisable()
        {
            ClearStuff();
        }
    }
}
