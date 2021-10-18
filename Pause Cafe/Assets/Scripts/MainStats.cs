using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Classifiers;
using Classifiers1;
using Classifiers2;
using AI_Class;

public class MainStats : MonoBehaviour {
	
	public GameObject classifierStatsGO;
	public GameObject classifierContainerGO;
	public GameObject displayClassifierGO;
	public GameObject displayNbClassifiersGO;
	public Button buttonBack;
	bool buttonBackPressed = false;
	
	public int scrollAmount;
	public bool forceUpdate;
	public List<Classifier1> classifierList1;
	public List<Classifier2> classifierList2;
	
	public List<GameObject> classifierContainerList;
	public int classifierType; // which classifier list is displayed
	
    // Start is called before the first frame update
    void Start(){
        buttonBack.onClick.AddListener(buttonBackPressed_);
		scrollAmount = 0;
		for (int i=0;i<20;i++){
			GameObject go = GameObject.Instantiate(classifierStatsGO,classifierContainerGO.transform);
			classifierContainerList.Add(go);
			go.transform.position = new Vector3(go.transform.position.x,go.transform.position.y-i*24,go.transform.position.z);
		}
		forceUpdate = true;
		
		// Copy the list
		classifierList1 = new List<Classifier1>();
		if (AIHard.rules != null){
			foreach (Classifier1 c in AIHard.rules.classifiers){
				classifierList1.Add(c);
			}
		}
		classifierList2 = new List<Classifier2>();
		if (AIMedium.rules != null){
			foreach (Classifier1 c in AIMedium.rules.classifiers){
				classifierList1.Add(c);
			}
		}
		
		classifierType = 0;
		
		displayNbClassifiersGO.GetComponent<Text>().text = "Classifiers : " + classifierList1.Count;
    }

    // Update is called once per frame
    void Update(){
		// Change classifier type
		if (Input.GetKeyDown(KeyCode.E)){
			classifierType = (classifierType+1)%2;
			if (classifierType == 0) displayNbClassifiersGO.GetComponent<Text>().text = "Classifiers : " + classifierList1.Count;
			else if (classifierType == 1) displayNbClassifiersGO.GetComponent<Text>().text = "Classifiers : " + classifierList2.Count;
			scrollAmount = 0;
			forceUpdate = true;
		}
		if (Input.GetKeyDown(KeyCode.A)){
			if (classifierType == 0) ClassifierList<Classifier1>.sortByFitness2(classifierList1);
			else ClassifierList<Classifier2>.sortByFitness2(classifierList2);
			forceUpdate = true;
		}
		if (Input.GetKeyDown(KeyCode.Z)){
			if (classifierType == 0) ClassifierList<Classifier1>.sortByFitness(classifierList1);
			else ClassifierList<Classifier2>.sortByFitness(classifierList2);
			forceUpdate = true;
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0.0f || forceUpdate){
			forceUpdate = false;
			scrollAmount -= (int)(Input.GetAxis("Mouse ScrollWheel")*10.0);
			if (classifierType == 0){
				for (int i=0;i<20;i++){
					if (i+scrollAmount >= 0 && i+scrollAmount < classifierList1.Count){
						classifierContainerList[i].SetActive(true);
						setClassifierStats(classifierContainerList[i],classifierList1[i+scrollAmount]);
					}else{
						classifierContainerList[i].SetActive(false);
					}
				}
			}else{
				for (int i=0;i<20;i++){
					if (i+scrollAmount >= 0 && i+scrollAmount < classifierList2.Count){
						classifierContainerList[i].SetActive(true);
						setClassifierStats(classifierContainerList[i],classifierList2[i+scrollAmount]);
					}else{
						classifierContainerList[i].SetActive(false);
					}
				}
			}
			
			
		}
        if (buttonBackPressed){
			SceneManager.LoadScene(0);
			buttonBackPressed = false;
		}
		
		if (Input.GetMouseButton(0)){
			Vector3 mousePos = Input.mousePosition;
			for (int i=0;i<20;i++){
				GameObject go = classifierContainerList[i];
				if (go.activeInHierarchy){
					Vector3 pos = go.transform.position;
					if (mousePos.x >= pos.x-500 && mousePos.y < pos.y && mousePos.x <= pos.x+500 && mousePos.y >= pos.y-24){
						displayClassifierGO.SetActive(true);
						if (classifierType == 0) displayClassifierGO.transform.GetChild(0).GetComponent<Text>().text = classifierList1[i+scrollAmount].getStringInfo();
						else displayClassifierGO.transform.GetChild(0).GetComponent<Text>().text = classifierList2[i+scrollAmount].getStringInfo();
						displayClassifierGO.GetComponent<Image>().color = go.GetComponent<Image>().color;
					}
				}
			}
		}else{
			displayClassifierGO.SetActive(false);
		}
    }
	
