using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnSlime : MonoBehaviour
{

    public PlayerController player;

    private Vector2 startPoint = new Vector2(-98, 20);
    private Vector2 endPoint = new Vector2(-128, 20);

    public GameObject slimePrefab;
    private GameObject clone;

    private int slimeHealth;
    private int maxSlimeHealth = 3;


    private void OnEnable()
    {
        player.onPlayerAttack += TakeDamage;
    }

    private void OnDisable()
    {
        player.onPlayerAttack -= TakeDamage;

    }

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        slimeHealth = maxSlimeHealth;

        Vector3 spawnPosition = EnemyRespawnPoint();

         clone = (GameObject)Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
    }

    void TakeDamage()
    {
        slimeHealth--;
        Debug.Log(slimeHealth);
    }

    void removeCurrentSlime()
    {
        Destroy(clone);
    }

    private Vector3 EnemyRespawnPoint()
    {
        var random = new System.Random();
        var lowerBoundX = -128;
        var upperBoundX = -98;
        var newX = random.Next(lowerBoundX, upperBoundX);

        var lowerBoundY = 20;
        var upperBoundY = 28;
        var newY = random.Next(lowerBoundY, upperBoundY);

        return new Vector3(newX, newY, 0);

    }
    private void Update()
    {
        if (slimeHealth == 0)
        {
            removeCurrentSlime();
            SpawnEnemy();
        }
    }
}
