using EasyTextEffects;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ResourceSystem;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI suppliesAmount;
    [SerializeField] private TextMeshProUGUI peopleAmount;
    [SerializeField] private TextMeshProUGUI valuablesAmount;
    [SerializeField] private TextMeshProUGUI gearAmount;

    [SerializeField] private TextMeshProUGUI suppliesAmountChange;
    [SerializeField] private TextMeshProUGUI peopleAmountChange;
    [SerializeField] private TextMeshProUGUI valuablesAmountChange;
    [SerializeField] private TextMeshProUGUI gearAmountChange;
    void Start()
    {
        OnResourceChanged += UpdateText;
        OnOutOfResource += OutOfResource;
        OnResourceLimitReached += LimitReached;
        UpdateText(null, new OnResourceChangedArgs(ResourceType.None, 0));
    }

    private void UpdateText(object s, OnResourceChangedArgs args)
    {
        switch (args.Resource)
        {
            case ResourceType.Supplies:
                StartCoroutine(AnimateResourceChange(args, suppliesAmount, suppliesAmountChange));
                break;
            case ResourceType.People:
                StartCoroutine(AnimateResourceChange(args, peopleAmount, peopleAmountChange));
                break;
            case ResourceType.Valuables:
                StartCoroutine(AnimateResourceChange(args, valuablesAmount, valuablesAmountChange));
                break;
            case ResourceType.Gear:
                StartCoroutine(AnimateResourceChange(args, gearAmount, gearAmountChange));
                break;
            default:
                suppliesAmount.text = getResource(ResourceType.Supplies).ToString();
                peopleAmount.text = getResource(ResourceType.People).ToString();
                valuablesAmount.text = getResource(ResourceType.Valuables).ToString();
                gearAmount.text = getResource(ResourceType.Gear).ToString();
                break;
        }
    }

    private IEnumerator AnimateResourceChange(OnResourceChangedArgs args, TextMeshProUGUI resLabel, TextMeshProUGUI resLabelChange)
    {
        string prefix = "";
        if (args.AmountChanged != 0)
        {
            if (args.AmountChanged > 0)
            {
                resLabelChange.color = new Color(0, 1, 0, 0);
                prefix = "+";
            }
            else
            {
                resLabelChange.color = new Color(1, 0, 0, 0);
            }
            resLabelChange.text = prefix + args.AmountChanged.ToString();
            resLabelChange.GetComponent<TextEffect>().Refresh();
            resLabelChange.GetComponent<TextEffect>().StartManualEffect("ResourceChange");
        }

        int currentValue = 0;
        int.TryParse(resLabel.text, out currentValue);
        int targetValue = getResource(args.Resource);

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * (1/1.5f);
            resLabel.text = Mathf.RoundToInt(Mathf.Lerp(currentValue, targetValue, elapsedTime)).ToString();
            yield return null;
        }

        resLabel.text = getResource(args.Resource).ToString();
        yield return null;
    }

    private void OutOfResource(object s, OnOutOfResourceArgs args)
    {
        
    }

    private void LimitReached(object s, OnResourceLimitReachedArgs args)
    {
        
    }
}
