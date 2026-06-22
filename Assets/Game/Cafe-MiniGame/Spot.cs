using UnityEngine;
public enum Spotpos {North, South, West, East}
public class Spot : MonoBehaviour
{
    public Spotpos spotpos;
    public MeshRenderer spotMeshRenderer;
    public Material myNormalMat;
    public Material myHoverMat; 
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

    void Start()
    {
        if(spotMeshRenderer == null)
        {
            spotMeshRenderer = GetComponent<MeshRenderer>();
        }

        if(spotMeshRenderer != null && myNormalMat != null)
        {
            spotMeshRenderer.material = myNormalMat;
        }
    }

    public void takeThisSpot(GuestData guest)
    {
        myGuest = guest;
        free = false;
    }

    private void OnMouseEnter()
    {
        if (free && spotMeshRenderer != null && myHoverMat != null)
        {
            spotMeshRenderer.material = myHoverMat;
        }
    }
    private void OnMouseExit()
    {
       if (free && spotMeshRenderer != null && myNormalMat != null)
        {
            spotMeshRenderer.material = myNormalMat;
        }
    }
}
