using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Always-visible "POWER  available / maximum" indicator in the top-right
// corner of every scene that has a PowerManager.
//
// available = maxPower - usedPower, i.e. how much power the player can
// still route right now. It updates from PowerManager.Changed, so it
// reacts at once to terminals, unstable circuits and generator upgrades.
//
// The HUD builds itself when such a scene loads, reusing the objective
// text's font, size and colour, so it needs no per-scene setup. Scenes
// without a PowerManager (menus, Rooftop) get no HUD.
public class PowerHUD : MonoBehaviour
{
    private static readonly Color NormalColor = new Color(0.55f, 0.9f, 0.85f, 1f);
    private static readonly Color EmptyColor = new Color(1f, 0.6f, 0.25f, 1f);

    private PowerManager powerManager;
    private TMP_Text label;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        CreateForScene();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateForScene();
    }

    private static void CreateForScene()
    {
        PowerManager pm = FindFirstObjectByType<PowerManager>();
        if (pm == null || FindFirstObjectByType<PowerHUD>() != null)
            return;

        GameObject root = new GameObject("PowerHUD");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root.transform, false);
        RectTransform panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-24f, -20f);
        panelRect.sizeDelta = new Vector2(170f, 32f);
        Image background = panel.GetComponent<Image>();
        background.color = new Color(0.02f, 0.05f, 0.09f, 0.75f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("Label", typeof(RectTransform));
        textObject.transform.SetParent(panel.transform, false);
        RectTransform textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        text.fontSize = 18f;

        // Match the objective line's look when a scene has one.
        ObjectiveManager objective = FindFirstObjectByType<ObjectiveManager>();
        if (objective != null && objective.objectiveText != null)
        {
            text.font = objective.objectiveText.font;
            text.fontSize = objective.objectiveText.fontSize;
        }

        root.AddComponent<PowerHUD>().Init(pm, text);
    }

    private void Init(PowerManager pm, TMP_Text text)
    {
        powerManager = pm;
        label = text;
        powerManager.Changed += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (powerManager != null)
            powerManager.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (powerManager == null || label == null)
            return;

        // A level without a real power budget has nothing meaningful to show.
        label.transform.parent.gameObject.SetActive(powerManager.maxPower > 0);

        int available = powerManager.AvailablePower;
        label.text = "POWER  " + available + " / " + powerManager.maxPower;
        label.color = available <= 0 ? EmptyColor : NormalColor;
    }
}
