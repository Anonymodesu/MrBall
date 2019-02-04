using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HighScore {

    public readonly string name;
    public readonly int cubies;
    public readonly int deaths;
    public readonly float time;
    public readonly float scoreMultiplier;
    
    public const int deathPoints = -10;
    public const int cubiePoints = 100;
    
    public HighScore(string name, int cubies, int deaths, float time, float scoreMultiplier) {
        this.name = name;
        this.cubies = cubies;
        this.deaths = deaths;
        this.time = time;
        this.scoreMultiplier = scoreMultiplier;
    }
    
    public string display() {
        return name + "\t\t" + cubies + "\t\t" + deaths + "\t\t" + time.ToString("0.00") + "\t\t" + calculateScore() + "\n"; 
    }
    
    public string Name() { return name; }
    public int Cubies() { return cubies; }
    public int Deaths() { return deaths; }
    public float Time() { return time; }

    public static int calculateScore(int cubies, int deaths, float time, float scoreMultiplier) {
        return (int) (scoreMultiplier * (1000 + cubies * cubiePoints + deaths * deathPoints - time));
    }
    
    public int calculateScore() {
        return HighScore.calculateScore(this.cubies,this.deaths,this.time,this.scoreMultiplier);
    }

}
