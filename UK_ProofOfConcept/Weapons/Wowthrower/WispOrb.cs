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
    public class WispOrb : MonoBehaviour
    {
        public GameObject sourceWeapon;
        public List<WispBuddy> buddies = new List<WispBuddy>();
        private Vector3 startPos;
        private float lifeTimer = 0;
        private float speed = 1;
        public float heat = 15f;
        private float startTime;
        public float deathTime;
        private float spawnLoopTime;
        private bool firstDeath;
        //EnemyColliderDetector ecd;

        
        private ParticleSystem wispPart;
        private Light wispLight;
        private ParticleSystem firePart;
        private ParticleSystem.EmissionModule firePartEmissions;
        private ParticleSystem bubblePart;
        private ParticleSystem.EmissionModule bubblePartEmissions;

        private Material wispSimpleMat;
        private Material fireSimpleMat;
        private Material bubbleSimpleMat;
        private Material fireEpicMat;
        private Material bubbleEpicMat;
        private AudioSource audSource;

        bool amIEpic = false;
        private WaterCheck waterCheck;

        void Start()
        {
            deathTime = 45f;
            firstDeath = true;
            startPos = transform.position;
            Invoke("SpawnLoop", 1 + 0.5f);
            DamageUpdate();
            /*ecd = base.gameObject.AddComponent<EnemyColliderDetector>();
            if (ecd == null)
            {
                Debug.Log("Dang");
            }*/
            startTime = Time.time;
            spawnLoopTime = Time.time;

            wispPart = GOPUtils.DescendantByName(base.gameObject, "Wisp Flame").GetComponent<ParticleSystem>();
            wispPart.Stop();
            GOPUtils.DescendantByName(base.gameObject, "Wisp Flame").TryGetComponent(out wispLight);

            firePart = GOPUtils.DescendantByName(base.gameObject, "Wisp Aura").GetComponent<ParticleSystem>();
            firePartEmissions = firePart.emission;
            firePart.Stop();

            bubblePart = GOPUtils.DescendantByName(base.gameObject, "Wisp Bubble Aura").GetComponent<ParticleSystem>();
            bubblePartEmissions = bubblePart.emission;
            bubblePart.Stop();

            wispSimpleMat = AssetHandler.LoadAsset<Material>("WispOrbMat");
            fireSimpleMat = AssetHandler.LoadAsset<Material>("fireMat");
            bubbleSimpleMat = AssetHandler.LoadAsset<Material>("bubbleMat");
            fireEpicMat = AssetHandler.LoadAsset<Material>("coolfireMat");
            bubbleEpicMat = AssetHandler.LoadAsset<Material>("coolbubbleMat");

            TryGetComponent(out audSource);
            audSource.volume = 0.5f;
            audSource.Stop();

            waterCheck = GetComponent<WaterCheck>();
            EpicnessVerification();
        }

        public void SpawnLoop()
        {
            if (buddies.Count < 10)
            {
                SpawnBuddy();
            }
            for (int i = 0; i < buddies.Count; i++)
            {
                if (buddies[i] == null)
                {
                    buddies.RemoveAt(i);
                    i--;
                }
            }
            Invoke("SpawnLoop", (2.5f - ((heat) * 0.0008f)));
        }

        public void Die()
        {
            Destroy(base.gameObject);
            if (heat > 2000f)//2520f
            {
                Instantiate<GameObject>(PrefabBox.superBoom, base.transform.position, Quaternion.identity);
            } else if (heat > 1000f)
            {
                Instantiate<GameObject>(PrefabBox.bigBoom, base.transform.position, Quaternion.identity);
            } else
            {
                Instantiate<GameObject>(PrefabBox.harmlessBoom, base.transform.position, Quaternion.identity);
            }
            for (int i = 0; i < buddies.Count; i++)
            {
                buddies[i].Die();
            }
            buddies.Clear();
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
                    eidid.eid.DeliverDamage(eidid.gameObject, Vector3.zero, transform.position, 0.5f, false, 0f, sourceWeapon, false);
                }

            }
            Invoke("DamageUpdate", 0.50f);
        }

        void Update()
        {
            if (ConfigManager.WowthrowerEpicFire.value != amIEpic)
            {
                amIEpic = ConfigManager.WowthrowerEpicFire.value;
                EpicnessVerification();
            }
            //heat += Time.deltaTime * 250f;
            if (waterCheck.inWater)
            {
                if (!bubblePart.isEmitting)
                {
                    bubblePart.Play();
                    firePart.Stop();
                    wispPart.Stop();
                    audSource.clip = AssetHandler.LoadAsset<AudioClip>("BubbleOrbNoises");
                    audSource.time = UnityEngine.Random.Range(0f, 2.5f);
                    audSource.loop = true;
                    audSource.Play();
                }
                bubblePartEmissions.rateOverTime = 10 * HeatFactor(15);
            }
            else
            {
                if (!firePart.isEmitting)
                {
                    firePart.Play();
                    wispPart.Play();
                    bubblePart.Stop();
                    audSource.clip = AssetHandler.LoadAsset<AudioClip>("FlameOrbNoises");
                    audSource.loop = true;
                    audSource.time = UnityEngine.Random.Range(0f, 2.5f);
                    audSource.Play();
                }
                firePartEmissions.rateOverTime = 20 * HeatFactor(15);
                ParticleSystem.MainModule firemain = firePart.main;
                firemain.startSpeed = 4 * HeatFactor(1.75f);
                firemain.simulationSpeed = 0.75f * HeatFactor(3.5f);
            }
        }

        public void EpicnessVerification()
        {
            ParticleSystemRenderer firePartRend = firePart.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer bubblePartRend = bubblePart.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer wispPartRend = wispPart.gameObject.GetComponent<ParticleSystemRenderer>();
            if (ConfigManager.WowthrowerEpicFire.value == true)
            {
                firePartRend.material = fireEpicMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = Color.white;

                bubblePartRend.material = bubbleEpicMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = Color.white;

                wispPartRend.material = fireEpicMat;
                ParticleSystem.MainModule wispMain = wispPart.main;
                wispMain.startColor = Color.white;
            }
            else
            {
                firePartRend.material = fireSimpleMat;
                ParticleSystem.MainModule fireMain = firePart.main;
                fireMain.startColor = GOPUtils.HexToColor("#FF7600");

                bubblePartRend.material = bubbleSimpleMat;
                ParticleSystem.MainModule bubbleMain = bubblePart.main;
                bubbleMain.startColor = GOPUtils.HexToColor("#99E7F8");

                wispPartRend.material = wispSimpleMat;
                ParticleSystem.MainModule wispMain = wispPart.main;
                wispMain.startColor = GOPUtils.HexToColor("#FF7600");
            }
        }

        public float HeatFactor(float factor)
        {
            return (1 + (heat / 2880) * factor);
        }

        void FixedUpdate()
        {
            //transform.Rotate(0, rotVel * Time.deltaTime, 0, Space.Self);
            transform.position = Vector3.Lerp(startPos + Vector3.up, startPos + Vector3.down, 0.5f + (Mathf.Sin(lifeTimer) / 2f));
            lifeTimer += Time.deltaTime;
            if (NoWeaponCooldown.NoCooldown) startTime = Time.time;
            if ((((Time.time - startTime > deathTime) && !NoWeaponCooldown.NoCooldown) || (heat > 2880f)) && firstDeath)
            {
                firstDeath = false;
                Die();
            }
            /*if ((Time.time - spawnLoopTime) > 2.5 - ((rotVel) * 0.0008f))
            {
                SpawnLoop();
                spawnLoopTime = Time.time;
            }*/
        }

        void SpawnBuddy()
        {
            WispBuddy newBuddy = Instantiate(AssetHandler.customPrefabs["Wisp Baby"], transform.position, Quaternion.identity).GetComponent<WispBuddy>();
            newBuddy.GetComponent<WispBuddy>().sourceWeapon = sourceWeapon;
            newBuddy.GetComponent<WispBuddy>().speed = (15f + heat * 0.1f);
            buddies.Add(newBuddy);
        }
    }
}
