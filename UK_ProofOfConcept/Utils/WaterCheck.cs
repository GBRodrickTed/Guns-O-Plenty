using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    // Used to detect if a collider is in contact with water
    // apparently lava is just really hot water
    // apparently triggers can't interact with other triggers :|
    // TODO: Find a way to track submergion depth + Find a way to consistantly differenciate between lava and water
    public class WaterCheck : MonoBehaviour
    {
        List<Water> bucket = new List<Water>();
        public bool inWater = false;
        float slowUpdateSpeed = 0.5f;
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

        public void SlowUpdate()
        {
            CheckBucket();
            base.Invoke(nameof(WaterCheck.SlowUpdate), slowUpdateSpeed);
        }

        private void Enter(Collider other)
        {
            Water water;
            //Debug.Log("thing in : " + other.gameObject.name);
            if (other.gameObject.TryGetComponent<Water>(out water))
            {
                other.gameObject.TryGetComponent<Water>(out water);
                //Debug.Log("Water in");
                if (!bucket.Contains(water))
                {
                    
                    bucket.Add(water);
                    CheckBucket();
                }
            }
        }
        private void Exit(Collider other)
        {
            Water water;
            //Debug.Log("thing out : " + other.gameObject.name);
            if (other.gameObject.TryGetComponent<Water>(out water))
            {
                other.gameObject.TryGetComponent<Water>(out water);
                //Debug.Log("Water out");
                if (bucket.Contains(water))
                {
                    bucket.Remove(water);
                    CheckBucket();
                }
            }
        }

        public void CheckBucket()
        {
            if (bucket.Count > 0)
            {
                inWater = true;
            } else
            {
                inWater = false;
            }
        }
    }
}
