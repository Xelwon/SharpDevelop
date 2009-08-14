﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class CUDFunctionAssociationMapping
    {
        public CSDL.Association.Association Association { get; set; }
        public string FromRole { get; set; }
        public string ToRole { get; set; }

        private CUDFunctionParametersMapping _associationPropertiesMapping;
        public CUDFunctionParametersMapping AssociationPropertiesMapping
        {
            get
            {
                if (_associationPropertiesMapping == null)
                    _associationPropertiesMapping = new CUDFunctionParametersMapping();
                return _associationPropertiesMapping;
            }
        }
    }
}
