using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebMenu.Data.Models;

namespace WebMenu.Data.Models
{
     public class User:IdentityUser
     {
          public virtual UserInfo UserInfo { get; set; }
     }
}
