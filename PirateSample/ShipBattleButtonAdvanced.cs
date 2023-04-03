using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShipBattleButtonAdvanced : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent onButtonHold;
    public UnityEvent onButtonRelease;
    [SerializeField] private GameObject _Red;
    private bool isPointerOver;
    private float _CooldownActualTimer;
    private float _CooldownTime = 3f;
    [HideInInspector] public bool _IsOnCooldown = false;
    [SerializeField] private Image _CannonBall;


    private void Start()
    {
        _Red.SetActive(false);
        _CooldownActualTimer = _CooldownTime;
    }
    private void Update()
    {
        Cooldown();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerOver = true;        
        onButtonHold.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) //IS it neccesary, if the fire do not fire whe I release the click outside the button?
    {
        onButtonRelease.Invoke();
        if (isPointerOver)
        {
            if (!_IsOnCooldown)
            {
                _CooldownActualTimer = 0;
                _IsOnCooldown = true;
            }
        }       
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;        
    }

    public void OnButtonHold()
    {
        _Red.SetActive(true);
    }

    public void OnButtonRelease()
    {
        _Red.SetActive(false);
    }
    private void Cooldown()
    {
        _CooldownActualTimer += Time.deltaTime;
        if (_CooldownActualTimer >= _CooldownTime)
        {
            _CannonBall.fillAmount = 1;
            _IsOnCooldown = false;
        }
        else
        {
            _CannonBall.fillAmount = _CooldownActualTimer / _CooldownTime;
        }
    }
}
