using UnityEngine;
using UnityEngine.AI;

public class Guest : MonoBehaviour
{
    private NavMeshAgent agent;
    private Spot targetSpot;
    private CafeManager manager;
    private bool isWalking = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void WalkToSpot(Spot spot, CafeManager cafeManager)
    {
        targetSpot = spot;
        manager = cafeManager;
        
        if (agent != null && targetSpot != null)
        {
            agent.SetDestination(targetSpot.transform.position);
            isWalking = true;
        }
    }

    void Update()
    {
        if (!isWalking) return;

        // check if arrived
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // arrived!
                    isWalking = false;
                    ArrivedAtSpot();
                }
            }
        }
    }

    private void ArrivedAtSpot()
    {
        // Tell Hazem he can get another guest!
        manager.OnGuestArrived(targetSpot);
    }
}