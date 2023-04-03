using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pirates
{
    public class ShipAI : ShipBase
    {
        private PlayerShip           _PlayerShip = null;
        public ShipController       _Ship = null;
        public ShipCombatController _Combat = null;

        private float _NextTimeToFindTarget = 5f;
        private float _NextFire = 0f;
        public GameObject PositionArrow;
        private LayerMask _ProjectileLayer;
        private LayerMask _Enviro;
        private LayerMask _Land;

        private void Start()
        {
            _PlayerShip = PlayerShip.Instance;
            _Land = LayerMask.NameToLayer("Land");
            _ProjectileLayer = LayerMask.NameToLayer("Projectile");
        }
        private void Update()
        {
            _NextTimeToFindTarget -= Time.deltaTime;

            if (_NextTimeToFindTarget < 0f && _PlayerShip != null)
            {
                Vector3 targetPos = _PlayerShip.transform.position;
                Vector3 pos = transform.position;
                Vector3 dir = (targetPos - pos).normalized;
                float dot = Vector3.Dot(dir, transform.forward);
                float distance = Vector3.Distance(targetPos, pos);

                //tady hodit mozna range dostrelu
                if (distance > 100f)
                {
                    Vector3 offset = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
                    _Ship.SetTargetPosition(targetPos + offset);
                }

                _NextTimeToFindTarget = 1f;
            }

            TryFire();
        }

        private void TryFire()
        {
            _NextFire -= Time.deltaTime;

            if (_NextFire > 0f || !_PlayerShip)
            {
                return;
            }

            Vector3 targetPos = _PlayerShip.transform.position;
            Vector3 pos = transform.position;
            Vector3 dir = (targetPos - pos).normalized;
            float dot = Vector3.Dot(dir, transform.forward);

            if (Mathf.Abs(dot) < 0.2f)
            {
                _NextFire = Random.Range(3f, 8f);
                _Combat.Fire();
            }
        }
        public  void SeaBattleCrash()
        {
            //TODO crashanimation and efect, and destroy
            //Instantiate
            ShipDestroyed();
            //Destroy(gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _ProjectileLayer)
            {
                GetDmgFromProjectile(other);
            }
            else if (other.gameObject.layer == _Land)
            {
                GetDmg(ShipHP);
            }
        }
        private void OnDestroy()
        {
            List<Transform> ships = ShipBattleCamera.Instance._EnemyShipArray;
            ships.Remove(gameObject.transform);
            GameControllerShip.Instance.EnemyShips.Remove(gameObject);
            if (ships == null || ships.Count == 0)
            {
                ShipBattleWinningScreen.FadeEvent?.Invoke(true);
            }
        }
    }
}
