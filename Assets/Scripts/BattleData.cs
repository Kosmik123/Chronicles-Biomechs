using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public static BattleData main;
    public Team battlingTeam;

    [System.Serializable]
    public class HeroControllersList
    {
        public List<HeroController> heroes = new List<HeroController>();
    }
    public HeroControllersList[] heroesByElement;

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
        for(int i = 0; i < battlingTeam.size; i++)
        {
            if (battlingTeam.heroes[i].elementId == elemId)
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
        float firstCardX = center - dist * (battlingTeam.size - 1) / 2;

        float cardX = firstCardX + index * dist;

        return new Vector3(cardX, Settings.main.heroCards.verticalPosition);
    }



}

