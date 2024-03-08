﻿using System.Collections.Generic;
using MelodyCircle.Models;

namespace MelodyCircle.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IList<string> Roles { get; set; }
    }
}