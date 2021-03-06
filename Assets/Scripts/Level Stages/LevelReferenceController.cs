﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReferenceController : MonoBehaviour
{
    public enum State
    {
        LOCKED, UNLOCKED, COMPLETED
    }

    [Header("To Link")]
    public SpriteRenderer star;
    public SpriteRenderer circleRenderer;
    public SpriteRenderer darkCenterRenderer;
    public SpriteRenderer lightCenterRenderer;
    private CircleCollider2D collider;

    [Header("Properties")]
    public int stageNumber;
    public int levelNumber;
    private Gradient glowRange;
    private float glowSpeed;
    public float colliderRadius;

    private PuzzleSettings.StagesSettings settings;

    [Header("States")]
    public bool clicked;
    public State currentState, lastState;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    public void Start()
    {
        settings = Settings.main.stagesSettings;

        glowRange = settings.starCenterGlowRange;
        glowSpeed = settings.starCenterGlowSpeed;

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
            case State.LOCKED:

                break;
            case State.UNLOCKED:
                lightCenterRenderer.color = glowRange.Evaluate(0.5f * Mathf.Sin(glowSpeed * Time.time) + 0.5f);
                break;
            case State.COMPLETED:
                
                break;
        }
    }

    public void UpdateState()
    {
        int levelCompleted = Player.main.levels.levelsCompletedByStage[stageNumber];
        if (levelCompleted < levelNumber)
            currentState = State.LOCKED;
        else if (levelCompleted > levelNumber)
            currentState = State.COMPLETED;
        else
            currentState = State.UNLOCKED;
    }

    public void UpdateGraphics()
    {
        switch (currentState)
        {
            case State.LOCKED:
                star.transform.localScale =
                    Vector3.one * settings.lockedLevelScale;

                circleRenderer.enabled = false;
                darkCenterRenderer.enabled = false;
                lightCenterRenderer.enabled = false;
                collider.radius = 0;
                break;

            case State.UNLOCKED:
                star.transform.localScale =
                    Vector3.one * settings.unlockedLevelScale;

                circleRenderer.enabled = true;
                darkCenterRenderer.enabled = false;
                lightCenterRenderer.enabled = true;
                collider.radius = settings.unlockedLvColiderRadius;
                break;

            case State.COMPLETED:
                star.transform.localScale = 
                    Vector3.one * settings.completedLevelScale;

                circleRenderer.enabled = true;
                darkCenterRenderer.enabled = true;
                lightCenterRenderer.enabled = false;
                collider.radius = settings.completedLvColiderRadius;
                break;
        }
    }

    public void OnMouseUpAsButton()
    {
        if(currentState != State.LOCKED)
        {
            Debug.Log("Loadiing level " + stageNumber + "-" + levelNumber);
            clicked = true;
            GameController.LoadLevel(Settings.main.levels.stages[stageNumber].levels[levelNumber]);
        }
    }
}
