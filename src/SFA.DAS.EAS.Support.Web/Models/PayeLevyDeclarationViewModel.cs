﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Support.Web.Models
{
    public class PayeSchemeLevyDeclarationViewModel
    {
        public string PayeSchemeName { get; set; }
        public string PayeSchemeFormatedAddedDate { get; set; }
        public string PayeSchemeRef { get; set; }
        public List<DeclarationViewModel> LevyDeclarations { get; set; }

    }
}