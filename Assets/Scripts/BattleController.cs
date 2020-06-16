using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public BattleData data;
    public PuzzleController currentController;

    [Header("To link")]
    public SwapController swapController;
    public Transform heroField;

    void Awake()
    {
        data = GetComponent<BattleData>();
    }

    void Start()
    {
        data.CreateLists();
        currentController = swapController;
        CreateHeroCards();
    }

    // Update is called once per frame
    void Update()
    {




        /*
         WaitForPlayerMove()
         CheckMatches()
            ChangeTokensToTroops()
            MoveTroops()
            DestroyDeadEnemies()






        */
    }


    public void CreateHeroCards()
    {
        for(int i = 0; i < data.battlingTeam.size; i++)
        {
            Vector3 pos = data.GetHeroCardPositionByIndex(i);
            GameObject card = Instantiate(Settings.main.heroCards.prefab, pos, Quaternion.identity, heroField);
            HeroCardController heroController = card.GetComponent<HeroCardController>();
            heroController.Initialize(i, data.battlingTeam.heroes[i]);
            card.name = heroController.hero.name;
            data.heroesByElement[heroController.hero.elementId].heroes.Add(heroController);
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
