using GunsOPlenty.Data;
using GunsOPlenty.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKILL.Cheats;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject fred;
    public GameObject sourceWeapon;
    public List<GameObject> freds = new List<GameObject>();
    private Vector3 startPos;
    private float lifeTimer = 0;
    private float speed = 1;
    public float rotVel = 15f;
    private float startTime;
    public float deathTime;
    private float spawnLoopTime;
    private bool firstDeath; // TODO: without this, the fixed update loop will sometimes call die multiple times. Make this not a thing
    EnemyColliderDetector ecd;
    void Start()
    {
        //fred = AssetHandler.beething;
        deathTime = 30f;
        firstDeath = true;
        startPos = transform.position;
        Invoke("SpawnLoop", 1 + 0.5f);
        ecd = base.gameObject.AddComponent<EnemyColliderDetector>();
        if (ecd == null)
        {
            Debug.Log("Dang");
        }
        startTime = Time.time;
        spawnLoopTime = Time.time;
    }

    public void SpawnLoop()
    {
        if (freds.Count < 10)
        {
            SpawnFred();
        }
        for (int i = 0; i < freds.Count; i++)
        {
            if (freds[i] == null)
            {
                freds.Remove(freds[i]);
            }
        }
        Invoke("SpawnLoop", (2.5f - ((rotVel) * 0.0008f)));
    }

    public void Die()
    {
        Destroy(base.gameObject);
        if (rotVel > 2000f)//2520f
        {
            Instantiate<GameObject>(PrefabBox.superBoom, base.transform.position, Quaternion.identity);
        }
        for (int i = 0; i < freds.Count; i++)
        {
            freds[i].GetComponent<BeeThing>().Die();
        }
        freds.Clear();
    }

    void FixedUpdate()
    {
        transform.Rotate(0, rotVel * Time.deltaTime, 0, Space.Self);
        transform.position = Vector3.Lerp(startPos + Vector3.up, startPos + Vector3.down, 0.5f + (Mathf.Sin(lifeTimer) / 2f));
        lifeTimer += Time.deltaTime;
        if (NoWeaponCooldown.NoCooldown) startTime = Time.time;
        if (((Time.time - startTime > deathTime) || (rotVel > 2880f)) && firstDeath)
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

    void SpawnFred()
    {
        GameObject newFred = Instantiate(fred, transform.position, Quaternion.identity);
        newFred.GetComponent<BeeThing>().sourceWeapon = sourceWeapon;
        newFred.GetComponent<BeeThing>().speed = (15f + rotVel * 0.1f);
        freds.Add(newFred);
    }
}
