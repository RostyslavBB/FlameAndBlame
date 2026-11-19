namespace FB.Game.Flow
{
    /// <summary>Reloads the game scene; the SceneContext and all scene subscriptions are disposed with it.</summary>
    public interface ISceneRestarter
    {
        void Restart();
    }
}
