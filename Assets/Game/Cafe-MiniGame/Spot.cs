using UnityEngine;

public enum Spotpos {North, South, West, East}
public class Spot : MonoBehaviour
{
    public Spotpos spotpos;
    public bool free = true;
    public GuestData myGuest = null;

    public void takeThisSpot(GuestData guest)
    {
        myGuest = guest;
        free = false;
    }
}
