using UnityEngine;

public enum Spotpos {North, South, West, East}
public class Spot : MonoBehaviour
{
    public Spotpos spotpos;
    public MeshRenderer spotMeshRenderer;
    private bool _free = true;
    public bool free
    {
        get { return _free; }
        set 
        {
            _free = value; 
            if(!_free){spotMeshRenderer.enabled = false;}
        }
    }
    public GuestData myGuest = null;

    public void takeThisSpot(GuestData guest)
    {
        myGuest = guest;
        free = false;
    }
}
