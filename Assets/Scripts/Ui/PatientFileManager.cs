using UnityEngine;

public class PatientFileManager : MonoBehaviour
{
    public GameObject patientFileUi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (patientFileUi != null)
        {
            patientFileUi.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOn()
    {
        if (patientFileUi != null)
        {
            patientFileUi.SetActive(true);
        }
    }
    
    public void SetOff()
    {
        if (patientFileUi != null)
        {
            patientFileUi.SetActive(false);
        }
    }
}
