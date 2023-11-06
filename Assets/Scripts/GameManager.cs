using UnityEngine;
using Normal.Realtime;

public class GameManager : RealtimeComponent<GameModel>
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnRealtimeModelReplaced(GameModel previousModel, GameModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events on the previous model
            previousModel.coinsCollectedDidChange -= CoinsCollectedDidChange;
            previousModel.gameTimeDidChange -= GameTimeDidChange;
        }

        if (currentModel != null)
        {
            // Register for events so we'll know if the coin count or game time changes later
            currentModel.coinsCollectedDidChange += CoinsCollectedDidChange;
            currentModel.gameTimeDidChange += GameTimeDidChange;
        }
    }

    public string GetFormattedGameTime()
    {
        // Format the game time into a string
        if (model != null)
        {
            float gameTime = model.gameTime;
            int minutes = (int)(gameTime / 60);
            int seconds = (int)(gameTime % 60);
            return $"{minutes:00}:{seconds:00}";
        }
        return "00:00";
    }

    public int GetCoinsCollected()
    {
        return model?.coinsCollected ?? 0;
    }

    private void CoinsCollectedDidChange(GameModel model, int value)
    {
        // Update coins collected UI or logic
        Debug.Log("Coins collected: " + value);
    }

    private void GameTimeDidChange(GameModel model, float value)
    {
        // Update game time UI or logic
        Debug.Log("Game time: " + value);
    }

    public void AddCoins(int amount)
    {
        if (realtime.connected && model.isOwnedLocallyInHierarchy)
        {
            model.coinsCollected += amount;
        }
    }

    public void UpdateGameTime(float deltaTime)
    {
        if (realtime.connected && model.isOwnedLocallyInHierarchy)
        {
            model.gameTime += deltaTime;
        }
    }

    // Other methods specific to managing the game's coins and time...
}
