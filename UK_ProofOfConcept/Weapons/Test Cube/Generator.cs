using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject fred;
    private Vector3 startPos;
    private float lifeTimer = 0;
    private float speed = 1;
    [SerializeField] private float rotVel = 15f;
    [SerializeField] private float deathTimer = 15;
    [SerializeField] private float spawnTimer = 0.25f;
    [SerializeField] private float spawnDelay = 1;
    void Start()
    {
        startPos = transform.position;
        InvokeRepeating("SpawnFred", spawnTimer, spawnDelay + 0.5f);
        Destroy(gameObject, deathTimer);
    }

    void Update()
    {
        transform.Rotate(0, rotVel * Time.deltaTime, 0, Space.Self);
        transform.position = Vector3.Lerp(startPos + Vector3.up, startPos + Vector3.down, 0.5f + (Mathf.Sin(lifeTimer) / 2f));
        lifeTimer += Time.deltaTime;
    }

    void SpawnFred()
    {
        Instantiate(fred, transform.position, Quaternion.identity);
    }
}
