// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;

public class GameManager : RealtimeComponent<GameModel>
{
    [SerializeField] private float CountdownDuration = 300.0f; // 5 minutes
    [SerializeField] private flagCondition experimentCondition; 
    [SerializeField] private float startRemoving = 5f; 
    private StartTrigger _startTrigger;
    private bool removeFlag = false;
    public int flagNum { get; private set; } = 0;

    public event Action<int> OnCoinsCollectedChanged;
    public int CoinsCollected => model.coinsCollected;

    // private Player[] players;

    private void Awake()
    {
        _startTrigger = FindObjectOfType<StartTrigger>();
        if (_startTrigger != null)
            _startTrigger.OnGameStarted += StartCountdown;
    }

    private void OnDestroy()
    {
        if (_startTrigger != null)
            _startTrigger.OnGameStarted -= StartCountdown;
    }

    private void Update()
    {
        if (_startTrigger == null || !_startTrigger.started) {
            return;
        }

        if ((CountdownDuration - model.gameTime) >  startRemoving && removeFlag == false) 
        {
            removeFlag = true;
            flagNum++;
            model.removeFlag++;
        }else if (removeFlag && experimentCondition == flagCondition.Gradual){
            // increment removeFlag ever x seconds
        }

        if (CheckFinished()) {
           SetTrialOver(true);
        } else {
            model.gameTime -= Time.deltaTime;
            model.gameTime = Mathf.Max(model.gameTime, 0.0f);
            UpdateCountdownUI(model.gameTime);
        }
       
    }
   

    protected override void OnRealtimeModelReplaced(GameModel previousModel, GameModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.gameTimeDidChange -= GameTimeDidChange;
            previousModel.coinsCollectedDidChange -= CoinsCollectedDidChange;
            previousModel.trialOverDidChange -= TrialOverDidChange;
            previousModel.removeFlagDidChange -= RemoveFlagDidChange;
        }

        if (currentModel != null)
        {
            currentModel.gameTimeDidChange += GameTimeDidChange;
            currentModel.coinsCollectedDidChange += CoinsCollectedDidChange;
            if (currentModel.isFreshModel) {
                currentModel.trialOver = false;
            }
            currentModel.trialOverDidChange += TrialOverDidChange;
            currentModel.removeFlagDidChange += RemoveFlagDidChange;
        }
    }

    private void GameTimeDidChange(GameModel model, float value)
    {
        // Update the UI when the gameTime changes
        UpdateCountdownUI(value);
    }

    private void CoinsCollectedDidChange(GameModel model, int value)
    {
        // Notify subscribers that the coins collected count has changed
        OnCoinsCollectedChanged?.Invoke(value);
    }

    private void TrialOverDidChange(GameModel model, bool value) {
        if (value) {
            EndTrialForAllPlayers();
        }
    }

    private void RemoveFlagDidChange(GameModel model, int value){

    }

    private void EndTrialForAllPlayers() {
        Player[] players = FindObjectsOfType<Player>();

        foreach (Player player in players) {
            player.EndTrial();
        }
    }

    private void UpdateCountdownUI(float remainingTime)
    {
        // Update any UI elements or trigger events based on the remaining time
        // if (remainingTime == CountdownDuration){
        //     model.gameTime = CountdownDuration;
        // }
    }

    public string GetFormattedGameTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(model.gameTime);
        return timeSpan.ToString(@"mm\:ss");
    }

    public void StartCountdown()
    {
        if (_startTrigger.started && model.gameTime == 0)
        {
            model.gameTime = CountdownDuration;
            Player[] p = FindObjectsOfType<Player>();
            foreach(Player player in p)
            {
                RealtimeView realtimeView = player.GetComponent<RealtimeView>();

                if (realtimeView != null && realtimeView.isOwnedLocallySelf)
                {
                    Logger_new ln = player.GetComponent<Logger_new>();
                    player.GetComponent<Player>().GetStarted();
                    ln.AddLine("GameStart");
                }
            }
        }
    }

    public void IncrementCoinsCollected()
    {
        model.coinsCollected++;
        Debug.Log("Increase value of coins");
        OnCoinsCollectedChanged?.Invoke(model.coinsCollected);
    }

    public void SetTrialOver(bool over) {
        model.trialOver = over;
    }

     public bool CheckFinished() {
        return model.gameTime <= 0 && _startTrigger != null && _startTrigger.started;
    }
}
