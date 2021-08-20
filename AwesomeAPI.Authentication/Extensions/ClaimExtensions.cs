// ClaimExtensions.cs
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
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace AwesomeAPI.Authentication.Extensions
{
    public static class ClaimExtensions
    {
        public static Guid GetId(this IPrincipal user)
        {
            string value = GetClaimValue(user, ClaimTypes.NameIdentifier);
            return Guid.Parse(value);
        }

        public static string GetFirstName(this IPrincipal user)
            => GetClaimValue(user, ClaimTypes.GivenName);

        public static string GetLastName(this IPrincipal user)
            => GetClaimValue(user, ClaimTypes.Surname);

        public static string GetEmail(this IPrincipal user)
            => GetClaimValue(user, ClaimTypes.Email);

        public static int GetApplicationId(this IPrincipal user)
        {
            string value = GetClaimValue(user, CustomClaimTypes.ApplicationId);
            return int.Parse(value);
        }

        public static IEnumerable<string> GetRoles(this IPrincipal user)
        {
            var values = ((ClaimsPrincipal)user).FindAll(ClaimTypes.Role).Select(x => x.Value);
            return values;
        }

        private static string GetClaimValue(IPrincipal user, string claimType)
        {
            string value = ((ClaimsPrincipal)user).FindFirst(claimType)?.Value;
            return value;
        }
    }
}
