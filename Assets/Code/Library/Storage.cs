using UnityEngine;

public class Storage : MonoBehaviour {
    [SerializeField]
    GameObject[] Prefabs;
    [SerializeField]
    GameObject[] ParticleStorage;

    public GameObject requestObject(string reason) {
        GameObject result = null;
        if (reason.ToLower().StartsWith("particle.")) {
            result = TranslateParticle(reason);
        }
        return result;
    }
    GameObject TranslateParticle(string name) {
        GameObject result;
        string cutted = name.Replace("particle.", "");
        switch (cutted) {
            case "snore": result = shareParticlePackage(1); break;
            default : result = shareParticlePackage(0); break;
        }
        return result;
    }
    GameObject shareParticlePackage(int indx) { return ParticleStorage[indx]; }
    GameObject sharePrefabPackage(int indx) { return Prefabs[indx]; }
}
