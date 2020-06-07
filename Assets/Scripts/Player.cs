using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    public class HeroSet
    {
        public HeroController hero;
        public Troop troop;
    }

    public class Team
    {
        public HeroSet[] members;
    }

    public static Player main;

    public int experience;
    public int level;

    public List<Team> teams;

    public List<Hero> ownedHeroes = new List<Hero>();
    public List<Troop> ownedTroops = new List<Troop>();

    public Troop[] troopsByElement;




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
