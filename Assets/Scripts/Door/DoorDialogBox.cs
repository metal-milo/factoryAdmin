using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highligthedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject faultSelector;
    [SerializeField] GameObject faultDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> faultTexts;

    [SerializeField] Text textOne;
    [SerializeField] Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableFaultSelector(bool enabled)
    {
        faultSelector.SetActive(enabled);
        //faultDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i=0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highligthedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void SetFaultNames(List<Option> faults)
    {
        for (int i = 0; i < faultTexts.Count; i++)
        {
            if (i < faults.Count)
            {
                faultTexts[i].text = faults[i].Base.Name;
            }
            else
            {
                faultTexts[i].text = "-";
            }
        }
    }
}
