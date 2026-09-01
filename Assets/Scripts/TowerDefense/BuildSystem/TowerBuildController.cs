using System.Collections.Generic;
using TowerDefense.TowerSystem;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TowerDefense.BuildSystem
{
    public class TowerBuildController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LayerMask buildTileMask = ~0;
        [SerializeField] private float rayDistance = 500f;
        [SerializeField] private TowerSelectionPanel towerSelectionPanel;
        [SerializeField] private TowerUpgradePanel towerUpgradePanel;
        [SerializeField] private List<TowerDefinition> towerDefinitions = new List<TowerDefinition>();

        private IBuildTile hoveredTile;
        private IBuildTile selectedTile;

        private void Awake()
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
            else if (towerDefinitions.Count > 0)
            {
                towerSelectionPanel.SetTowerDefinitions(towerDefinitions);
            }

            if (towerSelectionPanel != null)
            {
                towerSelectionPanel.Hide();
                towerSelectionPanel.TowerChosen += HandleTowerChosen;
                towerSelectionPanel.Cancelled += ClearSelection;
            }

            if (towerUpgradePanel != null)
            {
                towerUpgradePanel.Hide();
                towerUpgradePanel.Closed += ClearSelection;
                towerUpgradePanel.Sold += HandleTowerSold;
            }
        }

        private void OnDestroy()
        {
            if (towerSelectionPanel != null)
            {
                towerSelectionPanel.TowerChosen -= HandleTowerChosen;
                towerSelectionPanel.Cancelled -= ClearSelection;
            }

            if (towerUpgradePanel != null)
            {
                towerUpgradePanel.Closed -= ClearSelection;
                towerUpgradePanel.Sold -= HandleTowerSold;
            }
        }

        private void Update()
        {
            if (worldCamera == null)
            {
                return;
            }

            IBuildTile tileUnderPointer = IsPointerOverUi() ? null : GetTileUnderPointer();
            UpdateHover(tileUnderPointer);

            if (WasCancelPressed())
            {
                ClearSelection();
                return;
            }

            if (WasPrimaryPressed() && !IsPointerOverUi())
            {
                SelectTile(tileUnderPointer);
            }
        }


        private void HandleTowerSold(TowerRuntime towerRuntime)
        {
            if (selectedTile != null)
            {
                selectedTile.ClearTower();
            }

            if (towerRuntime != null)
            {
                Destroy(towerRuntime.gameObject);
            }

            ClearSelection();
        }

        private void SelectTile(IBuildTile tile)
        {
            if (tile == null)
            {
                ClearSelection();
                return;
            }

            if (tile.PlacedTower != null)
            {
                selectedTile = tile;

                if (towerSelectionPanel != null)
                {
                    towerSelectionPanel.Hide();
                }

                if (towerUpgradePanel != null)
                {
                    towerUpgradePanel.Show(tile.PlacedTower);
                }

                return;
            }

            if (!tile.CanBuild)
            {
                ClearSelection();
                return;
            }

            selectedTile = tile;

            if (towerUpgradePanel != null)
            {
                towerUpgradePanel.Hide();
            }

            if (towerSelectionPanel != null)
            {
                towerSelectionPanel.ShowForTile(tile);
            }
        }

        private void HandleTowerChosen(TowerDefinition towerDefinition)
        {
            if (selectedTile == null || towerDefinition == null)
            {
                return;
            }

            if (TowerDefense.PlayerSystem.PlayerStats.Instance == null
                || !TowerDefense.PlayerSystem.PlayerStats.Instance.TrySpendMoney(towerDefinition.Cost))
            {
                return;
            }

            if (selectedTile.TryPlaceTower(towerDefinition))
            {
                ClearSelection();
            }
            else
            {
                TowerDefense.PlayerSystem.PlayerStats.Instance.AddMoney(towerDefinition.Cost);
            }
        }

        private void ClearSelection()
        {
            selectedTile = null;

            if (towerSelectionPanel != null)
            {
                towerSelectionPanel.Hide();
            }

            if (towerUpgradePanel != null)
            {
                towerUpgradePanel.Hide();
            }
        }

        private void UpdateHover(IBuildTile tile)
        {
            if (hoveredTile == tile)
            {
                return;
            }

            if (hoveredTile != null)
            {
                hoveredTile.SetHovered(false);
            }

            hoveredTile = tile;

            if (hoveredTile != null)
            {
                hoveredTile.SetHovered(true);
            }
        }

        private IBuildTile GetTileUnderPointer()
        {
            Ray ray = worldCamera.ScreenPointToRay(GetPointerPosition());

            if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, buildTileMask, QueryTriggerInteraction.Ignore))
            {
                return null;
            }

            MonoBehaviour[] behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IBuildTile buildTile)
                {
                    return buildTile;
                }
            }

            return null;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private static Vector2 GetPointerPosition()
        {
            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        }

        private static bool WasPrimaryPressed()
        {
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }

        private static bool WasCancelPressed()
        {
            return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        }
    }
}