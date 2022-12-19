using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerController player;

    List<HealthHeart> hearts = new List<HealthHeart>();


    private void OnEnable()
    {
        player.onPlayerDamage += DrawHearts;
    }

    private void OnDisable()
    {
        player.onPlayerDamage -= DrawHearts;

    }

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        //how many total hearts to make based on max health
        int maxHealthRemainder = player.maxHealth % 2;

        int heartsToMake = (int)((player.maxHealth/2) + maxHealthRemainder);

        for (int i =0; i<heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for(int i=0; i<hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(player.playerHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder); 
        }

    }


    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }


    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }
}
