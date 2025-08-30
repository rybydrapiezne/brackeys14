using TMPro;
using UnityEngine;

public class OnHoverPopup : MonoBehaviour
{
    private static OnHoverPopup instance;

    public static OnHoverPopup Instance
    {
        get { return instance; }
    }

    public Vector2 pixelOffset = new Vector2(-20, -20);

    public GameObject panel;
    public TextMeshProUGUI textField;
    RectTransform textFieldRect;

    private Canvas canvas;

    RectTransform rectTransform;
    RectTransform canvasRect;

    public void showPopup(string text)
    {
        textField.text = text;
        gameObject.SetActive(true);
    }

    public void hidePopup()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        hidePopup();

        rectTransform = panel.GetComponent<RectTransform>();

        textFieldRect = textField.GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            Debug.LogError("OnHoverPopup: No Canvas found");

        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 screenPoint = Input.mousePosition;

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out localPoint);

        localPoint += pixelOffset;

        localPoint -= textFieldRect.sizeDelta / 2;

        //Debug.Log("mouse pos " + screenPoint);

        //Debug.Log("rect " + canvasRect.rect);

        // Clamp should be reworked
        if(screenPoint.x < textFieldRect.sizeDelta.x / 2 - pixelOffset.x)
        {
            localPoint.x += textFieldRect.sizeDelta.x - 2 * pixelOffset.x;
        }

        if (screenPoint.y < textFieldRect.sizeDelta.y / 2 - pixelOffset.y)
        {
            localPoint.y += textFieldRect.sizeDelta.y - 2 * pixelOffset.y;
        }

        rectTransform.anchoredPosition = localPoint;
    }
}
