using System;
using Sirenix.Utilities;
using UnityEngine;

public class Table : MonoBehaviour
{
    public int tableID;
    public int table_row;
    public int table_col;

    public Spot northSpot;
    public Spot southSpot;
    public Spot westSpot;
    public Spot eastSpot;

    private Spot[] spots;

    void Awake()
    {
        if(table_col == 0 || table_col == 0){return;} //error forgot to assign row and colum
        tableID = (table_row * 10) + tableID;
        spots = new Spot[] {northSpot, southSpot, westSpot, eastSpot};
    }

    public int CalculateTableScore()
    {
        int boardgame_score = 0;
        int music_score = 0;
        int beer_score = 0;
        int books_score = 0;


        foreach (Spot spot in spots)
        {
            if(spot != null && !spot.free)
            {
                foreach(GuestData.InterestType like in spot.myGuest.Likes)
                {
                    if(like == GuestData.InterestType.Beer){beer_score++;}
                    if(like == GuestData.InterestType.Boardgames){boardgame_score++;}
                    if(like == GuestData.InterestType.Music){music_score++;}
                    if(like == GuestData.InterestType.Books){books_score++;}
                }
            }
        }
        
        int highest_score = Mathf.Max(beer_score, boardgame_score, music_score, books_score);
        if(highest_score == 4){highest_score = 6;}

        return highest_score; 
    }
}