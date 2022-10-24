using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : Clickable
{
    public override void Operate()
    {
        if(Managers.Session.IsSeekMode())
        {
            Destroy(this.gameObject);
        }
    }
}
