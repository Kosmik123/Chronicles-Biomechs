using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    public BattleData data;
    public PuzzleController currentController;

    public enum GamePhase
    {
        PlayerMove,
        FindingMatches,

    }


    [Header("To link")]
    public Transform enemyField;
    public SwapController swapController;
    public Transform heroField;

    public Animation backgroundAnimation;
    public SpriteRenderer backgroundRenderer;

    [Header("States")]
    public Scene currentLevel;

    void Awake()
    {
        data = GetComponent<BattleData>();
    }

    void Start()
    {
        data.CreateLists();
        currentController = swapController;
        data.levelSettings = GameController.main.levelToLoad;
        SetBackground(data.levelSettings.background);
        GenerateWave(0);

        CreateHeroCards();
    }

    // Update is called once per frame
    void Update()
    {
        currentController.DoUpdate();

        /*
         WaitForPlayerMove()
         CheckMatches()
            ChangeTokensToTroops()
            MoveTroops()
            DestroyDeadEnemies()


        */


        if (data.isLevelLoaded)
        {
            if (data.AreAllEnemiesDead())
            {
                Destroy(data.currentContainer.gameObject);
                GenerateWave(data.waveIndex + 1);
            }
        }

    }

    public void SetBackground(Sprite bg)
    {
        backgroundRenderer.sprite = bg;
    }

    public void CreateHeroCards()
    {
        for (int i = 0; i < data.heroTeam.size; i++)
        {
            Vector3 pos = data.GetHeroCardPositionByIndex(i);
            GameObject card = Instantiate(Settings.main.heroCards.prefab, pos, Quaternion.identity, heroField);
            HeroCardController heroController = card.GetComponent<HeroCardController>();
            heroController.Initialize(i, data.heroTeam.heroes[i]);
            card.name = heroController.hero.name;
            data.heroesByElement[heroController.hero.elementId].heroes.Add(heroController);
        }
    }


    bool CanPlayerMove()
    { 
        return !swapController.grid.IsAnyTokenMoving() &&
            !swapController.grid.IsAnyTokenAnimating();
    }

    void GenerateWave(int waveIndex)
    {
        data.waveIndex = waveIndex;
    
        if(waveIndex < data.levelSettings.numberOfBasicWaves)
        {
            int configCount = data.levelSettings.possibleConfigurations.Length;
            GameObject randomEnemyConfig = data.levelSettings.possibleConfigurations[
                Random.Range(0, configCount)].gameObject;

            data.currentContainer = Instantiate(randomEnemyConfig, enemyField).GetComponent<EnemyContainer>();
            Debug.Log("Tu ustawiam jacy są wrogowie");

            int enemyCount = data.levelSettings.enemies.Length;
            foreach(EnemyController enemy in data.currentContainer.enemies)
            {
                int randomBattlerIndex = Random.Range(0, enemyCount);
                enemy.SetBattler(data.levelSettings.enemies[randomBattlerIndex]);
            }
        }
        else //BOSS
        {
            data.currentContainer = Instantiate(
                data.levelSettings.boss.configuration,
                enemyField).GetComponent<EnemyContainer>();

            foreach (int i in data.levelSettings.boss.helperIndexes)
                data.currentContainer.enemies[i].SetBattler(data.levelSettings.boss.helper);
            foreach (int i in data.levelSettings.boss.bossIndexes)
                data.currentContainer.enemies[i].SetBattler(data.levelSettings.boss.bossEnemy);
        }


    }












#if UNITY_EDITOR
    [CustomEditor(typeof(BattleController))]
    public class BattleControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create hero cards"))
            {
                Settings.main = FindObjectOfType<PuzzleSettings>();
                BattleController controller = target as BattleController;
                controller.data = controller.GetComponent<BattleData>();
                controller.CreateHeroCards();
            }
        }
    }
#endif

}
