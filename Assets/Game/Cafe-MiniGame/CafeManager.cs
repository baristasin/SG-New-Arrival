using UnityEngine;
using System.Collections.Generic;
//TODO this is AI generated code and is not yet applied to the current game.
public class CafeManager : MonoBehaviour
{
    public List<Table> allTables;
    
    private Queue<GuestData> guestQueue = new Queue<GuestData>();
    private GuestData currentGuest;
    
    // Reference to the active 3D character placeholder instance at the entrance
    public GameObject activeGuestObject; 

    void Start()
    {
        InitializeGuestQueue();
        SpawnNextGuest();
    }

    void Update()
    {
        // Detect Player clicking on a Spot
        if (Input.GetMouseButtonDown(0))
        {
            HandleSpotSelection();
        }
    }

    void InitializeGuestQueue()
    {
        // 1. Populate your 16 unique characters here using the list we discussed
        // 2. Enqueue them into guestQueue
    }

    void SpawnNextGuest()
    {
        if (guestQueue.Count > 0)
        {
            currentGuest = guestQueue.Dequeue();
            // Update your UI text here: "Hi, I like..."
            
            // Move your activeGuestObject back to the entrance spawn point
            // Reset its animations to 'Idle/Waiting'
        }
        else
        {
            CalculateFinalScore();
        }
    }

    void HandleSpotSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Spot clickedSpot = hit.collider.GetComponent<Spot>();

            if (clickedSpot != null && !clickedSpot.isOccupied)
            {
                // 1. Tell clickedSpot it is now reserved/occupied
                clickedSpot.SeatCharacter(currentGuest);

                // 2. Tell the NavMeshAgent on activeGuestObject to move to clickedSpot.transform.position
                // 3. Script on activeGuestObject handles switching to 'Walking' and then 'Sitting' on arrival

                // 4. Move to the next person in line
                SpawnNextGuest();
            }
        }
    }

    public void CalculateFinalScore()
    {
        // This is where you check the 4 hardcoded interfaces!
        // Example: Compare allTables[0].eastSpot.seatedCharacter with allTables[1].westSpot.seatedCharacter
        Debug.Log("Game Over! Checking synergies...");
    }
}