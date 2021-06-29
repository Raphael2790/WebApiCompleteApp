﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.WebApi.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenDTO UserToken { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class UserTokenDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimDTO> Claims { get; set; }
    }

    public class ClaimDTO
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
