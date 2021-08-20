// IdentityService.cs
// Author:
//       michele <michele.tolve@gmail.com>
// Copyright (c) 2021 micheletolve
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AwesomeAPI.Authentication;
using AwesomeAPI.Authentication.Entities;
using AwesomeAPI.BusinessLayer.Settings;
using AwesomeAPI.Model.Contracts;
using AwesomeAPI.Model.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AwesomeAPI.BusinessLayer.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public IdentityService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        { 
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _signinManager = signInManager;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
        {
            var signInResult = await _signinManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (! signInResult.Succeeded)
            {
                return null;
            }

            var applicationUser = await _userManager.FindByNameAsync(request.UserName);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                new Claim(ClaimTypes.Name, applicationUser.UserName),
                new Claim(ClaimTypes.GivenName, applicationUser.FirstName),
                new Claim(ClaimTypes.Surname, applicationUser.LastName ?? string.Empty)                
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddDays(10), signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var response = new AuthenticationResponse
            {
                user = new User
                {
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    Email = applicationUser.Email
                },
                AccessToken = accessToken
            };

            return response;           
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            var response = new RegisterResponse
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            return response;
        }
    }
}