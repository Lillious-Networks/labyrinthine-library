using Il2CppTMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace labyrinthine_library.Modules
{
    public enum ToastType { Info, Success, Warning, Error }

    public class ToastNotification : MonoBehaviour
    {
        private float toastWidth = 320f;
        private float toastHeight = 64f;
        private float padding = 16f;
        private float spacing = 8f;
        private int maxVisible = 5;

        private float displayDuration = 3f;
        private float animateInDuration = 0.35f;
        private float animateOutDuration = 0.25f;

        // Delay between each toast's slide-in when multiple arrive at once
        private float staggerDelay = 0.08f;

        private int fontSize = 14;

        private static readonly Color ColInfo = new Color(0.20f, 0.60f, 1.00f);
        private static readonly Color ColSuccess = new Color(0.18f, 0.80f, 0.44f);
        private static readonly Color ColWarning = new Color(1.00f, 0.72f, 0.00f);
        private static readonly Color ColError = new Color(0.95f, 0.26f, 0.26f);
        private static readonly Color ColBg = new Color(0.10f, 0.10f, 0.12f, 0.95f);
        private static readonly Color ColText = new Color(0.95f, 0.95f, 0.95f);

        private Canvas? _canvas;
        private RectTransform? _container;
        private readonly Queue<(string msg, ToastType type)> _queue = new();
        private readonly List<RectTransform> _active = new();
        private bool _initialized;

        // Tracks how many toasts have been added since the last TryDequeue batch,
        // so each one gets a progressively larger stagger offset.
        private int _staggerIndex = 0;

        public void Initialize()
        {
            if (_initialized) return;
            _canvas = CreateCanvas();
            _container = CreateContainer();
            _initialized = true;
        }

        public void Show(string message, ToastType type = ToastType.Info)
        {
            MelonLoader.MelonCoroutines.Start(InitToastCoroutine(message, type));
        }

        public void ShowMessage(string message, ToastType type = ToastType.Info)
        {
            if (!_initialized) Initialize();
            _queue.Enqueue((message, type));
            TryDequeue();
        }

        public void Info(string msg) => Show(msg, ToastType.Info);
        public void Success(string msg) => Show(msg, ToastType.Success);
        public void Warning(string msg) => Show(msg, ToastType.Warning);
        public void Error(string msg) => Show(msg, ToastType.Error);

        private void TryDequeue()
        {
            // Drain as many queued toasts as the visible cap allows,
            // assigning each an increasing stagger index so they slide in
            // one after another rather than all at once.
            while (_queue.Count > 0 && _active.Count + _staggerIndex < maxVisible)
            {
                var (msg, type) = _queue.Dequeue();
                MelonLoader.MelonCoroutines.Start(ShowCoroutine(msg, type, _staggerIndex));
                _staggerIndex++;
            }

            // Reset for the next batch once the coroutines have had a frame
            // to add themselves to _active.
            MelonLoader.MelonCoroutines.Start(ResetStaggerIndex());
        }

        private IEnumerator ResetStaggerIndex()
        {
            yield return null; // wait one frame
            _staggerIndex = 0;
        }

        private IEnumerator ShowCoroutine(string message, ToastType type, int stagger)
        {
            // Wait for this toast's stagger slot before doing anything visible.
            if (stagger > 0)
                yield return new WaitForSeconds(stagger * staggerDelay);

            RectTransform toast = BuildToast(message, type);
            _active.Add(toast);
            RepositionActive(instant: true);

            yield return MelonLoader.MelonCoroutines.Start(AnimateIn(toast));

            float elapsed = 0f;
            while (elapsed < displayDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return MelonLoader.MelonCoroutines.Start(AnimateOut(toast));

            _active.Remove(toast);
            Destroy(toast.gameObject);

            // Slide remaining toasts down to fill the gap.
            RepositionActive(instant: false);

            TryDequeue();
        }

        private IEnumerator AnimateIn(RectTransform toast)
        {
            if (toast == null) yield break;

            float startX = toastWidth + padding;
            float endX = 0f;
            float t = 0f;

            var cg = toast.GetComponent<CanvasGroup>();

            while (t < animateInDuration)
            {
                if (toast == null) yield break;
                t += Time.deltaTime;
                float p = EaseOutCubic(Mathf.Clamp01(t / animateInDuration));
                toast.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, p), toast.anchoredPosition.y);
                if (cg != null) cg.alpha = p;
                yield return null;
            }

            if (toast == null) yield break;
            toast.anchoredPosition = new Vector2(endX, toast.anchoredPosition.y);
            if (cg != null) cg.alpha = 1f;
        }

        private IEnumerator AnimateOut(RectTransform toast)
        {
            if (toast == null) yield break;

            float startX = toast.anchoredPosition.x;
            float endX = toastWidth + padding;
            float t = 0f;
            var cg = toast.GetComponent<CanvasGroup>();

            while (t < animateOutDuration)
            {
                if (toast == null) yield break;
                t += Time.deltaTime;
                float p = EaseInCubic(Mathf.Clamp01(t / animateOutDuration));
                toast.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, p), toast.anchoredPosition.y);
                if (cg != null) cg.alpha = 1f - p;
                yield return null;
            }
        }

        /// <summary>
        /// Positions each active toast so that index 0 is closest to the screen
        /// edge and higher indices stack downward — i.e. new toasts appear at the
        /// top and push older ones toward the bottom as more arrive.
        ///
        /// Because the container is anchored to the bottom-right with its pivot
        /// also at the bottom-right, negative Y values move upward on screen.
        /// We invert the index so the newest toast sits at Y = 0 (the anchor)
        /// and earlier toasts are pushed to increasingly negative Y.
        /// </summary>
        private void RepositionActive(bool instant)
        {
            int count = _active.Count;
            for (int i = 0; i < count; i++)
            {
                // i == 0 is the newest (topmost on screen when stacking down).
                // Each subsequent toast sits one slot below the previous one.
                float targetY = -i * (toastHeight + spacing);

                if (instant)
                    _active[i].anchoredPosition = new Vector2(_active[i].anchoredPosition.x, targetY);
                else
                    MelonLoader.MelonCoroutines.Start(SmoothMove(_active[i], targetY));
            }
        }

        private IEnumerator SmoothMove(RectTransform rt, float targetY)
        {
            if (rt == null) yield break;

            float startY = rt.anchoredPosition.y;
            float t = 0f, dur = 0.2f;
            while (t < dur)
            {
                if (rt == null) yield break;
                t += Time.deltaTime;
                float p = EaseOutCubic(Mathf.Clamp01(t / dur));
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(startY, targetY, p));
                yield return null;
            }

            if (rt == null) yield break;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, targetY);
        }

        private Canvas CreateCanvas()
        {
            var go = new GameObject("Labyrinthine - Toast Canvas " + Guid.NewGuid());
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(go);
            return canvas;
        }

        private RectTransform CreateContainer()
        {
            var go = new GameObject("Toast Container");
            if (_canvas == null) throw new InvalidOperationException("Canvas not initialized");
            go.transform.SetParent(_canvas.transform, false);

            var rt = go.AddComponent<RectTransform>();
            // Anchor to top-right so toasts spill downward naturally.
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-padding, -padding);
            rt.sizeDelta = new Vector2(toastWidth, 0f);

            return rt;
        }

        private RectTransform BuildToast(string message, ToastType type)
        {
            var go = new GameObject("Toast_" + type);
            go.transform.SetParent(_container, false);

            var rt = go.AddComponent<RectTransform>();
            // Anchor/pivot to top-right so each toast measures downward from Y = 0.
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
            rt.sizeDelta = new Vector2(toastWidth, toastHeight);
            rt.anchoredPosition = new Vector2(toastWidth + padding, 0f); // starts off-screen right

            go.AddComponent<CanvasGroup>();

            var bg = go.AddComponent<Image>();
            bg.color = ColBg;

            // Accent bar on the left edge
            var barGo = new GameObject("AccentBar");
            barGo.transform.SetParent(go.transform, false);
            var barImg = barGo.AddComponent<Image>();
            barImg.color = TypeToColor(type);

            var barRt = barGo.GetComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0f, 0f);
            barRt.anchorMax = new Vector2(0f, 1f);
            barRt.pivot = new Vector2(0f, 0.5f);
            barRt.anchoredPosition = Vector2.zero;
            barRt.sizeDelta = new Vector2(4f, 0f);

            // Message label
            var lblGo = new GameObject("Label");
            lblGo.transform.SetParent(go.transform, false);
            var lbl = lblGo.AddComponent<TextMeshProUGUI>();
            lbl.text = message;
            lbl.fontSize = fontSize;
            lbl.color = ColText;
            lbl.alignment = TextAlignmentOptions.MidlineLeft;
            lbl.overflowMode = TextOverflowModes.Ellipsis;
            lbl.enableWordWrapping = false;

            var lblRt = lblGo.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0f, 0f);
            lblRt.anchorMax = new Vector2(1f, 1f);
            lblRt.offsetMin = new Vector2(12f, 0f);
            lblRt.offsetMax = new Vector2(-12f, 0f);

            MelonLoader.MelonCoroutines.Start(AnimateProgressBar(CreateProgressBar(go, TypeToColor(type))));

            return rt;
        }

        private Image CreateProgressBar(GameObject parent, Color accentColor)
        {
            var pbGo = new GameObject("ProgressBar");
            pbGo.transform.SetParent(parent.transform, false);
            var img = pbGo.AddComponent<Image>();
            img.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.35f);

            var rt = pbGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(0f, 3f);

            return img;
        }

        private IEnumerator AnimateProgressBar(Image bar)
        {
            if (bar == null) yield break;

            float elapsed = 0f;
            var rt = bar.GetComponent<RectTransform>();
            while (elapsed < displayDuration)
            {
                if (bar == null || rt == null) yield break;
                elapsed += Time.deltaTime;
                float fill = 1f - Mathf.Clamp01(elapsed / displayDuration);
                rt.anchorMax = new Vector2(fill, rt.anchorMax.y);
                yield return null;
            }

            if (rt != null)
                rt.anchorMax = new Vector2(0f, rt.anchorMax.y);
        }

        private Color TypeToColor(ToastType t) => t switch
        {
            ToastType.Success => ColSuccess,
            ToastType.Warning => ColWarning,
            ToastType.Error => ColError,
            _ => ColInfo,
        };

        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
        private static float EaseInCubic(float t) => t * t * t;

        private IEnumerator InitToastCoroutine(string message, ToastType type)
        {
            yield return new WaitForSeconds(0.1f);
            ShowMessage(message, type);
        }
    }
}