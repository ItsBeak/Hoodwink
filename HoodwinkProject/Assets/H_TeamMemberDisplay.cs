using UnityEngine;
using TMPro;
using UnityEngine.Rendering.HighDefinition;
using Unity.VisualScripting;

public class H_TeamMemberDisplay : MonoBehaviour
{

    public TextMeshProUGUI memberName;
    public TextMeshProUGUI memberDescription;

    public HDAdditionalLightData overheadLightLeft, overheadLightRight;
    public float litBrightness, dimBrightness;

    public float lightDelay = 3f;
    float delayTimer;


    private void Start()
    {
        memberName.text = "";
        memberDescription.text = "";

    }
    private void Update()
    {
        if (memberName.text != "")
        {
            delayTimer = lightDelay;
        }
        else
        {
            delayTimer -= Time.deltaTime;
        }

        overheadLightLeft.intensity = Mathf.Lerp(overheadLightLeft.intensity, delayTimer < 0 ? litBrightness : dimBrightness, Time.deltaTime);
        overheadLightRight.intensity = Mathf.Lerp(overheadLightRight.intensity, delayTimer < 0 ? litBrightness : dimBrightness, Time.deltaTime);
    }

}
