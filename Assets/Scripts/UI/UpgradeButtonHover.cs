using System;
using TowerDefense.TowerSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class UpgradeButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public TowerRuntime TargetTower { get; private set; }

        public event Action<TowerRuntime> Highlighted;
        public event Action Unhighlighted;

        public void SetTargetTower(TowerRuntime towerRuntime)
        {
            TargetTower = towerRuntime;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Highlighted?.Invoke(TargetTower);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Unhighlighted?.Invoke();
        }
    }
}