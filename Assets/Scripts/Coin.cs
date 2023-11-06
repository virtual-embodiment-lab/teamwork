using System.Collections;
using System.Linq;
using Normal.Realtime;
using TMPro;
using UnityEngine;

/*
 * Coin instance manager
 */
public class Coin : RealtimeComponent<CoinModel>
{
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] GameObject particles = null;
    [SerializeField] GameObject nextCoin = null;
    [SerializeField] string nextCoinColor = null;
    [SerializeField] Material coinHidden = null;
    [SerializeField] Material coinFound = null;
    [SerializeField] GameObject tmp = null;
    [SerializeField] private float detectionDistance = 10f;
    private GameManager gameManager;

    protected void Start()
    {
        GetComponent<Renderer>().material = coinHidden;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);

        Player[] players = FindObjectsOfType<Player>();
        if (gameObject.layer == LayerMask.NameToLayer("Hidden"))
        {
            foreach (var player in players)
            {
                if (player.currentRole == Role.Explorer)
                {
                    float distanceToExplorer = Vector3.Distance(transform.position, player.transform.position);
                    if (distanceToExplorer <= detectionDistance)
                    {
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
            onFound();
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Collector) && model.found && !model.collected)
            onCollected();
    }

    public void onFound()
    {
        model.found = true;
        GetComponent<Renderer>().material = coinFound;
    }

    public void onCollected()
    {
        model.collected = true;
        gameManager.IncrementCoinsCollected();
        GetComponent<Renderer>().enabled = false;
        _ = Realtime.Instantiate(particles.name, transform.position, Quaternion.identity, new Realtime.InstantiateOptions { });
        StartCoroutine(SetCoinTextAfterInstantiation(nextCoinColor));
    }

    private IEnumerator SetCoinTextAfterInstantiation(string nextCoinColor)
    {
        GameObject nextCoinObject = Realtime.Instantiate(nextCoin.name, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, new Realtime.InstantiateOptions { });
        yield return null;
        tmp = nextCoinObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        tmp.GetComponent<TextMeshProUGUI>().text = $"Next Coin:\n{nextCoinColor}";
    }

    protected override void OnRealtimeModelReplaced(CoinModel previousModel, CoinModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events on the previous model
            previousModel.foundDidChange -= FoundDidChange;
            previousModel.collectedDidChange -= CollectedDidChange;
        }

        if (currentModel != null)
        {
            // Register for events on the new model
            currentModel.foundDidChange += FoundDidChange;
            currentModel.collectedDidChange += CollectedDidChange;

            // Update the material to reflect the current state
            UpdateVisualState();
        }
    }

    private void FoundDidChange(CoinModel model, bool found)
    {
        // Update the material when the found state changes
        UpdateVisualState();
    }

    private void CollectedDidChange(CoinModel model, bool collected)
    {
        // Update the GameManager when the coin is collected
        if (collected)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    private void UpdateVisualState()
    {
        if (model != null)
        {
            GetComponent<Renderer>().material = model.found ? coinFound : coinHidden;
            if (model.collected)
            {
                GetComponent<Renderer>().enabled = false;
                // Perform any additional cleanup if necessary
                //Destroy(gameObject);
            }
        }
    }
}
