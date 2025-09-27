using UnityEngine;
using Core.Economy;
#if TMP_PRESENT || UNITY_TEXTMESHPRO
using TMPro;
#endif
using UnityEngine.UI;


public class ResourceBankDisplay : MonoBehaviour
{
    public ResourceBank bank;
    public string format = "ATP: {0:N0} CK: {1}";
#if TMP_PRESENT || UNITY_TEXTMESHPRO
TMP_Text _tmp;
#endif
    Text _text;


    void Awake()
    {
#if TMP_PRESENT || UNITY_TEXTMESHPRO
_tmp = GetComponent<TMP_Text>();
#endif
        _text = GetComponent<Text>();
        if (!bank) bank = FindAnyObjectByType<ResourceBank>();
        if (bank) bank.OnChanged += Refresh;
    }
    void OnDestroy() { if (bank) bank.OnChanged -= Refresh; }


    void OnEnable() { Refresh(); }
    void Refresh()
    {
        if (!bank) return;
        string s = string.Format(format, bank.atp, bank.cytokines);
#if TMP_PRESENT || UNITY_TEXTMESHPRO
if (_tmp) { _tmp.text = s; return; }
#endif
        if (_text) _text.text = s;
    }
}