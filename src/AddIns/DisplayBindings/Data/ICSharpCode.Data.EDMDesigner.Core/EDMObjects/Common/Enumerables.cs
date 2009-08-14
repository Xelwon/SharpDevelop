﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common
{
    public static class Enumerables
    {
        public static T GetByName<T>(this IEnumerable<T> source, string name) where T : IEDMObjectBase
        {
            return source.FirstOrDefault(item => item.Name == name);
        }
    }
}
