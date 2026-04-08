using UnityEngine; using System; using System.Linq; using System.Collections.Generic;
public class CreatureBrain : MonoBehaviour {
    //energies
    [SerializeField] [Range(0,1)]
    float hunger = 1, hydration = 1, energy = 1;
    float hungerLoss = 0, hydrationLoss = 0, energyLoss = 0;
    bool isSleeping = false;

    //Thouts
    [SerializeField]
    List<string> Thouts;
    [SerializeField]
    Goal[] Goals;

    CreatureMovement movement;
    Storage Storage;

    void Start() {
        movement = gameObject.GetComponent<CreatureMovement>();
        Storage = GameObject.Find("stored").GetComponent<Storage>();
    }
    void Update() {
        if (!isSleeping) {
            AnalyseThouts();
        } else {
            if (energy > 0.75f) {
                movement.wakeup();
                Thouts.Remove("sleep");
            } else {
                energy += (Time.deltaTime * 0.05f);
                Snore();
            }
        }
    }
    void AnalyseThouts() {
        string hiestThout = "";
        int pastHiest = 100;
        foreach (string thout in Thouts) {
            Goal thoutGoal = Array.Find(Goals, g => g.name == thout);
            if (thoutGoal.value < pastHiest) {
                hiestThout = thout;
                pastHiest = thoutGoal.value;
            }
        }
        switch (hiestThout) {
            case "wonder":
            movement.setMovementMethod(MovementMethods.wonder);
                break;
            case "sleep":
            movement.sleep();
                break;
        }

        if (energy < 0.25f && !Thouts.Contains("sleep")) {
            Thouts.Add("sleep");
        }
    }
    public void VasteEnergy() { energy -= energyLoss; }
    public void ToggleSleep() { isSleeping = !isSleeping; }

    public void Snore() {
        if (energy % 2 == 0) {
            Instantiate(Storage.requestObject("particle.snore"), transform.position, transform.rotation, transform);
        }
    }

    public void geneticChange(string input) {
        //Hydration
        if (input.StartsWith("br.hy.s(") && input.EndsWith(")")) {
            int startIdx = input.IndexOf("(") + 1;
            int endIdx = input.IndexOf(")");
            string value = input.Substring(startIdx, endIdx - startIdx);
            float result;
            if (float.TryParse(value, out result)) { hydrationLoss = result; } 
            else { Debug.LogError("'" + value + "' is not a float"); }
        } 
        //Hunger
        else if (input.StartsWith("br.hu.s(") && input.EndsWith(")")) {
            int startIdx = input.IndexOf("(") + 1;
            int endIdx = input.IndexOf(")");
            string value = input.Substring(startIdx, endIdx - startIdx);
            float result;
            if (float.TryParse(value, out result)) { hungerLoss = result; } 
            else { Debug.LogError("'" + value + "' is not a float"); }
        } 
        //Energy
        else if (input.StartsWith("br.en.s(") && input.EndsWith(")")) {
            int startIdx = input.IndexOf("(") + 1;
            int endIdx = input.IndexOf(")");
            string value = input.Substring(startIdx, endIdx - startIdx);
            float result;
            if (float.TryParse(value, out result)) { energyLoss = result; } 
            else { Debug.LogError("'" + value + "' is not a float"); }
        }
        //Thout
        else if (input.StartsWith("br.thou(") && input.EndsWith(")")) {
            int startIdx = input.IndexOf("(") + 1;
            int endIdx = input.IndexOf(")");
            string data = input.Substring(startIdx, endIdx - startIdx);
            string[] dataComponents = data.Split(new char[] { ',' });
            Goal targetGoal = Goals[0];
            foreach(Goal g in Goals) {
                if (g.name == dataComponents[0]) {
                    targetGoal = g;
                    //Debug.Log("Succses at " + targetGoal.name);
                }
            }
            int result;
            if (int.TryParse(dataComponents[1], out result)) { targetGoal.value = result; } 
            else { Debug.LogError("'" + dataComponents[1] + "' is not a float"); }
        }
    } 
}

[System.Serializable]
public class Goal {
    public string name;
    public int value;
}