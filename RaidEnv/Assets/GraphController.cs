using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    public Text Title;
    public Image ProgressBar;
    public Text Val;
    public Text Val2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTitle(string text) {
        Title.text = text;
    }

    public void SetProgressRate(float val) {
        ProgressBar.fillAmount = val;
    }

    public void SetVal(int val) {
        Val.text = val.ToString();
        if(Val2 != null) {
            Val2.text = val.ToString();
        }
    }


}
