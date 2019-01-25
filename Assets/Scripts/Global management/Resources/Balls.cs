using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BallType : int {None = 0, MrBall = 1, MrsBall = 2, MrBowl = 3, BalldyBuilder = 4, InvisiBall = 5,
							Intangiball = 6, BowlVaulter = 7, UsainBowl = 8};

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

	[SerializeField]
	private List<Sprite> ballSprites;

	[SerializeField]
	private List<GameObject> balls;

	private int[] numCubiesRequired; //how many cubies are required to unlock each ball
	private Dictionary<BallType, BallDescription> ballDescriptions;

	void Start() {
		numCubiesRequired = new int[numBalls];
		numCubiesRequired[(int) BallType.None] = 0;
		numCubiesRequired[(int) BallType.MrBall] = 0;
		numCubiesRequired[(int) BallType.MrsBall] = 2;
		numCubiesRequired[(int) BallType.MrBowl] = 3;
		numCubiesRequired[(int) BallType.BalldyBuilder] = 4;
		numCubiesRequired[(int) BallType.InvisiBall] = 6;
		numCubiesRequired[(int) BallType.Intangiball] = 7;
		numCubiesRequired[(int) BallType.BowlVaulter] = 8;
		numCubiesRequired[(int) BallType.UsainBowl] = 9;

		ballDescriptions = new Dictionary<BallType, BallDescription>();
		ballDescriptions.Add(BallType.MrBall, new BallDescription("Mr. Ball", 
			"The breadwinner of the family. Old-fashioned and reliable.",
			"None."));
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
			"Do 1 super jump at any time."));
		ballDescriptions.Add(BallType.UsainBowl, new BallDescription("Usain Bowl", 
			"",
			"Temporary super speed."));
	}

	public Sprite getSprite(BallType type) {
		return ballSprites[(int) type];
	}

	public GameObject getBall(BallType type) {
		return balls[(int) type];
	}

	public int cubiesRequired(BallType type) {
		return numCubiesRequired[(int) type];
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
