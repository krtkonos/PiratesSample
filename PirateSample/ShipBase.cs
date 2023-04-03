using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pirates
{    
    public class ShipBase : MonoBehaviour
    {
        public int ShipHP;
        public int _ShipMaxHP;
        public int Armor;
        public int ShipLevel;
        private int _MinDmg;
        public int DmgProjectile;

        private float _ExplosionSize = 10;
        private bool _LowHP = false;

        [SerializeField] private GameObject _Explosion = null;
        [SerializeField] private Transform _MainSail = null;
        public enum ShipType
        {
            Sloop = 1,
            Brigantine = 2,
            Frigate = 3,
            Man_o_War = 4,
            The_Queens_Anne_Revenge = 5
        }
        public ShipType ShipStyle;

        // Start is called before the first frame update
        void Start()
        {
            ShipHP = _ShipMaxHP;
        }

        // Update is called once per frame
        void Update()
        {

        }
        protected void GetDmgFromProjectile(Collider collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {                
                int projectileDmg = collider.GetComponent<DestroyAfterCollision>().dmg;
                int finalDmg = (projectileDmg - Armor <= _MinDmg) ? _MinDmg : projectileDmg;
                GetDmg(finalDmg);
            }
        }
        public void GetDmg(int dmg)
        {
            if (ShipHP - dmg > 0)
            {
                ShipHP -= dmg;
            }
            else
            {
                ShipHP = 0;
                ShipDestroyed();
            }
            if (ShipHP < _ShipMaxHP / 10 && !_LowHP)
            {
                if (_MainSail != null && _Explosion != null)
                {
                    Instantiate(_Explosion, transform.position, Quaternion.identity);
                    StartCoroutine(SailsFallingCo(_MainSail));
                }
                gameObject.GetComponent<ShipController>()._MaxSpeed /= 2;
                _LowHP = true;
            }
        }
        protected void ShipDestroyed()
        {
            GameObject expl = Instantiate(_Explosion, transform.position,Quaternion.identity);
            expl.transform.localScale = new Vector3(_ExplosionSize, _ExplosionSize, _ExplosionSize);
            StartCoroutine(DestroyCo());
        }
        private IEnumerator SailsFallingCo(Transform sails)
        {
            float time = 2.5f;
            float elapsedTime = 0;
            Quaternion startRotation = sails.rotation;
            Quaternion endRotation = Quaternion.Euler(0, 0, 100);
            while (elapsedTime < time)
            {
                sails.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1);
            Destroy(sails.gameObject);
        }
        protected IEnumerator DestroyCo()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}

