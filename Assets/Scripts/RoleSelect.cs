using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class RoleSelect : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material collectorMaterial;
    [SerializeField] private Material tacticalMaterial;
    [SerializeField] private Material explorerMaterial;

    private Dictionary<Collider, Role> triggerRoles = new Dictionary<Collider, Role>();
    private Dictionary<GameObject, Role> playerRoles = new Dictionary<GameObject, Role>();
    private HashSet<Role> takenRoles = new HashSet<Role>();

    private void Awake()
    {
        InitializeRoleTriggers();
    }

    public void HandlePlayerEnterTrigger(Collider triggerCollider, GameObject player)
    {
        Player playerController = player.GetComponent<Player>();
        RealtimeView realtimeView = player.GetComponent<RealtimeView>();
        if (realtimeView.isOwnedLocallyInHierarchy)
        {
            if (triggerRoles.TryGetValue(triggerCollider, out Role roleEntered))
            {
                Role currentRole = (Role)playerController.GetRole();
                if (currentRole == roleEntered)
                    FreeRole(playerController, player, roleEntered);
                else if (!takenRoles.Contains(roleEntered))
                    AssignRole(playerController, player, roleEntered, currentRole);
            }
        }
    }

    private void AssignRole(Player playerController, GameObject playerGameObject, Role newRole, Role oldRole)
    {
        if (oldRole != Role.None)
        {
            takenRoles.Remove(oldRole);
            UpdateRoleVisuals(oldRole, false);
        }

        playerController.SetRole(newRole); // This will update the model and sync the role
        playerRoles[playerGameObject] = newRole;
        takenRoles.Add(newRole);
        UpdateRoleVisuals(newRole, true);
        ChangePlayerMaterial(playerGameObject, newRole);
        Debug.Log($"Player {playerGameObject.name} has taken the role of {newRole}.");
    }

    private void FreeRole(Player playerController, GameObject playerGameObject, Role role)
    {
        playerController.SetRole(Role.None); // This will update the model and sync the role
        playerRoles[playerGameObject] = Role.None;
        takenRoles.Remove(role);
        UpdateRoleVisuals(role, false);
        ChangePlayerMaterial(playerGameObject, Role.None);
        Debug.Log($"Player {playerGameObject.name} removed their role. They are now {Role.None}.");
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
