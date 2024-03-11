using System.Collections.Generic;
using MelodyCircle.Data;
using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;

namespace MelodyCircle.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IList<string> Roles { get; set; }

    }
}