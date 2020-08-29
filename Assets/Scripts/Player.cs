using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    public static Player main;

    public int experience;
    public int level;

    [System.Serializable]
    public class TeamsAndHeroes
    {
        public List<Team> teams = new List<Team>();

        public List<Hero> ownedHeroes = new List<Hero>();
        public List<Troop> ownedTroops = new List<Troop>();
        public List<Mask> ownedMasks = new List<Mask>();
    }
    public TeamsAndHeroes teamsAndHeroes;

    [System.Serializable]
    public class Levels
    {
        public int stagesUnlocked = 1;
        public int[] levelsCompletedByStage;

    }
    public Levels levels;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

