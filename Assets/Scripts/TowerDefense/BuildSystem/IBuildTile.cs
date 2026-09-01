using TowerDefense.TowerSystem;
using UnityEngine;

namespace TowerDefense.BuildSystem
{
    public interface IBuildTile
    {
        bool CanBuild { get; }
        Vector3 BuildPosition { get; }
        TowerRuntime PlacedTower { get; }

        void SetHovered(bool hovered);
        void SetSelected(bool selected);
        bool TryPlaceTower(TowerDefinition towerDefinition);
        void ClearTower();
        void ShowRangePreview(float radius);
        void HideRangePreview();
    }
}