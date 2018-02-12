﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EAS.Support.Web.Models
{
    public class DeclarationViewModel
    {
        public string SubmissionDate { get; set; }
        public string PayrollDate { get; set; }
        public string LevySubmissionId { get; set; }
        public string LevyDeclarationDescription { get; set; }
        public string YearToDateAmount { get; set; }

        public string RowClass { get; set; }
    }
}