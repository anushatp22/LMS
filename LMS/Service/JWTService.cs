using LMS.DTOs.Employee;
using LMS.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LMS.Service
{
    public class JWTService : IJWTService
    {
        private readonly JWTResponse _jwtSettings;

        public JWTService(IOptions<JWTResponse> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<string> GenerateToken(LoginConfirmJWTResponse employeeData, LoginRequest? loginRequest = null)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, employeeData.EmployeeId),
            new Claim(JwtRegisteredClaimNames.Email, employeeData.Email),
            new Claim(JwtRegisteredClaimNames.Name, employeeData.Name),
            new Claim("Role", employeeData.Role),
            new Claim("Department", employeeData.Department),
            new Claim("CompanyId", employeeData.CompanyId.ToString()),
            new Claim("Id", employeeData.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                //expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                //expires: DateTime.UtcNow.AddMinutes(3),
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return await Task.FromResult(tokenString);
        }

        //public RefreshToken GenerateRefreshToken(string? userId = null)
        //{
        //    var randomBytes = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomBytes);
        //    }

        //    return  new RefreshToken
        //    {
        //        Token = Convert.ToBase64String(randomBytes),
        //        Expires = DateTime.UtcNow.AddDays(7), // valid for 7 days
        //        Created = DateTime.UtcNow
        //    };
        //}

    }
}
