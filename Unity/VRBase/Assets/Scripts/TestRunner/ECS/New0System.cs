using HDJ.Framework.Core.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class New0System : ISystem
{
    protected override Type[] FilterComponentType()
    {
        return new Type[] {typeof(Test0Component), typeof(Test1Component) };
    }
}
