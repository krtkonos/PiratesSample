using Pirates;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    //public Transform target; // assign the target object in the Inspector //TODO can be deleted
    private List<Transform> targetArray = new List<Transform>(); // assign the target object in the Inspector
    //public Image indicatorImage; // assign the Image component in the Inspector //TODO can be deleted
    //public Image ArrowImage; // assign the Image component in the Inspector //TODO can be deleted
    public List<Image> ArrowImageArray = new List<Image>(); // assign the Image component in the Inspector
    public float distanceFromEdge = 10f; // distance from the edge of the screen to the indicator
    //public Transform arrow;
    [SerializeField] private List<Transform> arrowArr; 
    //public Transform arrow2;
    [SerializeField] private List<Transform> _pointerArray;
    public float change;

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private Vector2 screenCenter;

    void Start()
    {
        //rectTransform = ArrowImage.GetComponent<RectTransform>();
        parentRect = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        screenBounds = new Vector2(Screen.width, Screen.height);
        screenCenter = screenBounds / 2f;
        targetArray = ShipBattleCamera.Instance._EnemyShipArray;
        foreach (var item in ArrowImageArray)
        {
            item.enabled = false;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < targetArray.Count; i++)
        {
            if (targetArray[i] != null)
            {
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(targetArray[i].position);
                bool onScreen = viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
                RectTransform rectT = ArrowImageArray[i].GetComponent<RectTransform>();
                RectTransform rectParent = GetComponent<RectTransform>();

                ArrowImageArray[i].enabled = !onScreen;

                if (!onScreen)
                {
                    Vector2 indicatorPos = viewportPoint;
                    indicatorPos = indicatorPos * screenBounds - screenCenter;
                    rectT.anchoredPosition = indicatorPos;

                    // Clamp the indicator's position to the edge of the screen
                    rectT.anchoredPosition = new Vector2(
                        Mathf.Clamp(rectT.anchoredPosition.x, -rectParent.sizeDelta.x / 2 + rectT.sizeDelta.x / 2 + distanceFromEdge, rectParent.sizeDelta.x / 2 - rectT.sizeDelta.x / 2 - distanceFromEdge),
                        Mathf.Clamp(rectT.anchoredPosition.y, -rectParent.sizeDelta.y / 2 + rectT.sizeDelta.y / 2 + distanceFromEdge, rectParent.sizeDelta.y / 2 - rectT.sizeDelta.y / 2 - distanceFromEdge)
                    );

                    // Make sure the indicator is facing the camera
                    rectT.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);

                    if (PlayerShip.Instance != null)
                    {
                        _pointerArray[i].transform.position = PlayerShip.Instance.transform.position;
                    }

                    _pointerArray[i].LookAt(targetArray[i]);
                    change = _pointerArray[i].localEulerAngles.y * -1;
                    arrowArr[i].rotation = Quaternion.Euler(0, 0, change);

                }
            }
            else
            {
                ArrowImageArray[i].enabled = false;
            }
        }
        
    }
}
