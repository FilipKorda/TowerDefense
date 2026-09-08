using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.GameSystem
{
    [CreateAssetMenu(fileName = "Level Definition", menuName = "Tower Defense/Level Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private string levelName = "Level 1";
        [SerializeField] private int levelIndex = 0;

#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
#endif
        [SerializeField, HideInInspector] private string scenePath;

        public string LevelName => levelName;
        public int LevelIndex => levelIndex;
        public string ScenePath => scenePath;

#if UNITY_EDITOR
        private void OnValidate()
        {
            scenePath = sceneAsset != null ? AssetDatabase.GetAssetPath(sceneAsset) : string.Empty;
        }
#endif
    }
}