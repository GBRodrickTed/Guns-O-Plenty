using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    //Used for detecting the whole enemy and not its indiviual limbs
    //Code similar to how HurtZone works
    public class EnemyColliderDetector : MonoBehaviour
    {
        public List<EnemyIdentifier> hitList = new List<EnemyIdentifier>();
        private List<EnemyIdentifier> toRemove = new List<EnemyIdentifier>();
        private List<float> hitTimes = new List<float>();
        private List<int> limbCount = new List<int>();
        private List<List<GameObject>> limbs = new List<List<GameObject>>();
        public float checkHitListFreq = 0.5f;

        private void Start()
        {
            CheckHitList();
        }
        // Might not be nessessary
        // would be kinda expensive to check the whole list if an enemy is alive every frame
        public void CheckHitList()
        {
            if (hitList.Count > 0)
            {
                for (int i = 0; i < this.hitList.Count; i++)
                {
                    if (this.hitList[i] == null || this.hitList[i].dead)
                    {
                        this.toRemove.Add(this.hitList[i]);
                    }
                }
            }
            
            if (this.toRemove.Count > 0)
            {
                for (int i = 0; i < this.toRemove.Count; i++)
                {
                    List<int> list = this.limbCount;
                    int index = this.hitList.IndexOf(this.toRemove[i]);
                    int num2 = list[index];
                    list[index] = num2 - 1;
                    if (this.limbCount[index] <= 0)
                    {
                        //Debug.Log("My Man's Dead: " + this.toRemove[i].name);
                        this.hitTimes.RemoveAt(index);
                        this.limbCount.RemoveAt(index);
                        this.limbs.RemoveAt(index);
                        this.hitList.Remove(this.toRemove[i]);
                    }
                }
                this.toRemove.Clear();
            }
            Invoke("CheckHitList", checkHitListFreq);
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
                if (eidid != null && eidid.eid != null && !eidid.eid.dead)
                {
                    if (!this.hitList.Contains(eidid.eid))
                    {
                        //Debug.Log("Enter: " + eidid.eid.name);
                        this.hitList.Add(eidid.eid);
                        this.hitTimes.Add(1f);
                        this.limbCount.Add(1);
                        this.limbs.Add(new List<GameObject>());
                        this.limbs[this.hitList.IndexOf(eidid.eid)].Add(eidid.gameObject);
                        if (!base.enabled)
                        {
                            return;
                        }
                    }
                    else
                    {
                        //adds limb if eid already found
                        List<int> list = this.limbCount;
                        int index = this.hitList.IndexOf(eidid.eid);
                        int num = list[index];
                        list[index] = num + 1;
                        if (!this.limbs[this.hitList.IndexOf(eidid.eid)].Contains(eidid.gameObject))
                        {
                            this.limbs[this.hitList.IndexOf(eidid.eid)].Add(eidid.gameObject);
                        }
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
                if (eidid != null && eidid.eid != null && this.hitList.Contains(eidid.eid))
                {
                    List<int> list = this.limbCount;
                    int index = this.hitList.IndexOf(eidid.eid);
                    int num2 = list[index];
                    list[index] = num2 - 1;
                    if (this.limbs[this.hitList.IndexOf(eidid.eid)].Contains(eidid.gameObject))
                    {
                        //Debug.Log("Adding Remove: " + eidid.gameObject.name);
                        this.limbs[this.hitList.IndexOf(eidid.eid)].Remove(eidid.gameObject);
                    }
                    if (this.limbCount[index] <= 0)
                    {
                        //Debug.Log("Exit: " + eidid.eid.name);
                        this.hitTimes.RemoveAt(index);
                        this.limbCount.RemoveAt(index);
                        this.limbs.RemoveAt(index);
                        this.hitList.Remove(eidid.eid);
                    }
                }
            }
        }

        public List<GameObject> GetLimbs (EnemyIdentifier eid)
        {
            if (this.limbs.Count > 0 && this.hitList.Contains(eid)) return this.limbs[this.hitList.IndexOf(eid)];
            return null;
        }

        public void ClearStuff()
        {
            //clears all enemys in list
            //Debug.Log("Clearing Stuff - Begin");
            if (this.hitList.Count > 0)
            {
                /*foreach (EnemyIdentifier eid in this.hitList)
                {
                    Debug.Log("Removing: " + eid.name);
                }*/
                this.hitTimes.Clear();
                this.limbCount.Clear();
                this.limbs.Clear();
                this.hitList.Clear();
            }
            //Debug.Log("Clearing Stuff - End");
        }

        private void OnDisable()
        {
            ClearStuff();
        }
    }
}
