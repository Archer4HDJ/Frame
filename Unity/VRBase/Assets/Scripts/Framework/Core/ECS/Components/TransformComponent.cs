using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDJ.Framework.Core.ECS
{
   public class TransformComponent : IComponent
    {
        public Vector position = Vector.Zero;

        public Vector rotation = Vector.Zero;

        public Vector scale = Vector.Zero;
    }
}
