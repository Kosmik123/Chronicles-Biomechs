using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReferenceController : MonoBehaviour
{
    public enum LevelReferenceState
    {
        LOCKED, UNLOCKED, COMPLETED
    }

    [Header("ToLink")]
    public SpriteRenderer starRenderer;
    public SpriteRenderer darkCenterRenderer;
    public SpriteRenderer lightCenterRenderer;

    [Header("Properties")]
    public int stageNumber;
    public int levelNumber;
    private Gradient glowRange;
    private float glowSpeed;


    [Header("States")]
    public LevelReferenceState currentState;
    public LevelReferenceState lastState;

    public void Start()
    {
        glowRange = Settings.main.stagesSettings.darkCensterGlowRange;
        glowSpeed = Settings.main.stagesSettings.darkCensterGlowSpeed;

        UpdateState();
        UpdateGraphics();
    }

    public void Update()
    {
        UpdateState();
        if (lastState != currentState)
        {
            UpdateGraphics();
            lastState = currentState;
        }

        switch(currentState)
        {
            case LevelReferenceState.LOCKED:

                break;
            case LevelReferenceState.UNLOCKED:
                lightCenterRenderer.color = glowRange.Evaluate(0.5f * Mathf.Sin(glowSpeed * Time.time) + 0.5f);
                break;
            case LevelReferenceState.COMPLETED:
                
                break;
        }
    }

    public void UpdateState()
    {
        int levelCompleted = Player.main.levels.levelsCompletedByStage[stageNumber];
        if (levelCompleted < levelNumber)
            currentState = LevelReferenceState.LOCKED;
        else if (levelCompleted > levelNumber)
            currentState = LevelReferenceState.COMPLETED;
        else
            currentState = LevelReferenceState.UNLOCKED;
    }

    public void UpdateGraphics()
    {
        switch (currentState)
        {
            case LevelReferenceState.LOCKED:
                starRenderer.enabled = false;
                darkCenterRenderer.enabled = false;
                lightCenterRenderer.enabled = false;
                break;
            case LevelReferenceState.UNLOCKED:
                starRenderer.enabled = true;
                starRenderer.transform.localScale = new Vector3(1, 1, 1);
                darkCenterRenderer.enabled = false;
                lightCenterRenderer.enabled = true;
                lightCenterRenderer.transform.localScale = new Vector3(1, 1, 1);
                break;
            case LevelReferenceState.COMPLETED:
                starRenderer.enabled = true;
                starRenderer.transform.localScale = new Vector3(
                    Settings.main.stagesSettings.completedLevelScale,
                    Settings.main.stagesSettings.completedLevelScale);
                darkCenterRenderer.enabled = true;
                darkCenterRenderer.transform.localScale = new Vector3(
                    Settings.main.stagesSettings.completedLevelScale,
                    Settings.main.stagesSettings.completedLevelScale);
                lightCenterRenderer.enabled = false;
                break;
        }
    }

    public void OnMouseUpAsButton()
    {
        if(currentState != LevelReferenceState.LOCKED)
        {
            GameController.LoadLevel(Settings.main.levels.stages[stageNumber].levels[levelNumber]);
        }
    }
}
