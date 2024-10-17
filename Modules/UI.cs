using UnityEngine;
using MelonLoader;

namespace labyrinthine_library.UI;

public class UIElement : MelonMod
{
    public GameObject? uiElement;

    public void Draw(Vector2 position, GameObject element, GameObject? parent)
    {
        
        if (parent != null)
        {
            uiElement = UnityEngine.Object.Instantiate(element, parent.transform);

        } else
        {
            Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                MelonLogger.Error("No Canvas found in the scene.");
                return;
            }

            uiElement = UnityEngine.Object.Instantiate(element, canvas.transform);

        }

        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
        else
        {
            MelonLogger.Error("No RectTransform found on the UI element.");
        }
    }
}
