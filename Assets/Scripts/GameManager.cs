using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TurnManager turnManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnManager.StartNewHand();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Debug.LogWarning("Emergency stop triggered.");
        //     StopAllCoroutines();
        // }
    }
}
