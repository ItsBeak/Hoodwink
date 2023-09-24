using UnityEngine;
using Mirror;
using TMPro;

public class H_CodeKey : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnKeyChanged))] private string key = "000000";
    [SerializeField] private TextMeshPro keyDisplayText;

    public override void OnStartServer()
    {
        GenerateKey();
    }

    private void GenerateKey()
    {
        key = GenerateRandomKey();
    }

    private string GenerateRandomKey()
    {
        string newKey = "";
        for (int i = 0; i < 6; i++)
        {
            newKey += Random.Range(0, 10).ToString();
        }
        return newKey;
    }

    private void OnKeyChanged(string oldKey, string newKey)
    {
        keyDisplayText.text = newKey;
    }
}
