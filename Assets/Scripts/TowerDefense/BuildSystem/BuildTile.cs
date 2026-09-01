using System;
using TowerDefense.TowerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.BuildSystem
{
    [DisallowMultipleComponent]
    public class BuildTile : MonoBehaviour, IBuildTile
    {
        [Header("Build")]
        [SerializeField] private bool canBuild = true;

        [Header("Visuals")]
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverCanBuildColor = new Color(0.25f, 0.9f, 0.45f);
        [SerializeField] private Color hoverBlockedColor = new Color(0.95f, 0.25f, 0.2f);
        [SerializeField] private Color hoverOccupiedColor = new Color(0.95f, 0.75f, 0.2f);
        [SerializeField] private Color selectedColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private TowerRangeIndicator rangeIndicator;

        [Header("Tile Bonus")]
        [SerializeField] private TileBonusType bonusType = TileBonusType.None;
        [SerializeField, Min(0.05f)] private float bonusMultiplier = 1.2f;

        [Header("Tile Bonus Icon")]
        [SerializeField] private Canvas bonusCanvas;
        [SerializeField] private Image bonusIconImage;
        [SerializeField] private BonusIconMapping[] bonusIcons;

        [Serializable]
        private struct BonusIconMapping
        {
            public TileBonusType Type;
            public Sprite Icon;
        }

        private GameObject placedTower;
        private TowerRuntime placedTowerRuntime;
        private MaterialPropertyBlock propertyBlock;
        private bool isHovered;
        private bool isSelected;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        public bool CanBuild => canBuild && placedTower == null;
        public Vector3 BuildPosition => transform.position + Vector3.up;
        public TowerRuntime PlacedTower => placedTowerRuntime;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            ApplyColor(normalColor);
            RefreshBonusIcon();
        }

        private void OnValidate()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
        }

        public void SetHovered(bool hovered)
        {
            isHovered = hovered;

            if (!hovered && rangeIndicator != null && placedTowerRuntime != null)
            {
                rangeIndicator.Hide();
            }

            RefreshColor();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            RefreshColor();
        }

        private void RefreshColor()
        {
            if (isHovered)
            {
                if (placedTowerRuntime != null)
                {
                    ApplyColor(hoverOccupiedColor);

                    if (rangeIndicator != null)
                    {
                        rangeIndicator.Show(placedTowerRuntime.transform.position, placedTowerRuntime.AttackRange);
                    }

                    return;
                }

                ApplyColor(CanBuild ? hoverCanBuildColor : hoverBlockedColor);
                return;
            }

            ApplyColor(isSelected ? selectedColor : normalColor);
        }

        public bool TryPlaceTower(TowerDefinition towerDefinition)
        {
            if (!CanBuild || towerDefinition == null || towerDefinition.Prefab == null)
            {
                return false;
            }

            placedTower = Instantiate(
                towerDefinition.Prefab,
                BuildPosition + towerDefinition.BuildOffset,
                Quaternion.identity);

            TowerRuntime towerRuntime = placedTower.GetComponent<TowerRuntime>();
            if (towerRuntime == null)
            {
                towerRuntime = placedTower.AddComponent<TowerRuntime>();
            }

            towerRuntime.Initialize(towerDefinition);
            GameSystem.CodexProgressStore.UnlockTower(towerDefinition);
            if (bonusType != TileBonusType.None)
            {
                towerRuntime.ApplyTileBonus(bonusType, bonusMultiplier);
            }

            placedTowerRuntime = towerRuntime;

            return placedTower != null;
        }

        public void ClearTower()
        {
            placedTower = null;
            placedTowerRuntime = null;

            if (rangeIndicator != null)
            {
                rangeIndicator.Hide();
            }
        }

        public void ShowRangePreview(float radius)
        {
            if (rangeIndicator != null)
            {
                rangeIndicator.Show(BuildPosition, radius);
            }
        }

        public void HideRangePreview()
        {
            if (rangeIndicator != null)
            {
                rangeIndicator.Hide();
            }
        }

        private void RefreshBonusIcon()
        {
            if (bonusCanvas != null)
            {
                bonusCanvas.gameObject.SetActive(bonusType != TileBonusType.None);
            }

            if (bonusIconImage == null || bonusIcons == null)
            {
                return;
            }

            foreach (BonusIconMapping mapping in bonusIcons)
            {
                if (mapping.Type == bonusType)
                {
                    bonusIconImage.sprite = mapping.Icon;
                    return;
                }
            }
        }

        private void ApplyColor(Color color)
        {
            if (renderers == null)
            {
                return;
            }

            foreach (Renderer tileRenderer in renderers)
            {
                if (tileRenderer == null)
                {
                    continue;
                }

                tileRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(BaseColorId, color);
                propertyBlock.SetColor(ColorId, color);
                tileRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}