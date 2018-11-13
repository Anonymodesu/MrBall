using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : IEquatable<Level> {

	public const int numSubstages = 5;
    public const int numStages = 6;
	public const int numLevels = numStages * numSubstages;

	public readonly int stage;
	public readonly int substage;
		
	public Level(int stage, int substage) {
		this.stage = stage;
		this.substage = substage;
	}
		
	public override string ToString() {
		return "Scene_Stage " + stage + "-" + substage;
	}

	public bool Equals(Level other) {
		return !System.Object.ReferenceEquals(other, null) && (this.stage == other.stage) && (this.substage == other.substage);
	}

	public override bool Equals(System.Object other) {
		return !System.Object.ReferenceEquals(other, null) && (other is Level) 
				&& (this.stage == ((Level)other).stage) && (this.substage == ((Level)other).substage);
	}

	public override int GetHashCode() {
		return stage * numSubstages + numSubstages;
	}

	/*
	public static bool operator== (Level a, Level b) {
		return System.Object.ReferenceEquals(a, null) ? false : a.Equals(b);
	}

	public static bool operator!= (Level a, Level b) {
		return !(a == b);
	}
	*/

	public static void testLevel() {
		Level a = new Level(0, 1);
		Level b = new Level(0, 1);
		Level c = new Level(0, 2);
		System.Object d = new Level(0, 1);
		string e =  "blyat";
		Level f = null;
		
		if(!a.Equals(b)) Debug.Log("test 1 failed");
		if(!b.Equals(a)) Debug.Log("test 2 failed");
		if(!a.Equals(a)) Debug.Log("test 3 failed");
		if(a.Equals(c)) Debug.Log("test 4 failed");
		if(!a.Equals(d)) Debug.Log("test 5 failed");
		if(!d.Equals(a)) Debug.Log("test 6 failed");
		if(a.Equals(e)) Debug.Log("test 7 failed");
		if(a.Equals(f)) Debug.Log("test 8 failed");

		/*
		if(!(a == b)) Debug.Log("test 9 failed");
		if(!(b == a)) Debug.Log("test 10 failed");
		if(!(a == a)) Debug.Log("test 11 failed");
		if(a == c) Debug.Log("test 12 failed");
		if(!(a == d)) Debug.Log("test 13 failed");
		if(!(d == a)) Debug.Log("test 14 failed");
		if(a == f) Debug.Log("test 15 failed");
		if(f == a) Debug.Log("test 16 failed");
		*/
	}
}
