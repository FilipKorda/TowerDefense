using System;
using TowerDefense.BuildSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class TowerButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public TowerDefinition TowerDefinition { get; private set; }

        public event Action<TowerDefinition> Highlighted;
        public event Action Unhighlighted;

        public void SetTowerDefinition(TowerDefinition towerDefinition)
        {
            TowerDefinition = towerDefinition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Highlighted?.Invoke(TowerDefinition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Unhighlighted?.Invoke();
        }
    }
}