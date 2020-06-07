using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController main;

    public Team battlingTeam;

    public float[] cardXPositions;

    public class HeroControllersArray
    {
        public HeroController[] heroes;
    }

    public HeroControllersArray[] heroesByElement;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Vector3 GetHeroCardPositions(int elementId)
    {
        //bool isEven = heroesByElement.Length % 2 == 0;

        return new Vector3(cardXPositions[elementId], -12.75f);
    }


}
