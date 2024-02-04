using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Weapons
{
    //Code similar to how HurtZone works
    public class FireArea : MonoBehaviour
    {
        public List<EnemyIdentifier> hitlist = new List<EnemyIdentifier>();
        public List<EnemyIdentifier> toRemove = new List<EnemyIdentifier>();
        public List<float> hitTimes = new List<float>();
        public List<int> limbCount = new List<int>();

        public void FixedUpdate()
        {
            if (hitlist.Count > 0)
            {
                foreach (EnemyIdentifier eid in hitlist)
                {
                    if (eid != null && !eid.dead)
                    {
                        Flammable flameComp = eid.GetComponentInChildren<Flammable>();
                        if (flameComp != null)
                        {
                            flameComp.Burn(10f, true);
                        }
                        //TODO: specifically sisyphinian solders don't bleed or die when on fire
                        eid.DeliverDamage(eid.transform.gameObject, Vector3.zero, eid.transform.position, 1f / 2f, false, 0f, null, false);
                    }
                    else
                    {
                        this.toRemove.Add(eid);
                    }
                }
                if (this.toRemove.Count > 0)
                {
                    foreach (EnemyIdentifier item in this.toRemove)
                    {
                        this.hitlist.Remove(item);
                    }
                    this.toRemove.Clear();
                }
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
            if (other.gameObject.layer == 10 || other.gameObject.layer == 11 || other.gameObject.layer == 12 || other.gameObject.layer == 20)
            {
                EnemyIdentifierIdentifier eidid;
                eidid = other.transform.GetComponentInParent<EnemyIdentifierIdentifier>();
                if (eidid != null && eidid.eid != null && !eidid.eid.dead) //  && eidid.transform.localPosition != Vector3.zero
                {
                    if (!this.hitlist.Contains(eidid.eid))
                    {
                        Debug.Log("Enter: " + eidid.eid.name);
                        this.hitlist.Add(eidid.eid);
                        this.hitTimes.Add(1f);
                        this.limbCount.Add(1);
                        if (!base.enabled)
                        {
                            return;
                        }
                    }
                    else
                    {
                        //adds limb if eid already found
                        List<int> list = this.limbCount;
                        int index = this.hitlist.IndexOf(eidid.eid);
                        int num = list[index];
                        list[index] = num + 1;
                    }
                }
            }
        }
        private void Exit(Collider other)
        {
            if (other.gameObject.layer == 10 || other.gameObject.layer == 11 || other.gameObject.layer == 12 || other.gameObject.layer == 20)
            {
                EnemyIdentifierIdentifier eidid;
                eidid = other.transform.GetComponentInParent<EnemyIdentifierIdentifier>();
                if (eidid != null && eidid.eid != null && this.hitlist.Contains(eidid.eid))
                {
                    int num = this.hitlist.IndexOf(eidid.eid);
                    List<int> list = this.limbCount;
                    int index = num;
                    int num2 = list[index];
                    list[index] = num2 - 1;
                    if (this.limbCount[num] <= 0)
                    {
                        Debug.Log("Exit: " + eidid.eid.name);
                        this.hitTimes.RemoveAt(num);
                        this.limbCount.RemoveAt(num);
                        this.hitlist.Remove(eidid.eid);
                    }
                }
            }
        }

        public void ClearStuff()
        {
            //clears all enemys in list
            //Debug.Log("Clearing Stuff");
            if (this.hitlist.Count > 0)
            {
                //Debug.Log("Clearing Stuff");
                /*foreach (EnemyIdentifier eid in this.hitlist)
                {
                    Debug.Log("Removing: " + eid.name);
                }*/
                this.hitTimes.Clear();
                this.limbCount.Clear();
                this.hitlist.Clear();
            }
            //Debug.Log("Clearing Stuff - End");
        }

        private void OnDisable()
        {
            if (this.hitlist.Count > 0)
            {
                ClearStuff();
            }
        }
    }
}
