using System.Collections.Generic;
using UnityEngine;

public class GuestData
{
    public enum InterestType
    {
        Boardgames,
        Music,
        Beer,
        Books
    }

    public string Name;
    public Sprite Portrait {get; private set;}
    public GameObject CharacterPrefab {get; private set;}
    public List<InterestType> Likes = new List<InterestType>();
    public InterestType Dislike;

    public GuestData(string name, List<InterestType> likes, InterestType dislike, Sprite portrait, GameObject characterPrefab)
    {
        Name = name;
        Likes = likes;
        Dislike = dislike;
        Portrait = portrait;
        CharacterPrefab = characterPrefab; 
    }

}
