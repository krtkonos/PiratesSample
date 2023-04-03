using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.WSA;


namespace Pirates
{
    public class PlayerShip : ShipBase
    {
        public Camera               _Camera = null;
        public ShipController       _Ship = null;
        public ShipCombatController _CombatController = null;
        public SpriteRenderer       _Mark = null;

        private float _HideMarkTime = 0f;
        private LayerMask _EnemyLayer;
        private LayerMask _ProjectileLayer;
        private LayerMask _Enviro;
        private LayerMask _Land;
        [SerializeField] private Button Button1;
        [SerializeField] private Button Button2;
        private GameObject _EnemySpawnPoint;
        public Transform[] EnemySpawnPoints;
        public List<GameObject> SailsType = new List<GameObject>();
        [SerializeField] private GameObject _DirLeft;
        [SerializeField] private GameObject _DirRight;
        private GameObject _ShotDirections;

        [HideInInspector] public float _CooldownActualTimerRight;
        [HideInInspector] public float _CooldownActualTimerLeft;
        private float _CooldownTime = 3f;
        [HideInInspector] public bool _IsOnCooldownRight;
        [HideInInspector] public bool _IsOnCooldownLeft;

        [SerializeField] private Image _CannonBallRight;
        [SerializeField] private Image _CannonBallLeft;     
        [SerializeField] private Image _HealthStatus;     
        private float _HealthStatusWidth;       
        private float _borderNum;
        private int _BoatOrder;

        private const string _BoatsText = "Boats/";
        private const string _SloopText = _BoatsText + "Pirate_Boat_Medium_01";
        private const string _BrigantineText = _BoatsText + "SM_Veh_Veh_Boat_Large_01_Hull";
        private const string _FrigateText = _BoatsText + "SM_Veh_Boat_Warship_01_Hull";
        private const string _Man_o_WarText = _BoatsText + "SM_Veh_Boat_Warship_01_Hull_Upsized";
        private const string _QueenText = _BoatsText + "SM_Veh_Boat_Warship_01_Hull_Upsized";


        public static PlayerShip Instance { get;  set; }
        private void Awake()
        {
            Instance = this;
            if (GameControllerShip.Instance.ShipBattlePosition != null)
            {
                gameObject.transform.position = GameControllerShip.Instance.ShipBattlePosition.position;
            }
        }


        private void Start()
        {
            _Mark.enabled = false;
            _EnemyLayer = LayerMask.NameToLayer("EnemyShip");
            _ProjectileLayer = LayerMask.NameToLayer("Projectile");
            _Enviro = LayerMask.NameToLayer("Enviro");
            _Land = LayerMask.NameToLayer("Land");
            _CooldownActualTimerRight = _CooldownTime; 
            _CooldownActualTimerLeft = _CooldownTime;
            GetReferences();
            _HealthStatusWidth = _HealthStatus.rectTransform.rect.width;
            _borderNum = _HealthStatus.rectTransform.offsetMax.x;
            ShowShipHP();
        }

