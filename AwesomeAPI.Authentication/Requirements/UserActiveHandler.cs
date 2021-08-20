// UserActiveHandler.cs
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
using System.Threading.Tasks;
using AwesomeAPI.Authentication.Entities;
using AwesomeAPI.Authentication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AwesomeAPI.Authentication.Requirements
{
    public class UserActiveHandler : AuthorizationHandler<UserActiveRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserActiveHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserActiveRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                Guid userId = context.User.GetId();
                ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
                if (user is not null && user.LockoutEnd.GetValueOrDefault() <=  System.DateTimeOffset.UtcNow)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
