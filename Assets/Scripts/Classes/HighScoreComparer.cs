using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreComparer : Comparer<HighScore> {

    public override int Compare(HighScore a, HighScore b) {
        if(a == null && b == null) {
            return 0; 
            
        } else if(a == null) { // null (empty high scores) always ranked lower
            return 1;
            
        } else if(b == null) { 
            return -1;
            
        } else {
            return  b.calculateScore() - a.calculateScore();
        }
    }
}
