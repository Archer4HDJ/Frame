using HDJ.Framework.Core.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New2System : ISystem
{
    protected override Type[] FilterComponentType()
    {
        return new Type[] { typeof(Test0Component), typeof(Test1Component),typeof(Test2Component), typeof(Test3Component) };
    }
}