using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.TowerSystem
{
    public abstract class TowerAttackBehaviour : MonoBehaviour
    {
        public virtual void Tick(TowerRuntime tower, EnemyRuntime target, float deltaTime)
        {
        }

        public abstract void Attack(TowerRuntime tower, EnemyRuntime target);
    }
}
