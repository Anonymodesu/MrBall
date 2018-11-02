﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HighScore {

    string name;
    int cubies;
    int deaths;
    int time;
    
    public const int deathPoints = -10;
    public const int cubiePoints = 100;
    
    public HighScore(string name, int cubies, int deaths, int time) {
        this.name = name;
        this.cubies = cubies;
        this.deaths = deaths;
        this.time = time;
    }
    
    public string display() {
        return name + "\t\t" + cubies + "\t\t" + deaths + "\t\t" + time + "\t\t" + calculateScore(cubies,deaths,time) + "\n"; 
    }
    
    public string Name() { return name; }
    public int Cubies() { return cubies; }
    public int Deaths() { return deaths; }
    public int Time() { return time; }

    public static int calculateScore(int cubies, int deaths, int time) {
        return 1000 + cubies * cubiePoints + deaths * deathPoints - time;
    }
    
    public int calculateScore() {
        return HighScore.calculateScore(this.cubies,this.deaths,this.time);
    }
}