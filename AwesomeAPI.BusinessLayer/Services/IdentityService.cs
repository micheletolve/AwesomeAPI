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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AwesomeAPI.Authentication;
using AwesomeAPI.Authentication.Entities;
using AwesomeAPI.Authentication.Extensions;
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
            if (!signInResult.Succeeded)
            {
                return null;
            }

            var dbUser = await _userManager.FindByNameAsync(request.UserName);
            var userRoles = await _userManager.GetRolesAsync(dbUser);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
                new Claim(ClaimTypes.Name, dbUser.UserName),
                new Claim(ClaimTypes.GivenName, dbUser.FirstName),
                new Claim(ClaimTypes.Surname, dbUser.LastName ?? string.Empty)
            }.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authResponse = CreateToken(claims);

            dbUser.RefreshToken = authResponse.RefreshToken;
            dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes);

            await _userManager.UpdateAsync(dbUser);

            return authResponse;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var applicationUser = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(applicationUser, request.Password);
            if (result.Succeeded)
            {
                // Assign a default role "user"
                result = await _userManager.AddToRoleAsync(applicationUser, RoleNames.User);
            }
            var response = new RegisterResponse
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            return response;
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var applicationUser = ValidateToken(request.AccessToken);
            if (applicationUser is null)
            {
                return null;
            }

            var userId = applicationUser.GetId();
            var dbUser = await _userManager.FindByIdAsync(userId.ToString());
            if (dbUser?.RefreshToken is null || dbUser?.RefreshTokenExpirationDate < DateTime.UtcNow || dbUser?.RefreshToken != request.RefreshToken)
            {
                return null;
            }

            var authResponse = CreateToken(applicationUser.Claims);
            dbUser.RefreshToken = authResponse.RefreshToken;
            dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes);

            await _userManager.UpdateAsync(dbUser);
            return authResponse;
        }
        private AuthenticationResponse CreateToken(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes), signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var response = new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = GenerateRefreshToken(_jwtSettings.RefreshTokenByteLength)
            };

            return response;
        }

        private ClaimsPrincipal ValidateToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey)),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var applicationUser = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
                if (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg == SecurityAlgorithms.HmacSha256)
                {
                    return applicationUser;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return null;
        }

        private static string GenerateRefreshToken(int byteLength)
        {
            byte[] random = new byte[byteLength];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(random);

            return Convert.ToBase64String(random);
        }
    }
}