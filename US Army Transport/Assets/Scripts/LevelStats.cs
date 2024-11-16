using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelStats : MonoBehaviour
{

    public GameObject[] Players;
    public Transform[] SpawnPoints;
    public GameObject[] StillObjects;
    public Transform[] StillObjectsPos;

    public GameObject[] ToOff;
    public GameObject[] CarraigeCars;
    public Animator[] EnableAnim;


    public GameObject Props;
    public GameObject CPs;


    public GameObject TruckCam;
    public GameObject Temppark;
    public GameObject UpperPlank;
    public BoxCollider collOcean;

    public Animator TruckAnim;


    public GameObject CelebCam;


    public bool ShipLoad;
    


    private void Start()
    {

        GameManager.Instance.OnLevelStatsLoadedHandler(this);

    }







}
