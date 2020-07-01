﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Models.DTOs
{
    public class UserForRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
        public string Email { get; set; }

    }
}
