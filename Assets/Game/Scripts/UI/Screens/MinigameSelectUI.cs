using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    // Presentation-mode menu: pick one of the 3 minigames directly, no city walking.
    // Two ways to use it:
    //  - Scene-placed (preferred): add this to your own menu root in DayCity, assign the 3
    //    buttons below. It registers itself to UIManager on enable; show/hide is driven by
    //    GameManager, so leave the GameObject active and don't wire OnClick manually.
    //  - Runtime fallback: if no scene instance exists, CreateRuntime builds a default menu.
    public class MinigameSelectUI : UIScreen
    {
        [Header("Scene wiring (leave empty when built at runtime)")]
        [SerializeField] private Button _anmeldungButton;
        [SerializeField] private Button _apartmentHuntingButton;
        [SerializeField] private Button _visaButton;

        public event Action<MinigameId> OnMinigameChosen;

        protected override void Awake()
        {
            base.Awake();
            WireButton(_anmeldungButton, MinigameId.Anmeldung);
            WireButton(_apartmentHuntingButton, MinigameId.ApartmentHunting);
            WireButton(_visaButton, MinigameId.Visa);
        }

        private void OnEnable()
        {
            UIManager.Instance?.RegisterMinigameSelect(this);
        }

        private void WireButton(Button button, MinigameId id)
        {
            if (button != null)
                button.onClick.AddListener(() => OnMinigameChosen?.Invoke(id));
        }

        private static readonly (MinigameId id, string label)[] Entries =
        {
            (MinigameId.Anmeldung, "ANMELDUNG"),
            (MinigameId.ApartmentHunting, "APARTMENT HUNTING"),
            (MinigameId.Visa, "VISA"),
        };

        private bool _built;

        public static MinigameSelectUI CreateRuntime(Transform canvasParent, Transform insertBefore)
        {
            var go = new GameObject("MinigameSelectUI(Runtime)",
                typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(canvasParent, false);
            // Keep it below the Loading screen so transitions still cover it.
            if (insertBefore != null && insertBefore.parent == canvasParent)
                go.transform.SetSiblingIndex(insertBefore.GetSiblingIndex());
            Stretch((RectTransform)go.transform);

            var ui = go.AddComponent<MinigameSelectUI>();
            ui.Build();
            return ui;
        }

        private void Build()
        {
            if (_built) return;
            _built = true;

            var dim = NewRect("Dim", transform);
            Stretch(dim);
            dim.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

            var panel = NewRect("Panel", transform);
            panel.sizeDelta = new Vector2(660f, 130f + Entries.Length * 108f);
            panel.gameObject.AddComponent<Image>().color = new Color(0.09f, 0.10f, 0.14f, 0.95f);

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 28, 32);
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var title = NewRect("Title", panel).gameObject.AddComponent<TextMeshProUGUI>();
            title.text = "CHOOSE YOUR MISSION";
            title.fontSize = 42f;
            title.fontStyle = FontStyles.Bold;
            title.alignment = TextAlignmentOptions.Center;
            title.gameObject.AddComponent<LayoutElement>().preferredHeight = 70f;

            foreach (var (id, label) in Entries)
                BuildButton(panel, id, label);
        }

        private void BuildButton(RectTransform parent, MinigameId id, string label)
        {
            var rt = NewRect($"Button_{id}", parent);
            rt.gameObject.AddComponent<LayoutElement>().preferredHeight = 90f;

            var image = rt.gameObject.AddComponent<Image>();
            image.color = new Color(0.17f, 0.35f, 0.60f, 1f);

            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f, 1f);
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() => OnMinigameChosen?.Invoke(id));

            var labelRect = NewRect("Label", rt);
            Stretch(labelRect);
            var text = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 32f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
