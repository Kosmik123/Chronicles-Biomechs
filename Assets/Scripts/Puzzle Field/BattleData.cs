using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public static BattleData main;

    [Header("Battle Properties")]
    public Team heroTeam;
    public int seasonIndex;
    public int worldIndex;
    public int levelIndex;

    public LevelSettings levelSettings;

    [System.Serializable]
    public class HeroControllersList
    {
        public List<HeroCardController> heroes = new List<HeroCardController>();
    }

    [Header("States")]
    public bool isLevelLoaded;
    public Enemy[] enemies;
    public HeroControllersList[] heroesByElement;
    public EnemyContainer currentContainer;
    public List<EnemyController> battlingEnemies;
    public int waveIndex;

    // Start is called before the first frame update
    void Awake()
    {
        main = this;
    }

    public void CreateLists()
    {
        heroesByElement = new HeroControllersList[Settings.main.elements.Length];
        for (int i = 0; i < heroesByElement.Length; i++)
            heroesByElement[i] = new HeroControllersList();
    }

    public List<Vector3> GetHeroCardPositions(int elemId)
    {
        List<Vector3> positions = new List<Vector3>();
        for(int i = 0; i < heroTeam.size; i++)
        {
            if (heroTeam.heroes[i].elementId == elemId)
            {
                positions.Add(GetHeroCardPositionByIndex(i));
            }
        }
        return positions;
    }
    public Vector3 GetHeroCardPositionByIndex(int index)
    {
        float center = 0;
        float dist = Settings.main.heroCards.horizontalDistance;
        float firstCardX = center - dist * (heroTeam.size - 1) / 2;

        float cardX = firstCardX + index * dist;

        return new Vector3(cardX, Settings.main.heroCards.verticalPosition);
    }

    public int GetHeroesAttack(int elemId)
    {
        int result = 0;
        foreach(HeroCardController h in heroesByElement[elemId].heroes)
        {
            result += h.hero.GetAttack();
        }
        return result;
    }

    public void AddEnemy(EnemyController newEnemy)
    {
        battlingEnemies.Add(newEnemy);
    }

    public bool AreAllEnemiesDead()
    {
        foreach (EnemyController enemy in battlingEnemies)
            if (!enemy.isDead)
                return false;

        return true;
    }


    public void SetEnemies(EnemyController[] newEnemies)
    {
        foreach (var enemy in newEnemies)
            battlingEnemies.Add(enemy);
        isLevelLoaded = true;
    }
}

