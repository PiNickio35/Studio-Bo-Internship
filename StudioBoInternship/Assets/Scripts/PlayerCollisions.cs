using System;
using Base_Classes;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCollisions : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Spawner")
        {
            CollisionHandler collisionHandler = other.GetComponent<CollisionHandler>();
            GameManager.Instance.nextSpawnPoint = collisionHandler.spawnPointName;
            GameManager.Instance.sceneToLoad = collisionHandler.sceneToLoad;
            GameManager.Instance.LoadNextScene();
        }

        if (other.tag == "EncounterRegion")
        {
            BaseRegion region = other.GetComponent<RegionHolder>().currentRegion;
            GameManager.Instance.currentRegion = region;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "EncounterRegion")
        {
            GameManager.Instance.canGetEncounter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "EncounterRegion")
        {
            GameManager.Instance.canGetEncounter = false;
        }
    }
}
