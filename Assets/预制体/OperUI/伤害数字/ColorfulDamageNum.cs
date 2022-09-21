using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

// using Random = System.Random;

public class ColorfulDamageNum : MonoBehaviour
{
    public float disableTime = 1.5f;
    private float t;
    public float speed = 0.5f;
    private int id;
    private List<Vector3> directionList = new List<Vector3>();


    private void Awake()
    {
        directionList.Add(new Vector3(0, 1, 0) * speed);
        directionList.Add(new Vector3(0.1f, 0.995f, 0) * speed);
        directionList.Add(new Vector3(-0.1f, 0.995f, 0) * speed);
        directionList.Add(new Vector3(0.2f, 0.98f, 0) * speed);
        directionList.Add(new Vector3(-0.2f, 0.98f, 0) * speed);
        directionList.Add(new Vector3(0.3f, 0.954f, 0) * speed);
        directionList.Add(new Vector3(-0.3f, 0.954f, 0) * speed);
        directionList.Add(new Vector3(0.4f, 0.9165f, 0) * speed);
        directionList.Add(new Vector3(-0.4f, 0.9165f, 0) * speed);
        directionList.Add(new Vector3(0.5f, 0.866f, 0) * speed);
        directionList.Add(new Vector3(-0.5f, 0.866f, 0) * speed);
        directionList.Add(new Vector3(0.6f, 0.8f, 0) * speed);
        directionList.Add(new Vector3(-0.6f, 0.8f, 0) * speed);
        directionList.Add(new Vector3(0.5f, 0.866f, 0) * speed);
        directionList.Add(new Vector3(-0.5f, 0.866f, 0) * speed);
        directionList.Add(new Vector3(0.6f, 0.8f, 0) * speed);
        directionList.Add(new Vector3(-0.6f, 0.8f, 0) * speed);
        t = disableTime;
    }

    void Update()
    {
        transform.Translate(directionList[id] * (Time.deltaTime * disableTime));

        t -= Time.deltaTime;
        if (t <= 0)
        {
            t = disableTime;
            id = Random.Range(0, directionList.Count);
            PoolManager.RecycleObj(gameObject);
        }
    }
}
