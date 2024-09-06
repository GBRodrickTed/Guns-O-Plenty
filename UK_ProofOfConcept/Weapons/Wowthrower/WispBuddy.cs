using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;

namespace GunsOPlenty.Weapons
{
    // TODO: Make these little rambunctious rapscallions less laggy
    public class WispBuddy : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 crazyPos;
        public float factor = 0.5f;
        public float bound = 36f;
        public float speed = 5f;
        public float deathTimer = 15f;
        public float startTime;
        public GameObject target;
        public EnemyIdentifier targetEid;
        //public EnemyColliderDetector ecd;
        public GameObject sourceWeapon;

        private ParticleSystem firePart;
        private ParticleSystem bubblePart;
        private ParticleSystem fireTailPart;
        private ParticleSystem bubbleTailPart;

        private Material fireSimpleMat;
        private Material bubbleSimpleMat;
        private Material fireEpicMat;
        private Material bubbleEpicMat;

        private WaterCheck waterCheck;
        private bool amIEpic = false;
        void Start()
        {
            startPos = transform.position;
            crazyPos = startPos;
            /*ecd = base.gameObject.AddComponent<EnemyColliderDetector>();
            if (ecd == null)
            {
                Debug.Log("Nabit");
            }*/
            UpdateTargeter();
            DamageUpdate();
            startTime = Time.time;
            speed = 15;

            firePart = GOPUtils.DescendantByName(base.gameObject, "Wisp Flame").GetComponent<ParticleSystem>();
            firePart.Stop();

            bubblePart = GOPUtils.DescendantByName(base.gameObject, "Wisp Bubble").GetComponent<ParticleSystem>();
            bubblePart.Stop();

            fireTailPart = GOPUtils.DescendantByName(base.gameObject, "Wisp Tail").GetComponent<ParticleSystem>();
            fireTailPart.Stop();

            bubbleTailPart = GOPUtils.DescendantByName(base.gameObject, "Wisp Bubble Tail").GetComponent<ParticleSystem>();
            bubbleTailPart.Stop();

            fireSimpleMat = AssetHandler.LoadAsset<Material>("fireMat");
            bubbleSimpleMat = AssetHandler.LoadAsset<Material>("bubbleMat");
            fireEpicMat = AssetHandler.LoadAsset<Material>("coolfireMat");
            bubbleEpicMat = AssetHandler.LoadAsset<Material>("coolbubbleMat");

            TryGetComponent<WaterCheck>(out waterCheck);
            EpicnessVerification();
        }

        public void Die()
        {
            if (speed > 150f)
            {
                Instantiate<GameObject>(PrefabBox.boom, base.transform.position, Quaternion.identity);
            }
            Destroy(base.gameObject);
        }

        public void DamageUpdate()
        {
            /*EnemyColliderDetector ecd;
            ecd.GetLi*/
            /*if (ecd.hitList.Count() > 0)
            {
                for (int i = 0; i < ecd.hitList.Count(); i++)
                {
                    if (ecd.hitList[i] != null && !ecd.hitList[i].dead)
                    {
                        GameObject hitLimb = ecd.GetLimbs(ecd.hitList[i])[UnityEngine.Random.Range(0, ecd.GetLimbs(ecd.hitList[i]).Count - 1)];
                        ecd.hitList[i].DeliverDamage(hitLimb, Vector3.zero, hitLimb.transform.position, 0.2f, false, 0f, sourceWeapon, false);
                    }
                }
            }*/
            RaycastHit[] hitlist = Physics.SphereCastAll(base.transform.position, 1f, transform.forward, 0);
            EnemyIdentifierIdentifier eidid = null; 
            for (int i = 0; i < hitlist.Length; i++)
            {
                if (hitlist[i].transform.TryGetComponent<EnemyIdentifierIdentifier>(out eidid))
                {
                    if (eidid.eid == null || eidid.eid.dead || eidid.eid.blessed)
                    {
                        continue;
                    }
                    eidid.eid.DeliverDamage(eidid.gameObject, Vector3.zero, transform.position, 0.15f, false, 0f, sourceWeapon, false);
                } 
                    
            }
            Invoke("DamageUpdate", 0.20f);
        }

