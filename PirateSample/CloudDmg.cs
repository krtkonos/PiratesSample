using Pirates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDmg : MonoBehaviour
{
    private bool stopcoroutine = false;
    private int _StormDmg = 15;
    private void OnTriggerEnter(Collider other)
    {
        stopcoroutine = false;
        ShipBase baseSHip = other.GetComponent<ShipBase>();
        StartCoroutine(DamegeBoatsCo(baseSHip));
    }
    private void OnTriggerExit(Collider other)
    {
        stopcoroutine = true;
    }
    private IEnumerator DamegeBoatsCo(ShipBase hp)
    {
        while (!stopcoroutine)
        {
            yield return new WaitForSeconds(0.1f);
            if (hp != null)
            {
                if (hp.ShipHP > 0)
                {
                    hp.GetDmg(_StormDmg);
                }                
            }
        }
    }
}
