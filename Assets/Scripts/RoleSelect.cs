using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine.UI;
using System;

public class RoleSelect : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material collectorMaterial;
    [SerializeField] private Material tacticalMaterial;
    [SerializeField] private Material explorerMaterial;

    [Header("Instruction")]
    [SerializeField] private Texture NoneT;
    [SerializeField] private Texture ExplorerT;
    [SerializeField] private Texture CollectorT;
    [SerializeField] private Texture TacticalT;


    private Dictionary<Collider, Role> triggerRoles = new Dictionary<Collider, Role>();
    private Dictionary<GameObject, Role> playerRoles = new Dictionary<GameObject, Role>();
    private HashSet<Role> takenRoles = new HashSet<Role>();

    private void Awake()
    {
        InitializeRoleTriggers();
    }

    private void Update()
    {
        SyncPlayerModels();
    }

    public void HandlePlayerEnterTrigger(Collider triggerCollider, GameObject player)
    {
        Player playerController = player.GetComponent<Player>();
        Debug.Log(triggerCollider);

        if (triggerRoles.TryGetValue(triggerCollider, out Role roleEntered))
        {
            GameObject roleText = GameObject.Find("RoleLabel");
 
            if (playerRoles.TryGetValue(player, out Role currentRole))
            {
                int currentRoleVal = (int) currentRole;

                if (currentRole == roleEntered)
                {
                    playerRoles[player] = Role.None;
                    takenRoles.Remove(currentRole);
                    UpdateRoleVisuals(roleEntered, false);
                    playerController.SetRole(Role.None); // Call to update the player's role to None.
                    ChangePlayerMaterial(player, Role.None);
                    Debug.Log($"Player {player.name} removed their role. They are now {Role.None}.");
                }
                else
                {
                    if (!takenRoles.Contains(roleEntered))
                    {
                        if (currentRole != Role.None)
                        {
                            takenRoles.Remove(currentRole);
                            UpdateRoleVisuals(currentRole, false);
                            playerController.SetRole(Role.None); // Change previous role to None.
                            ChangePlayerMaterial(player, Role.None);
                        }

                        playerRoles[player] = roleEntered;
                        takenRoles.Add(roleEntered);
                        UpdateRoleVisuals(roleEntered, true);
                        playerController.SetRole(roleEntered); // Update the player's role.
                        ChangePlayerMaterial(player, roleEntered);
                        Debug.Log($"Player {player.name} has taken the role of {roleEntered}.");
                    }
                }
            }
            else if (!takenRoles.Contains(roleEntered))
            {
                playerRoles.Add(player, roleEntered);
                takenRoles.Add(roleEntered);
                UpdateRoleVisuals(roleEntered, true);
                playerController.SetRole(roleEntered); // Update the player's role.
                ChangePlayerMaterial(player, roleEntered);
                Debug.Log($"Player {player.name} has taken the role of {roleEntered}.");
            }

            Player pl = player.GetComponent<Player>();
            player.GetComponent<Logger_new>().AddLine("SwitchRole:"+ pl.currentRole.ToString());

            RealtimeView realtimeView = player.GetComponent<RealtimeView>();

            if (realtimeView != null && realtimeView.isOwnedLocallySelf)
            {
                Renderer objRenderer = GameObject.Find("Instruction").GetComponent<Renderer>();
                Texture newTexture;
                switch (pl.currentRole)
                {
                    case Role.None:
                        newTexture = NoneT;
                        break;
                    case Role.Explorer:
                        newTexture = ExplorerT;
                        break;
                    case Role.Collector:
                        newTexture = CollectorT;
                        break;
                    case Role.Tactical:
                        newTexture = TacticalT;
                        break;
                    default:
                        newTexture = NoneT;
                        Debug.Log("Invalid option selected.");
                        break;
                }

                if (objRenderer != null && newTexture != null)
                {
                    // Change the main texture of the material
                    objRenderer.material.mainTexture = newTexture;
                }
            }
        }

    }
    
    // Use the playermodels to sync the start room
    private void SyncPlayerModels()
    {
        Player[] players = FindObjectsOfType<Player>();
        Dictionary<GameObject, Role> updatedPlayerRoles = new Dictionary<GameObject, Role>();
        HashSet<Role> updatedTakenRoles = new HashSet<Role>();

        foreach (Player player in players) {
            Role playerModelRole = (Role)player.GetRole();
            Role playerCurrentRole = player.currentRole;

            if (playerModelRole != playerCurrentRole)
            {
                Debug.Log($"Player's model role: {playerModelRole}");
                Debug.Log($"Player's current role: {playerCurrentRole}");
                player.SetRole(playerModelRole);
            }

            UpdateRoleVisuals(playerModelRole, true);
            ChangePlayerMaterial(player.gameObject, playerModelRole);
            updatedPlayerRoles[player.gameObject] = playerModelRole;
            if ((int)playerModelRole != 0) {
                updatedTakenRoles.Add(playerModelRole);
            }
        }

        updatedPlayerRoles = playerRoles;
        updatedTakenRoles = takenRoles;
    }

    private void InitializeRoleTriggers()
    {
        foreach (Role role in System.Enum.GetValues(typeof(Role)))
        {
            if (role == Role.None) continue;
            Collider triggerCollider = GameObject.Find(role + "Trigger").GetComponent<Collider>();
            if (triggerCollider)
            {
                triggerRoles.Add(triggerCollider, role);
            }
        }
    }

    private void UpdateRoleVisuals(Role role, bool isTaken)
    {
        Collider roleCollider = GetColliderForRole(role);
        if (roleCollider)
        {
            Transform meshRendererTransform = roleCollider.transform.GetChild(0);
            Renderer meshRenderer = meshRendererTransform.GetComponent<Renderer>();
            Canvas canvas = meshRendererTransform.GetComponentInChildren<Canvas>();

            Material roleMaterial = GetMaterialForRole(role);
            meshRenderer.material = isTaken ? defaultMaterial : roleMaterial;
            canvas.enabled = !isTaken;
        }
    }

    private Collider GetColliderForRole(Role role)
    {
        foreach (var kvp in triggerRoles)
        {
            if (kvp.Value == role)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    private Material GetMaterialForRole(Role role)
    {
        switch (role)
        {
            case Role.Collector: return collectorMaterial;
            case Role.Tactical: return tacticalMaterial;
            case Role.Explorer: return explorerMaterial;
            default: return defaultMaterial;
        }
    }

    private void ChangePlayerMaterial(GameObject player, Role role)
    {
        Renderer playerRenderer = player.GetComponentInChildren<Renderer>();
        if (!playerRenderer) return;
        playerRenderer.material = role == Role.None ? defaultMaterial : GetMaterialForRole(role);
    }

    public bool AreAllRolesAssigned()
    {
        // Find all player GameObjects
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Check if the dictionary already contains all players
        if (playerRoles.Count < players.Length)
        {
            // Not all players have a role assigned
            return false;
        }

        // Check if any player has a Role.None
        foreach (var playerRole in playerRoles)
        {
            if (playerRole.Value == Role.None)
            {
                // A player has not selected a role
                return false;
            }
        }

        // All players have a role that is not None
        return true;
    }

}
