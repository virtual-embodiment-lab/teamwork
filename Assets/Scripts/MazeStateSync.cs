using UnityEngine;
using Normal.Realtime;

public class MazeStateSync : RealtimeComponent<MazeStateModel>
{
    [SerializeField] private GameObject[] mazes; // Assign your mazes in the inspector.

    protected override void OnRealtimeModelReplaced(MazeStateModel previousModel, MazeStateModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.activeMazeDidChange -= ActiveMazeDidChange;
        }
        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                currentModel.activeMaze = -1; // -1 to represent no maze is active initially.
            }
            currentModel.activeMazeDidChange += ActiveMazeDidChange;
            UpdateActiveMaze(currentModel.activeMaze);
        }
    }

    public bool IsMazeSelected()
    {
        // Assuming this script is on the same GameObject as MazeStateSync, or you have a reference to it
        MazeStateSync mazeStateSync = GetComponent<MazeStateSync>();

        // If MazeStateSync isn't on this GameObject, you might find it like this:
        // MazeStateSync mazeStateSync = FindObjectOfType<MazeStateSync>();

        // Check if the mazeStateSync is not null and the activeMaze is a non-negative number (meaning a maze has been selected)
        return mazeStateSync != null && mazeStateSync.model.activeMaze >= 0;
    }


    private void ActiveMazeDidChange(MazeStateModel model, int value)
    {
        UpdateActiveMaze(value);
    }

    public void SetActiveMaze(int mazeIndex)
    {
        model.activeMaze = mazeIndex;
    }

    private void UpdateActiveMaze(int mazeIndex)
    {
        for (int i = 0; i < mazes.Length; i++)
        {
            bool isActive = i == mazeIndex;
            mazes[i].SetActive(isActive);
        }
    }
}