	void setClassifierStats(GameObject classifierStatsGO,Classifier1 classifier){
		if (classifier != null){
			string strDisp;
			strDisp = "";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.GUERRIER) > 0) strDisp += "GUERRIER ";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.VOLEUR) > 0) strDisp += "VOLEUR ";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.ARCHER) > 0) strDisp += "ARCHER ";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.MAGE) > 0) strDisp += "MAGE ";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.SOIGNEUR) > 0) strDisp += "SOIGNEUR ";
			if ((classifier.charClass & (byte)Classifier1Attributes.CharClass.ENVOUTEUR) > 0) strDisp += "ENVOUTEUR ";
			classifierStatsGO.transform.GetChild(0).GetComponent<Text>().text = strDisp;
			
			strDisp = "";
			if ((classifier.HP & (byte)Classifier1Attributes.HP_.BETWEEN_100_75) > 0) strDisp += "100-75% ";
			if ((classifier.HP & (byte)Classifier1Attributes.HP_.BETWEEN_74_40) > 0) strDisp += "74-40% ";
			if ((classifier.HP & (byte)Classifier1Attributes.HP_.BETWEEN_39_0) > 0) strDisp += "39-0% ";
			classifierStatsGO.transform.GetChild(1).GetComponent<Text>().text = strDisp;
			
			strDisp = "";
			if ((classifier.PA & (byte)Classifier1Attributes.PA_.ONE) > 0) strDisp += "ONE ";
			if ((classifier.PA & (byte)Classifier1Attributes.PA_.TWO_OR_MORE) > 0) strDisp += "TWO+ ";
			classifierStatsGO.transform.GetChild(2).GetComponent<Text>().text = strDisp;
			
			strDisp = "";
			if ((classifier.skillAvailable & (byte)Classifier1Attributes.SkillAvailable.YES) > 0) strDisp += "YES ";
			if ((classifier.skillAvailable & (byte)Classifier1Attributes.SkillAvailable.NO) > 0) strDisp += "NO ";
			classifierStatsGO.transform.GetChild(3).GetComponent<Text>().text = strDisp;
			
			strDisp = "";
			if ((classifier.threat & (byte)Classifier1Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
			if ((classifier.threat & (byte)Classifier1Attributes.Threat.DANGER) > 0) strDisp += "DANGER ";
			if ((classifier.threat & (byte)Classifier1Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
			classifierStatsGO.transform.GetChild(4).GetComponent<Text>().text = strDisp;
			
			strDisp = "";
			if ((classifier.maxTargets & (byte)Classifier1Attributes.MaxTargets.NONE) > 0) strDisp += "NONE ";
			if ((classifier.maxTargets & (byte)Classifier1Attributes.MaxTargets.ONE) > 0) strDisp += "ONE ";
			if ((classifier.maxTargets & (byte)Classifier1Attributes.MaxTargets.TWO) > 0) strDisp += "TWO ";
			if ((classifier.maxTargets & (byte)Classifier1Attributes.MaxTargets.THREE_OR_MORE) > 0) strDisp += "THREE+ ";
			classifierStatsGO.transform.GetChild(5).GetComponent<Text>().text = strDisp;

			classifierStatsGO.transform.GetChild(6).GetComponent<Text>().text = "" + classifier.infoAllies.Count;

			classifierStatsGO.transform.GetChild(7).GetComponent<Text>().text = "" + classifier.infoEnemies.Count;
			
			classifierStatsGO.transform.GetChild(8).GetComponent<Text>().text = "" + classifier.action;
			
			classifierStatsGO.transform.GetChild(9).GetComponent<Text>().text = "" + classifier.fitness + "," + classifier.useCount + "," + classifier.merges;
			if (classifier.fitness < 0.5f){
				classifierStatsGO.GetComponent<Image>().color = new Color(1.0f,(classifier.fitness)*2,(classifier.fitness)*2);
			}else{
				classifierStatsGO.GetComponent<Image>().color = new Color(1.0f-((classifier.fitness-0.5f)*2),1.0f,1.0f-((classifier.fitness-0.5f)*2));
			}
		}
	}
	
	void setClassifierStats(GameObject classifierStatsGO,Classifier2 classifier){
		if (classifier != null){
			classifierStatsGO.transform.GetChild(0).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(1).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(2).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(3).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(4).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(5).GetComponent<Text>().text = "";
			classifierStatsGO.transform.GetChild(6).GetComponent<Text>().text = "";
			
			
			classifierStatsGO.transform.GetChild(7).GetComponent<Text>().text = "" + classifier.lastUse;
			
			classifierStatsGO.transform.GetChild(8).GetComponent<Text>().text = "" + classifier.action;
			
			classifierStatsGO.transform.GetChild(9).GetComponent<Text>().text = "" + classifier.fitness + "," + classifier.useCount;
			if (classifier.fitness < 0.5f){
				classifierStatsGO.GetComponent<Image>().color = new Color(1.0f,(classifier.fitness)*2,(classifier.fitness)*2);
			}else{
				classifierStatsGO.GetComponent<Image>().color = new Color(1.0f-((classifier.fitness-0.5f)*2),1.0f,1.0f-((classifier.fitness-0.5f)*2));
			}
		}
	}
	
	// Events
	void buttonBackPressed_(){buttonBackPressed = true;}
}
