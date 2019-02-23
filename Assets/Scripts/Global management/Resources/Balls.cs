using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BallType : int {None = 0, MrBall = 1, MrsBall = 2, MrBowl = 3, BalldyBuilder = 4, InvisiBall = 5,
							Intangiball = 6, BowlVaulter = 7, UsainBowl = 8, Reballious = 9, GyroBall = 10, NielsBall = 11};

public class Balls : MonoBehaviour {

	private struct BallDescription {
		public string name;
		public string description;
		public string powers;

		public BallDescription(string name, string description, string powers) {
			this.name = name;
			this.description = description;
			this.powers = powers;
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
						playerTrailGlow, rampImpactEffect, balldyBuilderEffect, intangiballHelper, intangiballPoof;

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
	

	private int[] numCubiesRequired; //how many cubies are required to unlock each ball
	private float[] scoreMultipliers;
	private Dictionary<BallType, BallDescription> ballDescriptions;

	void Awake() {
		numCubiesRequired = new int[numBalls];
		numCubiesRequired[(int) BallType.None] = 0;
		numCubiesRequired[(int) BallType.MrBall] = 0;
		numCubiesRequired[(int) BallType.MrsBall] = 0;
		numCubiesRequired[(int) BallType.MrBowl] = 0;
		numCubiesRequired[(int) BallType.BalldyBuilder] = 0;
		numCubiesRequired[(int) BallType.InvisiBall] = 0;
		numCubiesRequired[(int) BallType.Intangiball] = 0;
		numCubiesRequired[(int) BallType.BowlVaulter] = 0;
		numCubiesRequired[(int) BallType.UsainBowl] = 0;

		scoreMultipliers = new float[numBalls];
		scoreMultipliers[(int) BallType.None] = 0;
		scoreMultipliers[(int) BallType.MrBall] = 1.1f;
		scoreMultipliers[(int) BallType.MrsBall] = 1.2f;
		scoreMultipliers[(int) BallType.MrBowl] = 1;
		scoreMultipliers[(int) BallType.BalldyBuilder] = 1;
		scoreMultipliers[(int) BallType.InvisiBall] = 1.5f;
		scoreMultipliers[(int) BallType.Intangiball] = 1;
		scoreMultipliers[(int) BallType.BowlVaulter] = 1;
		scoreMultipliers[(int) BallType.UsainBowl] = 1;
		scoreMultipliers[(int) BallType.Reballious] = 1.5f;
		scoreMultipliers[(int) BallType.GyroBall] = 1;
		scoreMultipliers[(int) BallType.NielsBall] = 1;

		ballDescriptions = new Dictionary<BallType, BallDescription>();
		ballDescriptions.Add(BallType.MrBall, new BallDescription("Mr. Ball", 
			"The breadwinner of the family. Old-fashioned and reliable.",
			"Points earned +10%"));
		ballDescriptions.Add(BallType.MrsBall, new BallDescription("Mrs. Ball", 
			"Mr. Ball's ambitious wife who always wants just a little bit more.",
			"Points earned +20%. Can't brake."));
		ballDescriptions.Add(BallType.MrBowl, new BallDescription("Mr. Bowl", 
			"A close friend of Mrs. Ball. His family has a hereditary skin condition.",
			"Brake strength increased."));
		ballDescriptions.Add(BallType.BalldyBuilder, new BallDescription("Balldy Builder", 
			"",
			"Turns a green ramp into a checkpoint."));
		ballDescriptions.Add(BallType.InvisiBall, new BallDescription("Invisi Ball", 
			"Bit of a shut-in, she is seldom seen in the family.",
			"Points earned +50%."));
		ballDescriptions.Add(BallType.Intangiball, new BallDescription("Intangi Ball", 
			"",
			"Phases through matter."));
		ballDescriptions.Add(BallType.BowlVaulter, new BallDescription("Bowl Vaulter", 
			"",
			"Gives a boost to current velocity."));
		ballDescriptions.Add(BallType.UsainBowl, new BallDescription("Usain Bowl", 
			"",
			"Temporary super speed."));
		ballDescriptions.Add(BallType.Reballious, new BallDescription("Reballious", 
			"They see me rolling...",
			"Can only roll left and backward. Points earned +50%."));
		ballDescriptions.Add(BallType.GyroBall, new BallDescription("Gyro Ball", 
			"Uses super state-of-the-art gyroscope technology.",
			"Can change direction in midair."));
		ballDescriptions.Add(BallType.NielsBall, new BallDescription("Niels Ball", 
			"The first Noball Laureate in Ball family.",
			"Reverses gravity."));
	}

	public Sprite getSprite(BallType type) {
		return ballSprites[(int) type];
	}

	public Material getBall(BallType type) {
		return balls[(int) type];
	}

	public int cubiesRequired(BallType type) {
		return numCubiesRequired[(int) type];
	}

	public float getScoreMultiplier(BallType type) {
		return scoreMultipliers[(int) type];
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

}
