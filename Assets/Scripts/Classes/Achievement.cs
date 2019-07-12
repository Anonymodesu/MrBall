using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Achievement { //used for both storing requirements for a stage's achievements as well as player records
	//time and deaths are stored as their negatives so that Max could be applied to all of them

	public class Evaluation {
		public bool requirementSatisfied;
		public bool newRequirementSatisfied;
		public String field;
		public String record;
		public String requirement;
	}

	private static readonly String[] fieldNames = new String[] {"Cubies", "Deaths", "Time", "Points"};

	//in the format cubies, deaths, time, points
	private float[] fields;

	public const int numFields = 4;

	public int Cubies {
		get { return (int) fields[0]; }
	}

	public int Deaths {
		get { return - (int) fields[1]; }
	}

    public string TimeString {
        get { return Time.ToString("0.00"); }
    }

    public float Time {
        get { return -fields[2]; }
    }

    public int Points {
		get { return (int) fields[3]; }
	}
	
	public Achievement(int cubies, int deaths, float time, int points) {
		fields = new float[numFields];
		fields[0] = cubies;
		fields[1] = -deaths;
		fields[2] = -time;
		fields[3] = points;
	}

	public Achievement(Achievement other) {
		fields = new float[numFields];
		for(int i = 0; i < fields.Length; i++) {
			fields[i] = other.fields[i];
		}
	}

	public String[] displayValues() {
		String[] ret = new String[numFields];
		ret[0] = Cubies.ToString();
		ret[1] = Deaths.ToString();
		ret[2] = TimeString;
		ret[3] = Points.ToString();
		return ret;
	}

	public override String ToString() {
		String ret = "";
		for(int i = 0; i < numFields; i++) {
			ret += fieldNames[i] + ": " + (int)fields[i] + "; ";
		}
		ret += "\n";
		return ret;
	}

	//returns the number of achievements obtained by satisfying these requirements 
	public int numSatisfied(Achievement required) {
		int num = 0;

		for(int i = 0; i < numFields; i++) {
			if(this.fields[i] >= required.fields[i]) {
				num++;
			}
		}

		return num;
	}

	//returns an iterable series of evaluations for each record in current
	public static IEnumerable<Evaluation> EvaluateAchievement(Achievement required, 
															Achievement current = null, Achievement old = null) {
		for(int i = 0; i < numFields; i++) {
			
			bool currentRecordSatisfies = false;
			if(current != null) {
				currentRecordSatisfies = current.fields[i] >= required.fields[i];
			}

			//if oldRecord is null, then this is the first time completing the level
			bool oldRecordDoesNotSatisfy = true;
			if(old != null) {
				oldRecordDoesNotSatisfy = old.fields[i] < required.fields[i];
			}

			yield return new Evaluation {
				requirementSatisfied = currentRecordSatisfies,
				newRequirementSatisfied = currentRecordSatisfies && oldRecordDoesNotSatisfy,
				field = fieldNames[i],
				record = current == null ? "-" : current.FieldToString(i),
				requirement = required.FieldToString(i)
			};
		}
	}

	//returns the string representation of a record
	private String FieldToString(int i){
		switch(i) {
			case 0: //cubies
				return fields[0].ToString();
			case 1: //deaths
				return (-fields[1]).ToString();
			case 2: //time
				return (-fields[2]).ToString("0.00");
			case 3: //points
				return fields[3].ToString();
		}
		return "wrong index fed into Achievement.FieldToString()";
	}

	//returns the best achievements between the two
	public static Achievement Max(Achievement a, Achievement b) {
		Achievement newAchievement = new Achievement(0,0,0,0);
		for(int i = 0; i < numFields; i++) {
			newAchievement.fields[i] = Math.Max(a.fields[i], b.fields[i]);
		}

		return newAchievement;
	}

	

}
