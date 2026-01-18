using UnityEngine;
using UnityEngine.UI;

public class UIImageToggleHideCanvas_Inspector : MonoBehaviour
{
    public Image targetImage;
    public Sprite spriteA;
    public Sprite spriteB;
    public GameObject canvasToHide;

    public void OnButtonPressed()
    {
        if (targetImage.sprite == spriteA)
        {
            targetImage.sprite = spriteB;
        }
        else if (targetImage.sprite == spriteB)
        {
            canvasToHide.SetActive(false);
        }
    }
}