        public void UpdateTargeter()
        {
            float minDist = 10000000000f;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    EnemyIdentifier eid = enemies[i].GetComponent<EnemyIdentifier>();
                    if (eid == null || eid.dead || eid.blessed)
                    {
                        continue;
                    }
                    float thingEnemyDist = (enemies[i].transform.position - base.transform.position).sqrMagnitude;
                    if (thingEnemyDist < minDist)
                    {
                        minDist = thingEnemyDist;
                        targetEid = eid;
                    }
                }
            }

            if (minDist >= 10000000000f)
            {
                //Debug.Log("Doie");
                target = null;
                targetEid = null;
            }
            else
            {
                //Debug.Log("Ding");
                // attacks a random limb in the enemy
                EnemyIdentifierIdentifier[] limbs = targetEid.GetComponentsInChildren<EnemyIdentifierIdentifier>();
                target = limbs[UnityEngine.Random.Range(0, limbs.Length)].gameObject;
                //target = targetEid.gameObject;
            }
            Invoke("UpdateTargeter", 0.5f);
        }

        void Update()
        {
            if (ConfigManager.WowthrowerEpicFire.value != amIEpic)
            {
                amIEpic = ConfigManager.WowthrowerEpicFire.value;
                EpicnessVerification();
            }

            if (waterCheck.inWater)
            {
                if (!bubblePart.isEmitting)
                {
                    bubblePart.Play();
                    bubbleTailPart.Play();
                    firePart.Stop();
                    fireTailPart.Stop();
                }
            }
            else
            {
                if (!firePart.isEmitting)
                {
                    firePart.Play();
                    fireTailPart.Play();
                    bubblePart.Stop();
                    bubbleTailPart.Stop();
                }
            }
        }
        public void EpicnessVerification()
        {
            ParticleSystemRenderer firePartRend = firePart.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer fireTailPartRend = fireTailPart.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer bubblePartRend = bubblePart.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer bubbleTailPartRend = bubbleTailPart.gameObject.GetComponent<ParticleSystemRenderer>();
            if (ConfigManager.WowthrowerEpicFire.value == true)
            {
                firePartRend.material = fireEpicMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = Color.white;

                bubblePartRend.material = bubbleEpicMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = Color.white;

                fireTailPartRend.material = fireEpicMat;
                ParticleSystem.MainModule fireTailMain = fireTailPart.main;
                fireTailMain.startColor = Color.white;

                bubbleTailPartRend.material = bubbleEpicMat;
                ParticleSystem.MainModule bubbleTailMain = bubbleTailPart.main;
                bubbleTailMain.startColor = Color.white;
            }
            else
            {
                firePartRend.material = fireSimpleMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = GOPUtils.HexToColor("#FF7600");

                bubblePartRend.material = bubbleSimpleMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = GOPUtils.HexToColor("#99E7F8");

                fireTailPartRend.material = fireSimpleMat;
                ParticleSystem.MainModule fireTailMain = fireTailPart.main;
                fireTailMain.startColor = GOPUtils.HexToColor("#FF7600");

                bubbleTailPartRend.material = bubbleSimpleMat;
                ParticleSystem.MainModule bubbleTailMain = bubbleTailPart.main;
                bubbleTailMain.startColor = GOPUtils.HexToColor("#99E7F8");
            }
        }

        void FixedUpdate()
        {
            crazyPos += GOPUtils.RandPos((bound + speed * 0.8f) * Time.deltaTime);
            if (target != null)
            {
                crazyPos += (target.transform.position - base.transform.position).normalized * (speed * Time.deltaTime);
            }
            float blend = Mathf.Pow(factor, Time.deltaTime * speed);
            base.transform.position = Vector3.Lerp(crazyPos, base.transform.position, blend);
            if (((Time.time - startTime > 1000f) && !NoWeaponCooldown.NoCooldown) || (speed > 200f))
            {
                Die();
            }
            //transform.position = Vector3.MoveTowards(transform.position, crazyPos, Time.deltaTime * speed);
        }
    }
}
