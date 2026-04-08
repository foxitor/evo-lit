using UnityEngine; using System; using System.Linq; using System.Collections.Generic;
public class CreatureBrain : MonoBehaviour {
    //general
    [SerializeField]
    Classifications myClass;
    //energies
    [SerializeField] [Range(0,1)]
    float hunger = 1, hydration = 1, energy = 1, age;
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
                if (myClass != Classifications.plant) {
                    movement.wakeup();
                    Thouts.Remove("sleep");
                } else {
                    ToggleSleep();
                    Thouts.Remove("refill_plant");
                }
            } else {
                if (myClass != Classifications.plant) {
                    energy += (Time.deltaTime * 0.05f);
                    Snore();
                } else {
                    hydration += (Time.deltaTime * 0.05f);
                    energy += (Time.deltaTime * 0.03f);
                }
            }
        }
        if (hunger > 1) { hunger = 1; }
        if (hydration > 1) { hydration = 1; }
        if (energy > 1) { energy = 1; }
        if (age > 1) { age = 1; }
    }
    void AnalyseThouts() {
        if (!(myClass == Classifications.unlabled)) {
            string hiestThout = "";
            int pastHiest = 100;
            foreach (string thout in Thouts) {
                Goal thoutGoal = Array.Find(Goals, g => g != null && g.name == thout);
                    if (thoutGoal != null && thoutGoal.value < pastHiest) {
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
                case "grow":
                if (age < 1) {
                    age += 0.01f * Time.deltaTime;
                    hydration -= (Time.deltaTime * 0.05f);
                    energy -= (Time.deltaTime * 0.05f);
                }
                    break;
                case "refill_plant":
                if (age > 0.1) {
                    ToggleSleep();
                } else { Debug.Log("IM DEAD"); }
                    break;
            }

            if (energy < 0.25f && !Thouts.Contains("sleep") && !Thouts.Contains("refill_plant")) {
                if (myClass != Classifications.plant) {
                    Thouts.Add("sleep");
                } else {
                    Thouts.Add("refill_plant");
                }
            } 
            if (energy == 0) {
                Debug.Log("IM DEAD");
            }
        } else {
            if (Array.Find(Goals, g => g.name == "grow") != null) {
                myClass = Classifications.plant;
            } else if (Array.Find(Goals, g => g.name == "hunt") != null) {
                myClass = Classifications.preditor;
            } else {
                myClass = Classifications.prey;
            }
        }
    }
    public void VasteEnergy() { energy -= energyLoss; }
    public void ToggleSleep() { isSleeping = !isSleeping; }

    public void Snore() {
        if (UnityEngine.Random.Range(0, 128) == 17) {
            GameObject particle = Instantiate(Storage.requestObject("particle.snore"), transform.position, transform.rotation, transform);
            particle.GetComponent<Animator>().Play("snore"+UnityEngine.Random.Range(1,4));
            Destroy(particle, 0.75f);
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
public enum Classifications {
    unlabled,
    prey,
    preditor,
    plant
}