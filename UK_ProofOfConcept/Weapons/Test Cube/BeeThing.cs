using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BeeThing : MonoBehaviour
{
    //public static UKAsset<GameObject> UK_Boom { get; private set; } = new UKAsset<GameObject>("Assets/Prefabs/Attacks and Projectiles/Coin.prefab");
    private Vector3 startPos;
    private Vector3 crazyPos;
    [SerializeField] private float factor = 0.5f;
    [SerializeField] private float bound = 1.5f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float deathTimer = 15f;
    void Start()
    {
        startPos = transform.position;
        crazyPos = startPos;
        Invoke("Die", deathTimer);
    }

    void Die()
    {
        /*GameObject gmCoin = Instantiate<UnityEngine.GameObject>(UK_Boom.Asset, base.transform.position, Quaternion.identity);
        Coin coin = gmCoin.GetComponent<Coin>();
        coin.Invoke("DelayedPunchflection", 0.101f);*/
        Destroy(base.gameObject);
    }

    void Update()
    {
        crazyPos += RandomVec3(-bound * Time.deltaTime, bound * Time.deltaTime);
        float blend = Mathf.Pow(factor, Time.deltaTime * speed);
        base.transform.position = Vector3.Lerp(crazyPos, base.transform.position, blend);
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
