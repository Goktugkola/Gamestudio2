using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TextMeshProUGUI AmmoLeftText;
    [SerializeField] private TMPro.TextMeshProUGUI AmmoMaxText;
    [SerializeField] private Shotgun shotgun;
    void Update()
    {
        AmmoLeftText.text = shotgun.ammoLeft.ToString();
        AmmoMaxText.text = shotgun.ammoMax.ToString();
    }
}
