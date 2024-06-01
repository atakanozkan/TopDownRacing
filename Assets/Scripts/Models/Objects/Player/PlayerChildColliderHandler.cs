using System.Collections;
using System.Collections.Generic;
using TDR.Managers;
using TDR.Models.Player;
using UnityEngine;

public class PlayerChildColliderHandler : MonoBehaviour
{
    private Player playerParent;

    void Start()
    {
        playerParent = GetComponentInParent<Player>();
        if (playerParent == null)
        {
            Debug.LogError("Player script not found on parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPCCarCollider"))
        {
            GameManager.Instance.TriggerGameOver();
            Debug.Log("Child collision with NPC Car Detected!");
        }
    }
}
