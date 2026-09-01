using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefense.PlayerSystem;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.BuildSystem
{
    public class TowerSelectionPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, 80f);
        [SerializeField] private List<TowerDefinition> towerDefinitions = new List<TowerDefinition>();
        [SerializeField] private TowerInfoPanel towerInfoPanel;

        public event Action<TowerDefinition> TowerChosen;
        public event Action Cancelled;

        private readonly List<GameObject> spawnedButtons = new List<GameObject>();
        private Canvas parentCanvas;
        private bool initialized;
        private bool currentTileCanBuild;
        private IBuildTile currentTile;

        public void SetTowerDefinitions(IEnumerable<TowerDefinition> definitions)
        {
            towerDefinitions.Clear();

            if (definitions == null)
            {
                return;
            }

            foreach (TowerDefinition definition in definitions)
            {
                if (definition != null)
                {
                    towerDefinitions.Add(definition);
                }
            }
        }

        private void Awake()
        {
            bool alreadyInitialized = initialized;
            Initialize();

            if (!alreadyInitialized)
            {
                Hide();
            }
        }

        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            if (buttonContainer == null)
            {
                buttonContainer = panelRoot;
            }

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            parentCanvas = GetComponentInParent<Canvas>();

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Cancel);
                closeButton.onClick.AddListener(Cancel);
            }

            initialized = true;
        }
        private void OnEnable()
        {
            StartCoroutine(SubscribeWhenReady());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.OnMoneyChanged -= HandleMoneyChanged;
            }
        }

        private IEnumerator SubscribeWhenReady()
        {
            while (PlayerStats.Instance == null)
            {
                yield return null;
            }

            PlayerStats.Instance.OnMoneyChanged += HandleMoneyChanged;
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Cancel);
            }
        }

        public void ShowForTile(IBuildTile tile)
        {
            if (tile == null)
            {
                Hide();
                return;
            }

            if (currentTile != null && currentTile != tile)
            {
                currentTile.HideRangePreview();
                currentTile.SetSelected(false);
            }

            currentTile = tile;
            currentTile.SetSelected(true);

            if (titleLabel != null)
            {
                titleLabel.text = "Wybierz wieze";
            }

            currentTileCanBuild = tile.CanBuild;
            RebuildButtons(currentTileCanBuild);
            PositionNear(tile.BuildPosition);

            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            panelRoot.gameObject.SetActive(false);
            towerInfoPanel.Hide();

            if (currentTile != null)
            {
                currentTile.HideRangePreview();
                currentTile.SetSelected(false);
                currentTile = null;
            }

        }

        private void Cancel()
        {
            Cancelled?.Invoke();
            Hide();
        }

        private void HandleMoneyChanged(int currentMoney)
        {
            RefreshButtonsInteractable();
        }

        private void RefreshButtonsInteractable()
        {
            int currentMoney = PlayerStats.Instance != null ? PlayerStats.Instance.Money : 0;

            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                if (i >= towerDefinitions.Count)
                {
                    break;
                }

                TowerDefinition definition = towerDefinitions[i];
                Button button = spawnedButtons[i].GetComponent<Button>();

                if (button == null || definition == null)
                {
                    continue;
                }

                bool canAfford = currentMoney >= definition.Cost;
                button.interactable = currentTileCanBuild && definition.Prefab != null && canAfford;
            }
        }

        private void RebuildButtons(bool interactable)
        {
            ClearButtons();

            int currentMoney = PlayerStats.Instance != null ? PlayerStats.Instance.Money : 0;

            foreach (TowerDefinition towerDefinition in towerDefinitions)
            {
                if (towerDefinition == null)
                {
                    continue;
                }

                bool canAfford = currentMoney >= towerDefinition.Cost;

                Button button = CreateButton();
                button.interactable = interactable && towerDefinition.Prefab != null && canAfford;
                button.onClick.AddListener(() => TowerChosen?.Invoke(towerDefinition));
                ApplyButtonContent(button, towerDefinition);
                ApplyButtonHover(button, towerDefinition);
                spawnedButtons.Add(button.gameObject);
            }
        }

        private Button CreateButton()
        {
            if (buttonPrefab != null)
            {
                return Instantiate(buttonPrefab, buttonContainer);
            }

            GameObject buttonObject = new GameObject("Tower Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.transform.SetParent(buttonContainer, false);

            return buttonObject.GetComponent<Button>();
        }

        private static void ApplyButtonContent(Button button, TowerDefinition towerDefinition)
        {
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"{towerDefinition.DisplayName} <color=#FFA500>{towerDefinition.Cost}</color>";
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = towerDefinition.ButtonColor;
            }
        }

        private void ApplyButtonHover(Button button, TowerDefinition towerDefinition)
        {
            TowerButtonHover hover = button.GetComponent<TowerButtonHover>();
            hover.SetTowerDefinition(towerDefinition);
            hover.Highlighted += HandleButtonHighlighted;
            hover.Unhighlighted += HandleButtonUnhighlighted;
        }

        private void HandleButtonHighlighted(TowerDefinition towerDefinition)
        {
            if (towerInfoPanel != null)
            {
                towerInfoPanel.Show(towerDefinition, panelRoot);
            }

            if (currentTile != null && towerDefinition != null)
            {
                currentTile.ShowRangePreview(towerDefinition.AttackRange);
            }
        }

        private void HandleButtonUnhighlighted()
        {
            if (towerInfoPanel != null)
            {
                towerInfoPanel.Hide();
            }

            if (currentTile != null)
            {
                currentTile.HideRangePreview();
            }
        }

        private void PositionNear(Vector3 worldPosition)
        {
            Vector2 desiredScreenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPosition) + screenOffset;
            Vector2 clampedScreenPosition = ScreenBoundsUtility.ClampToScreen(panelRoot, parentCanvas, desiredScreenPosition);

            ScreenBoundsUtility.SetScreenPosition(panelRoot, parentCanvas, clampedScreenPosition);
        }

        private void ClearButtons()
        {
            foreach (GameObject spawnedButton in spawnedButtons)
            {
                if (spawnedButton != null)
                {
                    Destroy(spawnedButton);
                }
            }

            spawnedButtons.Clear();
        }
    }
}