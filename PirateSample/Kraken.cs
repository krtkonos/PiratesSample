using Pirates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Kraken : MonoBehaviour
{
    private NavMeshAgent _KrakenNav;
    private Transform _playerShip;
    private Transform _TargetShip;
    private int _KrakenDMG = 35;
    private float _maxPos = 1220;
    private int _areaMask = 0; // the area mask for the NavMeshArea
    public List<Transform> _EShipArray = new List<Transform>();
    private bool stopcoroutine = false;
    private void Awake()
    {
        _KrakenNav = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        _playerShip = PlayerShip.Instance.transform;
        _EShipArray = ShipBattleCamera.Instance._EnemyShipArray;
    }


    private void Update()
    {       
        NavMeshHit hit;
        FindNearest();
        if (!_TargetShip)
        {
            return;
        }
        if (NavMesh.SamplePosition(_TargetShip.position, out hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            Vector3 nearestPoint = hit.position;
            NavMesh.FindClosestEdge(nearestPoint, out hit, NavMesh.AllAreas);
            Vector3 closestEdge = hit.position;
            _KrakenNav.destination = closestEdge;
        }
        else
        {
            _KrakenNav.destination = _TargetShip.position;
        }
    }
    private void FindNearest()
    {
        float result = 0;
        float distance = 0;
        for (int i = 0; i < _EShipArray.Count; i++)
        {
            if (_EShipArray[i] == null)
            {
                continue;
            }
            distance = Vector3.Distance(gameObject.transform.position, _EShipArray[i].position);
            if (result > distance || result == 0)
            {
                _TargetShip = _EShipArray[i];
                result = distance;
            }
        }
        if (_playerShip != null)
        {
            distance = Vector3.Distance(gameObject.transform.position, _playerShip.position);
            if (result > distance)
            {
                _TargetShip = _playerShip;
                result = distance;
            }
            if (!_TargetShip)
            {
                _TargetShip = _playerShip;
            }
        }
    }
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
                hp.GetDmg(_KrakenDMG);
            }
        }
    }
}
