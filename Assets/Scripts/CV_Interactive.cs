using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC
{
    public class CV_Interactive : MonoBehaviour
    {
        [SerializeField] GameObject miniGame;
        GameObject highlight;

        private void OnEnable()
        {
            highlight = transform.GetChild(0).gameObject;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                highlight.SetActive(true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                highlight.SetActive(false);
            }
        }
    }
}
