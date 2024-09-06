using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BeeThing : MonoBehaviour
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
    public EnemyColliderDetector ecd;
    public GameObject sourceWeapon;
    void Start()
    {
        startPos = transform.position;
        crazyPos = startPos;
        ecd = base.gameObject.AddComponent<EnemyColliderDetector>();
        if (ecd == null)
        {
            Debug.Log("Nabit");
        }
        UpdateTargeter();
        DamageUpdate();
        startTime = Time.time;
        speed = 15;
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
        if (ecd.hitList.Count() > 0)
        {
            for (int i = 0; i < ecd.hitList.Count(); i++)
            {
                if (ecd.hitList[i] != null && !ecd.hitList[i].dead)
                {
                    GameObject hitLimb = ecd.GetLimbs(ecd.hitList[i])[UnityEngine.Random.Range(0, ecd.GetLimbs(ecd.hitList[i]).Count - 1)];
                    ecd.hitList[i].DeliverDamage(hitLimb, Vector3.zero, hitLimb.transform.position, 0.2f, false, 0f, sourceWeapon, false);
                }
            }
        }
        Invoke("DamageUpdate", 0.20f);
    }

    public void UpdateTargeter()
    {
        float minDist = 10000000000f;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0) {
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
        } else
        {
            //Debug.Log("Ding");
            // attacks a random limb in the enemy
            EnemyIdentifierIdentifier[] limbs = targetEid.GetComponentsInChildren<EnemyIdentifierIdentifier>();
            target = limbs[UnityEngine.Random.Range(0, limbs.Length)].gameObject;
        }
        Invoke("UpdateTargeter", 0.25f);
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
        if ((Time.time - startTime > 1000f) || (speed > 200f))
        {
            Die();
        }
        //transform.position = Vector3.MoveTowards(transform.position, crazyPos, Time.deltaTime * speed);
    }

    Vector3 RandomVec3(float min, float max)
    {
        float randx = UnityEngine.Random.Range(min, max);
        float randy = UnityEngine.Random.Range(min, max);
        float randz = UnityEngine.Random.Range(min, max);
        return new Vector3(randx, randy, randz);
    }
}
