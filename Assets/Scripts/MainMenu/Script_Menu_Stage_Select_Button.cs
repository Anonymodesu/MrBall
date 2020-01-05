using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Script_Menu_Stage_Select_Button : Button {


	protected UI_Achievement[] achievementText;
	protected Text achievementsTitleText;

	protected Text levelText;

	protected Image levelImage; //the UI container for the image
	protected Level level;
	protected LevelImages levelImages; //a list storing all level images

	// Use this for initialization
	override protected void Start () {
		base.Start();
		GetComponent<Button>().onClick.AddListener(delegate {StartLevel(); });

		achievementText = new UI_Achievement[4];
		achievementText[0] = GameObject.Find("CubiesText").GetComponent<UI_Achievement>();
		achievementText[1] = GameObject.Find("DeathsText").GetComponent<UI_Achievement>();
		achievementText[2] = GameObject.Find("TimeText").GetComponent<UI_Achievement>();
		achievementText[3] = GameObject.Find("ScoreText").GetComponent<UI_Achievement>();

		achievementsTitleText = GameObject.Find("Achievements").transform.Find("Title").GetComponent<Text>();

		levelText = GameObject.Find("LevelName").GetComponent<Text>();

		levelImage = GameObject.Find("LevelImage").GetComponent<Image>();
		levelImages = GameObject.Find("LevelImages").GetComponent<LevelImages>();
	}

	
	public override void OnPointerEnter(PointerEventData eventData) {
		
		Level level = GameManager.getInstance().getLevel(GetComponentInChildren<Text>().text); //text in button corresponds to the level
		levelText.text = LevelData.getInstance().getLevelName(level);
		
		Achievement current = PlayerManager.getInstance().getRecord(SettingsManager.CurrentPlayer, level);
		Achievement required = GameManager.getInstance().getRequirement(level);
		
		displayRecords(current, required);
		
		levelImage.sprite = levelImages.levelImages[level.stage * Level.numSubstages + level.substage];
		
		achievementsTitleText.text = "Records";
    }


    protected void displayRecords(Achievement current, Achievement required) {

    	//loop over the two lists simultaneously
    	var e1 = Achievement.EvaluateAchievement(required, current).GetEnumerator();
    	var e2 = achievementText.GetEnumerator(); 
    	{
    		while(e1.MoveNext() && e2.MoveNext()) {
    			var eval = e1.Current;
    			UI_Achievement achievementData = (UI_Achievement) e2.Current;

    			achievementData.loadValues(eval, false);
    		}
    	}
    }
	
	public void setLevel(Level level) {
		this.level = level;
	}

	public virtual void StartLevel() { //only for buttons in level select menu
		SettingsManager.QuickSaveLoaded = false;
        level.Load();
    }
}
