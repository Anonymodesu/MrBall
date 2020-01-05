using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BallType : int {None = 0, MrBall = 1, MrsBall = 2, MrBowl = 3, BalldyBuilder = 4, InvisiBall = 5,
							Intangiball = 6, BowlVaulter = 7, UsainBowl = 8, Reballious = 9, GyroBall = 10, NielsBall = 11,
							Unstoppaball = 12, Ballerina = 13, MrMister = 14, MrsMisses = 15};

public class Balls : MonoBehaviour {

	private struct BallDescription {
		public string name;
		public string description;
		public string powers;
		public int cubiesRequired;
		public float scoreMultiplier;

		public BallDescription(string name, string description, string powers, int cubiesRequired, float scoreMultiplier) {
			this.name = name;
			this.description = description;
			this.powers = powers;
			this.cubiesRequired = cubiesRequired;
			this.scoreMultiplier = scoreMultiplier;
		}
	}

	public static readonly int numBalls = Enum.GetNames(typeof(BallType)).Length;

	#pragma warning disable 0649

	[SerializeField]
	private List<Sprite> ballSprites;

	[SerializeField]
	private List<Material> balls;

	[SerializeField]
	private GameObject usainBowlEffect, perpendicularExplosion, perpendicularTrail, superExplosion, bowlVaulterEffect, playerTrail, 
						playerTrailGlow, rampImpactEffect, balldyBuilderEffect, intangiballHelper, intangiballPoof, ballerinaEffect,
						mrMisterEffect;

	[SerializeField]
	private Material intangiballTransparent;

	#pragma warning restore 0649

	public GameObject UsainBowlEffect {
		get { return usainBowlEffect; }
	}
	public GameObject PerpendicularExplosion {
		get { return perpendicularExplosion; }
	}
	public GameObject PerpendicularTrail {
		get { return perpendicularTrail; }
	}
	public GameObject SuperExplosion {
		get { return superExplosion; }
	}
	public GameObject BowlVaulterEffect {
		get { return bowlVaulterEffect; }
	}
	public GameObject PlayerTrail {
		get { return playerTrail; }
	}
	public GameObject PlayerTrailGlow {
		get { return playerTrailGlow; }
	}
	public GameObject RampImpactEffect {
		get { return rampImpactEffect; }
	}
	public GameObject BalldyBuilderEffect {
		get { return balldyBuilderEffect; }
	}
	public GameObject IntangiballHelper {
		get { return intangiballHelper; }
	}
	public GameObject IntangiballPoof {
		get { return intangiballPoof; }
	}
	public Material IntangiballTransparent {
		get { return intangiballTransparent; }
	}
	public GameObject BallerinaEffect {
		get { return ballerinaEffect; }
	}
	public GameObject MrMisterEffect {
		get { return mrMisterEffect; }
	}
	
	private Dictionary<BallType, BallDescription> ballDescriptions;

	void Awake() {

		ballDescriptions = new Dictionary<BallType, BallDescription>();
		ballDescriptions.Add(BallType.None, new BallDescription("None", 
			"What a hacker",
			"???",
			0, 0));
		ballDescriptions.Add(BallType.MrBall, new BallDescription("Mr. Ball", 
			"The breadwinner of the family. Old-fashioned and reliable.",
			"Points earned +10%",
			0, 1.1f));
		ballDescriptions.Add(BallType.MrsBall, new BallDescription("Mrs. Ball", 
			"Mr. Ball's ambitious wife who always wants just a little bit more.",
			"Points earned +20%. Can't brake.",
			1, 1.2f));
		ballDescriptions.Add(BallType.MrBowl, new BallDescription("Mr. Bowl", 
			"A close friend of Mrs. Ball. His family has a hereditary skin condition.",
			"Brake strength increased.",
			2, 1));
		ballDescriptions.Add(BallType.BalldyBuilder, new BallDescription("Balldy Builder", 
			"",
			"Turns a green ramp into a checkpoint.",
			3, 1));
		ballDescriptions.Add(BallType.InvisiBall, new BallDescription("Invisi Ball", 
			"Bit of a shut-in, she is seldom seen in the family.",
			"Points earned +50%.",
			4, 1.5f));
		ballDescriptions.Add(BallType.Intangiball, new BallDescription("Intangi Ball", 
			"",
			"Phases through matter.",
			5, 1));
		ballDescriptions.Add(BallType.BowlVaulter, new BallDescription("Bowl Vaulter", 
			"",
			"Do a super jump at any time.",
			6, 1));
		ballDescriptions.Add(BallType.UsainBowl, new BallDescription("Usain Bowl", 
			"",
			"Temporary super speed.",
			7, 1));
		ballDescriptions.Add(BallType.Reballious, new BallDescription("Reballious", 
			"They see me rolling...",
			"Can only roll left and backward. Points earned +50%.",
			8, 1));
		ballDescriptions.Add(BallType.GyroBall, new BallDescription("Gyro Ball", 
			"An offensive technique that grows stronger as the user slows down.",
			"Can change direction in midair.",
			9, 1));
		ballDescriptions.Add(BallType.NielsBall, new BallDescription("Niels Ball", 
			"The first Noball Laureate in Ball family.",
			"Reverses gravity.",
			10, 1));
		ballDescriptions.Add(BallType.Unstoppaball, new BallDescription("Unstoppaball", 
			"'Keeping us down is impossible,' exclaimed the brothers Unstoppaball and Impossiball.",
			"Cannot be stopped. Points earned +50%",
			11, 1.5f));
		ballDescriptions.Add(BallType.Ballerina, new BallDescription("Ballerina", 
			"",
			"Pirouettes to float.",
			12, 1));
		ballDescriptions.Add(BallType.MrMister, new BallDescription("Mr. Mister", 
			"Once an avid smoker, he has recently switched to vaping.",
			"Expels mist to reverse direction.",
			13, 1));
		ballDescriptions.Add(BallType.MrsMisses, new BallDescription("Mrs. Misses", 
			"May as well die in style.",
			"+5 points on death.",
			14, 1));
	}

	public Sprite getSprite(BallType type) {
		return ballSprites[(int) type];
	}

	public Material getBall(BallType type) {
		return balls[(int) type];
	}

	public int cubiesRequired(BallType type) {
		return ballDescriptions[type].cubiesRequired;
	}

	public float getScoreMultiplier(BallType type) {
		return ballDescriptions[type].scoreMultiplier;
	}

	public string getName(BallType type) {
		return ballDescriptions[type].name;
	}

	public string getDescription(BallType type) {
		return ballDescriptions[type].description;
	}

	public string getPowers(BallType type) {
		return ballDescriptions[type].powers;
	}

	//returns a list of newly acquired balls by collecting a total of newNumCubies
	//ignores balls that were acquired by collecting a total of oldNumCubies
	public IEnumerable<BallType> newBalls(int oldNumCubies, int newNumCubies) {
		int newBall = numBalls;

		//find the closest ball to acquisition, in terms of cubies
		for(int ball = 0; ball < numBalls; ball++) {
			if(oldNumCubies < ballDescriptions[(BallType) ball].cubiesRequired) {
				newBall = ball;
				break;
			}
		}

		for(int ball = newBall; ballDescriptions[(BallType) ball].cubiesRequired <= newNumCubies; ball++) {
			yield return (BallType) ball;
		}
	}

}
