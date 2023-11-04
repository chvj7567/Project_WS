using System.Threading.Tasks;
using UnityEngine;

public class UIStartArg : CHUIArg
{
    
}

public class UIStart : UIBase
{
    UIStartArg arg;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIStartArg;
    }

    private void Start()
    {

    }
}
