using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePanelController : MonoBehaviour
{
    [Header("To Link")]
    public Text timestampText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        long timestamp = ((System.DateTimeOffset) System.DateTime.Now).ToUnixTimeSeconds();
        string timestampString = timestamp.ToString();
        while (timestampString.Length < 10)
            timestampString = ("0" + timestampString);
        timestampText.text = timestampString;
    }
}
