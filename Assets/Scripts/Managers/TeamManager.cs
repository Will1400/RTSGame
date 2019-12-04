//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class TeamManager : MonoBehaviour
//{
//    public static TeamManager Instance;

//    public List<Team> Teams;

//    public Transform TeamsHolder;

//    private void Awake()
//    {
//        if (Instance is null)
//            Instance = this;
//        else if (Instance != this)
//            Destroy(gameObject);

//        if (GameObject.Find("Teams"))
//            TeamsHolder = GameObject.Find("Teams").transform;
//        else
//            TeamsHolder = new GameObject("Teams").transform;
//    }
//}