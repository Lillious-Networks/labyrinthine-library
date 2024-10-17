using UnityEngine;
using MelonLoader;
using UnityEngine.UI;

namespace labyrinthine_library.UI;

public class UIElement : MelonMod
{
    public Canvas? canvas;

    public Canvas? Draw(Vector2 position, GameObject element, Canvas? parent)
    {

        if (parent != null)
        {
            canvas = parent;
        }
        else
        {
            canvas = new GameObject("Labyrinthine Mod Menu").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999999;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        GameObject uiElement = UnityEngine.Object.Instantiate(element, canvas.transform);
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }

        return canvas;
    }
}
