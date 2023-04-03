using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pirates
{
    public class ShipCombatController : MonoBehaviour
    {
        [SerializeField] private Transform  _ShootPoint = null;
        [SerializeField] private float      _ShootOffset = 7f;
        [SerializeField] private int        _Cannons = 5;
        [SerializeField] private Transform  _Target = null;
        [SerializeField] private GameObject _Projectile = null;
        [SerializeField] private GameObject _CannonFX = null;
        private PlayerShip _PlayerShip;

        private int     _CannonsReady = 0;
        private float   _ReloadTime = 0;
        private int _DmgProjectile;

        private void Start()
        {
            
            _CannonsReady = _Cannons;
            if (gameObject.GetComponent<ShipAI>() != null)
            {
                _DmgProjectile = gameObject.GetComponent<ShipAI>().DmgProjectile;
                _Target = PlayerShip.Instance.transform;
            }
            if (gameObject.GetComponent<PlayerShip>() != null)
            {
                _PlayerShip = GetComponent<PlayerShip>();
                _DmgProjectile = _PlayerShip.DmgProjectile;
            }
        }

        private void Update()
        {
            _ReloadTime += Time.deltaTime;

            //if (_ReloadTime > 1f)
            {
                _CannonsReady++;
                _CannonsReady = Mathf.Clamp(_CannonsReady, 0, _Cannons);
                _ReloadTime = 0f;
            }
        }

        /// <summary>
        /// AI fire
        /// </summary>
        public void Fire() 
        {
            Vector3 dir = (_Target.position - transform.position).normalized;
            float dot = Vector3.Dot(dir, _ShootPoint.forward);
            Vector3 finalDir = _ShootPoint.forward;

            if(dot < 0f)
            {
                finalDir *= -1f;
            }

            StartCoroutine(FireCannons(_CannonsReady, finalDir));
            _CannonsReady = 0;
        }

        /// <summary>
        /// Player Fire
        /// </summary>
        /// <param name="pDirection"></param>
        public void Fire(int pDirection)
        {
            //Vector3 dir = (_Target.position - transform.position).normalized;
            //float dot = Vector3.Dot(dir, _ShootPoint.forward);
            Vector3 finalDir = _ShootPoint.forward;

            if (pDirection == 1)
            {
                finalDir *= -1f;
                if (!_PlayerShip._IsOnCooldownLeft)
                {
                    StartCoroutine(FireCannons(_CannonsReady, finalDir));
                    _CannonsReady = 0;
                    _PlayerShip._CooldownActualTimerLeft = 0;
                    _PlayerShip._IsOnCooldownLeft = true;
                }
            }
            else
            {                
                if (!_PlayerShip._IsOnCooldownRight)
                {
                    StartCoroutine(FireCannons(_CannonsReady, finalDir));
                    _CannonsReady = 0;
                    _PlayerShip._IsOnCooldownRight = true;
                    _PlayerShip._CooldownActualTimerRight = 0;
                }
            }

            /*if(dot < 0f)
            {
                finalDir *= -1f;
            }*/
        }

        private IEnumerator FireCannons(int pCannons, Vector3 pFinalDir)
        {
            int start = _Cannons * -1;
            for (int i = 0; i < pCannons; i++)
            {
                Vector3 position = _ShootPoint.position
                                   + _ShootPoint.right * Random.Range(start - 2f, start + 2f)
                                   + pFinalDir * Random.Range(_ShootOffset - 0.5f, _ShootOffset + 1f);

                GameObject go = Instantiate(_Projectile, position, Quaternion.identity);
                Rigidbody rb = go.GetComponent<Rigidbody>();
                rb.AddForce(pFinalDir * 70f, ForceMode.Impulse);
                start++;
                start++;
                DestroyAfterCollision dmg = go.GetComponent<DestroyAfterCollision>();
                dmg.dmg = _DmgProjectile;

                go = Instantiate(_CannonFX, position, Quaternion.identity);
                go.transform.forward = pFinalDir;

                yield return new WaitForSecondsRealtime(0.01f);
            }

            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}

