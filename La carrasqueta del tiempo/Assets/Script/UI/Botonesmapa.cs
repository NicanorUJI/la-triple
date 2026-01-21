using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHoverImage : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Image to Toggle")]
    public GameObject imageToToggle;

    void Start()
    {
        if (imageToToggle != null)
            imageToToggle.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageToToggle != null)
            imageToToggle.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageToToggle != null)
            imageToToggle.SetActive(false);
    }
}
