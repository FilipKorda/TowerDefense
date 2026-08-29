namespace TowerDefense.GameSystem
{
    public static class GameSession
    {
        public static DifficultyDefinition SelectedDifficulty { get; private set; }
        public static LevelDefinition SelectedLevel { get; private set; }

        public static void SetSelection(LevelDefinition level, DifficultyDefinition difficulty)
        {
            SelectedLevel = level;
            SelectedDifficulty = difficulty;
        }

        public static void Clear()
        {
            SelectedLevel = null;
            SelectedDifficulty = null;
        }
    }
}