        private void Update()
        {            
            CooldownRight();
            CooldownLeft();
            _HideMarkTime -= Time.deltaTime;
            if (_Mark.enabled && _HideMarkTime > 0f)
            {
                Color color = _Mark.color;
                color.a = Mathf.Sin(_HideMarkTime);
                _Mark.color = color;
            }

            Vector3 position = Vector3.zero;
#if UNITY_EDITOR
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            position = Input.mousePosition;
#else
            if (Input.touchCount == 0)
            {
                return;
            }

            position = Input.touches[0].position;
#endif

            Ray ray2 = _Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(position, ray2.direction, Mathf.Infinity, LayerMask.GetMask("UI"));
            if (hit2D)
            {
                if (hit2D.collider.gameObject == Button1.gameObject || hit2D.collider.gameObject == Button2.gameObject)
                {
                    return;
                }
            }


            Ray ray = _Camera.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _Ship.SetTargetPosition(hit.point);
                Vector3 pos = hit.point;
                pos.y = 0f;
                _Mark.transform.position = pos;
                _Mark.enabled = true;
                _HideMarkTime = 3f;
            }            
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_EnemyLayer == other.gameObject.layer)
            {
                ShipAI AI = other.GetComponent<ShipAI>();
                if (ShipLevel > AI.ShipLevel)
                {
                    Debug.Log("My ship won");
                    AI.SeaBattleCrash();
                }
                else if (ShipLevel < other.GetComponent<ShipAI>().ShipLevel)
                {
                    Debug.Log("My ship lost");
                    ShipDestroyed();
                }
                else
                {
                    Debug.Log("Battle");
                    GameControllerShip.Instance.SaveMyShip();
                }
            }
            else if(other.gameObject.layer == _ProjectileLayer)
            {
                GetDmgFromProjectile(other);
                ShowShipHP();
            }
            else if (other.gameObject.layer == _Land)
            {
                Debug.Log("Land");
                GetDmg(ShipHP);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == _Enviro)
            {
                ShowShipHP();
            }
        }

        private void GetReferences()
        {
            //Transform _EnemySpawnPointTransform = transform.Find("EnemySpawnPoints");
            //EnemySpawnPoints = _EnemySpawnPointTransform.GetComponentsInChildren<Transform>();
            //_ShotDirections = transform.Find("Direction").gameObject;
            //_DirLeft = _ShotDirections.transform.Find("DirLeft").gameObject;
            //_DirRight = _ShotDirections.transform.Find("DirRight").gameObject;
            _DirLeft.SetActive(false);
            _DirRight.SetActive(false);

            SpawnEnemyBoats(4,2);
        }

        private void SpawnEnemyBoats(int SloopCount = 5, int BrigantineCount = 0, int FrigateCount = 0, int Man_o_WarCount = 0, int The_Queens_Anne_RevengeCount = 0)
        {           
            List<GameObject> boats = GameControllerShip.Instance.EnemyShips;
            _BoatOrder = 0;

            if (boats != null || boats.Count == 0)
            {
                /*for (int i = 1; i < SloopCount + 1; i++)
                {
                    GameObject BoatTOList = Instantiate(pirateEnemyBoatSloop, EnemySpawnPoints[i].position, Quaternion.identity);
                    ShipBattleCamera.Instance._EnemyShipArray.Add(BoatTOList.transform);
                    boats.Add(BoatTOList);
                }*/
                SpawnShipType(_SloopText, SloopCount);
                SpawnShipType(_BrigantineText, BrigantineCount);
                SpawnShipType(_FrigateText, FrigateCount);
                SpawnShipType(_Man_o_WarText, Man_o_WarCount);
                SpawnShipType(_QueenText, The_Queens_Anne_RevengeCount);
            }
            else
            {                
                for (int i = 1; i < boats.Count; i++)
                {
                    Instantiate(boats[i], EnemySpawnPoints[i].position, Quaternion.identity);
                }
            }
            
        }
        private void OnDestroy()
        {
            ShipBattleWinningScreen.FadeEvent?.Invoke(false);
            GameControllerShip.Instance.newBattle = true;
            GameControllerShip.Instance.EnemyShips.Clear();
            ShipBattleCamera.Instance._EnemyShipArray.Clear();
        }

        public void ShowDirectionLight(bool right)
        {
            if (right)
            {
                _DirRight.SetActive(true);
            }
            else
            {
                _DirLeft.SetActive(true);
            }
        }
        public void HideDirectionLight(bool right)
        {
            if (right)
            {
                _DirRight.SetActive(false);
            }
            else
            {
                _DirLeft.SetActive(false);
            }
        }
        private void CooldownRight()
        {
            _CooldownActualTimerRight += Time.deltaTime;
            if (_CooldownActualTimerRight >= _CooldownTime)
            {
                _CannonBallRight.fillAmount = 1;
                _IsOnCooldownRight = false;
            }
            else
            {
                _CannonBallRight.fillAmount = _CooldownActualTimerRight / _CooldownTime;
            }
        }
        private void CooldownLeft()
        {
            _CooldownActualTimerLeft += Time.deltaTime;
            if (_CooldownActualTimerLeft >= _CooldownTime)
            {
                _CannonBallLeft.fillAmount = 1;
                _IsOnCooldownLeft = false;
            }
            else
            {
                _CannonBallLeft.fillAmount = _CooldownActualTimerLeft / _CooldownTime;
            }
        }
        public void ShowShipHP()
        {
            float percentageHp = ((float)ShipHP / (float)_ShipMaxHP) * 100;
            _HealthStatus.rectTransform.offsetMax = new Vector2((-_HealthStatusWidth - ((-_HealthStatusWidth / 100) * percentageHp)) + _borderNum, _HealthStatus.rectTransform.offsetMax.y);
        }

        private void SpawnShipType(string shipName, int count)
        {
            if (count != 0 || _BoatOrder + count > EnemySpawnPoints.Length)
            {
                GameObject spawnedBoat = Resources.Load(shipName) as GameObject;
                for (int i = 0; i < count; i++)
                {
                    GameObject BoatTOList = Instantiate(spawnedBoat, EnemySpawnPoints[i + _BoatOrder].position, Quaternion.identity);
                    ShipBattleCamera.Instance._EnemyShipArray.Add(BoatTOList.transform);
                    GameControllerShip.Instance.EnemyShips.Add(BoatTOList);
                }
            }
            _BoatOrder += count;
        }
    }
}
