﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Enums
{
    [Flags]
    public enum Level
    {
        None = 0,
        [Display(Name = "Level 2 - GCSE")]
        Level2 = 1,
        [Display(Name = "Level 3 - A level")]
        Level3 = 2,
        [Display(Name = "Level 4 - higher national cerificate (HNC)")]
        Level4 = 4,
        [Display(Name = "Level 5 - higher national diploma (HND)")]
        Level5 = 8,
        [Display(Name = "Level 6 - degree")]
        Level6 = 16,
        [Display(Name = "Level 7 - master’s degree")]
        Level7 = 32,
    }
}