using System.Collections.Generic;
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
        if (triggerRoles.TryGetValue(triggerCollider, out Role roleEntered))
        {
            if (playerRoles.TryGetValue(player, out Role currentRole))
            {
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
        }
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
}
