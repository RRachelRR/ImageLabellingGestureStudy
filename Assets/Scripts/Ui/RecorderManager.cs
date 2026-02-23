using UnityEngine;

public class RecorderManager : MonoBehaviour
{
    public GameObject recorderUi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (recorderUi != null)
        {
            recorderUi.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOn()
    {
        if (recorderUi != null)
        {
            recorderUi.SetActive(true);
        }
    }
    
    public void SetOff()
    {
        if (recorderUi != null)
        {
            recorderUi.SetActive(false);
        }
    }
}
