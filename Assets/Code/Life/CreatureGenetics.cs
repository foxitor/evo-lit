using UnityEngine; using System;

public class CreatureGenetics : MonoBehaviour {
    [TextArea(5, 15)]
    public string energyGenetics;
    [TextArea(5, 10)]
    public string thoutGenetics;
    //references
    CreatureBrain brain;

    void Start() {
        brain = gameObject.GetComponent<CreatureBrain>();
        applyGenetic();
    }
    public void applyGenetic() {
        string[] enGens = energyGenetics.Split(new char[] { '-' });
        foreach (string gen in enGens) {
            if (gen.StartsWith("br.")) {
                brain.geneticChange(gen);
            } else {Debug.Log(gen);}
        }
        string[] thoutGens = thoutGenetics.Split(new char[] { '-' });
        foreach (string gen in thoutGens) {
            if (gen.StartsWith("br.")) {
                brain.geneticChange(gen);
            } else {Debug.Log(gen);}
        }
    }
}
