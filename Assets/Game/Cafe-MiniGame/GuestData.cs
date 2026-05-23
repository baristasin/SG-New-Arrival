using System.Collections.Generic;

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
    public List<InterestType> Likes = new List<InterestType>();
    public InterestType Dislike;

    public GuestData(string name, List<InterestType> likes, InterestType dislike)
    {
        Name = name;
        Likes = likes;
        Dislike = dislike;
    }

}
