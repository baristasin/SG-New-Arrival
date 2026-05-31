using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using TMPro;

public class CafeManager : MonoBehaviour
{
    [Header("Setup")]
    public List<Table> allTables;
    public GameObject GuestPrefab;

    public TextMeshProUGUI scoreText;
    
    private Table[,] tableGrid = new Table[5, 5]; 
    
    private Queue<GuestData> guestQueue = new Queue<GuestData>();
    private GuestData currentGuest;
  


    
    [Header("Active Guest Interaction")]
    public GameObject activeGuestObject; 
    public Transform spawnPoint;

    private GameObject waitingGuestVisuals;

    void Start()
    {
        InitializeTableGrid();
        InitializeGuestQueue();
        SpawnNextGuest();
        scoreText.text = "Score: 00";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleSpotSelection();
        }
    }

    // sort tables
    void InitializeTableGrid()
    {
        foreach (Table table in allTables)
        {
            if (table != null)
            {
                tableGrid[table.table_row, table.table_col] = table;
            }
        }
    }

    // Create a deck of guests
    void InitializeGuestQueue()
    {
        var BG = GuestData.InterestType.Boardgames;
        var MU = GuestData.InterestType.Music;
        var BE = GuestData.InterestType.Beer;
        var BK = GuestData.InterestType.Books;

        // BG - Group
        guestQueue.Enqueue(new GuestData("Gamer 1", new List<GuestData.InterestType> { BG, BE }, MU));
        guestQueue.Enqueue(new GuestData("Gamer 2", new List<GuestData.InterestType> { BG, BK }, BE));
        guestQueue.Enqueue(new GuestData("Gamer 3", new List<GuestData.InterestType> { BG, MU }, BK));
        guestQueue.Enqueue(new GuestData("Gamer 4", new List<GuestData.InterestType> { BG, BE }, BK));

        // BK - Group
        guestQueue.Enqueue(new GuestData("Reader 1", new List<GuestData.InterestType> { BK, BE }, MU));
        guestQueue.Enqueue(new GuestData("Reader 2", new List<GuestData.InterestType> { BK, MU }, BG));
        guestQueue.Enqueue(new GuestData("Reader 3", new List<GuestData.InterestType> { BK, BG }, BE));
        guestQueue.Enqueue(new GuestData("Reader 4", new List<GuestData.InterestType> { BK, MU }, BE));

        // BE - Group
        guestQueue.Enqueue(new GuestData("Bar-Fan 1", new List<GuestData.InterestType> { BE, MU }, BK));
        guestQueue.Enqueue(new GuestData("Bar-Fan 2", new List<GuestData.InterestType> { BE, BG }, MU));
        guestQueue.Enqueue(new GuestData("Bar-Fan 3", new List<GuestData.InterestType> { BE, BK }, BG));
        guestQueue.Enqueue(new GuestData("Bar-Fan 4", new List<GuestData.InterestType> { BE, MU }, BG));

        // MU - Group
        guestQueue.Enqueue(new GuestData("Concert-Goer 1", new List<GuestData.InterestType> { MU, BG }, BK));
        guestQueue.Enqueue(new GuestData("Concert-Goer 2", new List<GuestData.InterestType> { MU, BE }, BG));
        guestQueue.Enqueue(new GuestData("Concert-Goer 3", new List<GuestData.InterestType> { MU, BK }, BG));
        guestQueue.Enqueue(new GuestData("Concert-Goer 4", new List<GuestData.InterestType> { MU, BE }, BK));
    }

    void SpawnNextGuest()
    {   scoreText.text = $"Score: {CalculateFinalScore()}";
        if (guestQueue.Count > 0)
        {
            currentGuest = guestQueue.Dequeue();
            
            Debug.Log($"NextGuest: {currentGuest.Name}. Likes: {currentGuest.Likes[0]} & {currentGuest.Likes[1]}. Hates: {currentGuest.Dislike}");

            //spawn guest nexto Hazem
            if (GuestPrefab != null && spawnPoint != null)
            {
                Vector3 waitPosition = spawnPoint.position + new Vector3(1.5f, 0, 0);
                waitingGuestVisuals = Instantiate(GuestPrefab, waitPosition, spawnPoint.rotation);

                //adjust speechBubblo
                GuestVisuals guestVisuals = waitingGuestVisuals.GetComponent<GuestVisuals>();
                if (guestVisuals != null)
                {
                    guestVisuals.SetupSpeechBubble(currentGuest);
                }
            }


            // Bring Hazem back
            if (activeGuestObject != null && spawnPoint != null)
            {
                NavMeshAgent agent = activeGuestObject.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.Warp(spawnPoint.position);
                }
            }
            
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

            if (clickedSpot != null && clickedSpot.free)
            {
                clickedSpot.takeThisSpot(currentGuest);

                if (activeGuestObject != null)
                {
                    Guest guest = activeGuestObject.GetComponent<Guest>();
                    if (guest != null)
                    {
                        guest.WalkToSpot(clickedSpot,this);
                    }
                }
            }
        }
    }

    public void OnGuestArrived(Spot arrivedSpot)
    {
        if (waitingGuestVisuals != null && arrivedSpot != null)
        {
            // move Guest
            waitingGuestVisuals.transform.position = arrivedSpot.transform.position;
            waitingGuestVisuals.transform.rotation = arrivedSpot.transform.rotation;
            waitingGuestVisuals.transform.SetParent(arrivedSpot.transform);

            // active speechBubblo
            GuestVisuals visualScript = waitingGuestVisuals.GetComponent<GuestVisuals>();
            if (visualScript != null)
            {
                visualScript.HideSpeechBubble();
            }

            //placeholder colours
            var renderer = waitingGuestVisuals.GetComponent<Renderer>();
            if (renderer != null && arrivedSpot.myGuest.Likes.Count > 0)
            {
                if (arrivedSpot.myGuest.Likes[0] == GuestData.InterestType.Beer) renderer.material.color = Color.yellow;
                if (arrivedSpot.myGuest.Likes[0] == GuestData.InterestType.Books) renderer.material.color = Color.blue;
                if (arrivedSpot.myGuest.Likes[0] == GuestData.InterestType.Boardgames) renderer.material.color = Color.green;
                if (arrivedSpot.myGuest.Likes[0] == GuestData.InterestType.Music) renderer.material.color = Color.cyan;
            }

            // empty the car
            waitingGuestVisuals = null;
        }

        SpawnNextGuest();
    }
    public int CalculateFinalScore()
    {
        Debug.Log("All seats taken final score is...");

        int finalScore = 0;

        //Table_score
        foreach (Table table in allTables)
        {
            if (table != null)
            {
                int tableScore = table.CalculateTableScore();
                finalScore += tableScore;
                Debug.Log($"Tisch {table.tableID} erzielt {tableScore} Punkte.");
            }
        }

        // synergy
        int synergyBonus = 0;

        // run through the matrix
        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                Table currentTable = tableGrid[r, c];
                if (currentTable == null) continue;

                // CHECK East/West
                if (c + 1 < 5)
                {
                    Table rightTable = tableGrid[r, c + 1];
                    if (rightTable != null)
                    {
                        synergyBonus += EvaluateSpotPair(currentTable.eastSpot, rightTable.westSpot);
                    }
                }

                // CHECK Up/Down
                if (r + 1 < 5)
                {
                    Table bottomTable = tableGrid[r + 1, c];
                    if (bottomTable != null)
                    {
                        synergyBonus += EvaluateSpotPair(currentTable.southSpot, bottomTable.northSpot);
                    }
                }
            }
        }

        finalScore += synergyBonus;
        Debug.Log($"Synergie-Bonus/Malus total: {synergyBonus} Points.");
        Debug.Log($"=== final result: {finalScore} points ===");
        return finalScore;
    }

    // copare two spots
    private int EvaluateSpotPair(Spot spotA, Spot spotB)
    {
        if (spotA == null || spotB == null || spotA.free || spotB.free) return 0;

        int scoreDelta = 0;

        // shared interest
        foreach (GuestData.InterestType likeA in spotA.myGuest.Likes)
        {
            if (spotB.myGuest.Likes.Contains(likeA))
            {
                scoreDelta += 1; //this is a magic number maybe chage this to a var (common intrest score)
                
                //maybe fancy stuff for soulmates
            }
        }

        // Conflicts: Wenn einer das Desinteresse des anderen triggert
        if (spotB.myGuest.Likes.Contains(spotA.myGuest.Dislike)) scoreDelta -= 2; //this is a magic number maybe chage this to a var (reason for a mortal Kombat) //insert fatalities here
        if (spotA.myGuest.Likes.Contains(spotB.myGuest.Dislike)) scoreDelta -= 2;

        return scoreDelta;
    }

    
}