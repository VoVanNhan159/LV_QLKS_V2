﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LV_QLKS.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("google-login")]
        public async Task<ActionResult> Google()
        {
            var pro = new AuthenticationProperties
            {
                RedirectUri = "/createwithgoogle"
            };
            return Challenge(pro, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("facebook-login")]
        public async Task<ActionResult> Facebook()
        {
            var pro = new AuthenticationProperties
            {
                RedirectUri = "/createwithfacebook"
            };
            return Challenge(pro, FacebookDefaults.AuthenticationScheme);
        }
    }
}
