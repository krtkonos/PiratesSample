using Pirates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBattleWinningScreen : MonoBehaviour
{
    [SerializeField] private GameObject _WinningScreen;
    private Image _WinningScreenImage;
    public float duration = 2f;
    [SerializeField] private Text _text;
    public static Action<bool> FadeEvent;
    List<Transform> ships;
    private void Start()
    {
        GetRefferences();
        _WinningScreenImage.color = new Color(59, 78, 255, 0);
        _WinningScreen.SetActive(false);
    }
    private void OnEnable()
    {
        FadeEvent += FadeIn;
    }
    private void GetRefferences()
    {
        //_WinningScreen = transform.Find("WinningScren").gameObject;
        //_text = _WinningScreen.transform.Find("WinText").GetComponent<Text>();
        _WinningScreenImage = _WinningScreen.GetComponent<Image>();
    }
    public void FadeIn(bool win)
    {        
        _WinningScreen.SetActive(true);
        _text.text = win ? "Win" : "Loose";
        StartCoroutine(Fade(0, 1, win));
    }

    IEnumerator Fade(float start, float end, bool win)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            _WinningScreenImage.color = win ? new Color(59 / 255f, 78 / 255f, 255 / 255f, alpha) : new Color(219 / 255f, 28 / 255f, 28 / 255f, alpha);
            /*_WinningScreenImage.color = new Color(59/255f, 78/255f, 255/255f, alpha);
            _WinningScreenImage.color = new Color(219/255f, 28/255f, 28/255f, alpha);*/
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
