using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GunsOPlenty.Trials.UI
{
    public class TrialInfoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // "these can't be listeners because... THEY JUST CAN'T, OKAY"
    {
        public GameObject thingymajig;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (thingymajig != null && !thingymajig.activeInHierarchy)
            {
                thingymajig.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (thingymajig != null && thingymajig.activeInHierarchy)
            {
                thingymajig.SetActive(false);
            }
        }
    }
}
