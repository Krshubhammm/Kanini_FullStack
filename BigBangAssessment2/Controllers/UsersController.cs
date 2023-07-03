using BigBangAssessment2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BigBangAssessment2.Helpers;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BigBangAssessment2.Models.Dto;
using PasswordHasher = BigBangAssessment2.Helpers.PasswordHasher;

namespace BigBangAssessment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /* private readonly ApplicationDbContext _authContext;
         public UserController(ApplicationDbContext context)
         {
             _authContext = context;
         }
         [HttpPost("authenticate")]
         public async Task<IActionResult> Authenticate([FromBody] User userObj)
         {
             if (userObj == null)
                 return BadRequest();

             var user = await _authContext.Users
                 .FirstOrDefaultAsync(x => x.Username == userObj.Username);

             if (user == null)
                 return NotFound(new { Message = "User not found!" });

             if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
             {
                 return BadRequest(new { Message = "Password is Incorrect" });
             }

             user.RefreshToken = CreateJwt(user);
             var newAccessToken = user.RefreshToken;
             var newRefreshToken = CreateRefreshToken();
             user.RefreshToken = newRefreshToken;
             user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
             await _authContext.SaveChangesAsync();

             return Ok(new TokenApiDto()
             {
                 AccessToken = newAccessToken,
                 RefreshToken = newRefreshToken
             });
         }

         [HttpPost("register")]
         public async Task<IActionResult> AddUser([FromBody] User userObj)
         {
             if (userObj == null)
                 return BadRequest();

             // check email
             if (await CheckEmailExistAsync(userObj.Email))
                 return BadRequest(new { Message = "Email Already Exist" });

             //check username
             if (await CheckUsernameExistAsync(userObj.Username))
                 return BadRequest(new { Message = "Username Already Exist" });

             var passMessage = CheckPasswordStrength(userObj.Password);
             if (!string.IsNullOrEmpty(passMessage))
                 return BadRequest(new { Message = passMessage.ToString() });

             userObj.Password = PasswordHasher.HashPassword(userObj.Password);
             userObj.Role = "User";
             if (!string.IsNullOrEmpty(userObj.Role) && userObj.Role.ToLower() == "admin")
             {
                 // Here, you can add additional logic to determine if the user registering with the "Admin" role is allowed.
                 // For simplicity, we'll allow anyone to register with the "Admin" role in this example.
                 userObj.Role = "Admin";
             }
             userObj.RefreshToken = "";
             await _authContext.AddAsync(userObj);
             await _authContext.SaveChangesAsync();
             return Ok(new
             {
                 Status = 200,
                 Message = "User Added!"
             });
         }

         private Task<bool> CheckEmailExistAsync(string? email)
             => _authContext.Users.AnyAsync(x => x.Email == email);

         private Task<bool> CheckUsernameExistAsync(string? username)
             => _authContext.Users.AnyAsync(x => x.Email == username);

         private static string CheckPasswordStrength(string pass)
         {
             StringBuilder sb = new StringBuilder();
             if (pass.Length < 9)
                 sb.Append("Minimum password length should be 8" + Environment.NewLine);
             if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                 sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
             if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                 sb.Append("Password should contain special charcter" + Environment.NewLine);
             return sb.ToString();
         }

         private string CreateJwt(User user)
         {
             var jwtTokenHandler = new JwtSecurityTokenHandler();
             var key = Encoding.ASCII.GetBytes("veryverysceret.....");
             var identity = new ClaimsIdentity(new Claim[]
             {
                 new Claim(ClaimTypes.Role, user.Role),
                 new Claim(ClaimTypes.Name,user.Username)
             });

             var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

             var tokenDescriptor = new SecurityTokenDescriptor
             {
                 Subject = identity,
                 Expires = DateTime.Now.AddSeconds(10),
                 SigningCredentials = credentials
             };
             var token = jwtTokenHandler.CreateToken(tokenDescriptor);
             return jwtTokenHandler.WriteToken(token);
         }

         private string CreateRefreshToken()
         {
             var tokenBytes = RandomNumberGenerator.GetBytes(64);
             var refreshToken = Convert.ToBase64String(tokenBytes);

             var tokenInUser = _authContext.Users
                 .Any(a => a.RefreshToken == refreshToken);
             if (tokenInUser)
             {
                 return CreateRefreshToken();
             }
             return refreshToken;
         }

         private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
         {
             var key = Encoding.ASCII.GetBytes("veryverysceret.....");
             var tokenValidationParameters = new TokenValidationParameters
             {
                 ValidateAudience = false,
                 ValidateIssuer = false,
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(key),
                 ValidateLifetime = false
             };
             var tokenHandler = new JwtSecurityTokenHandler();
             SecurityToken securityToken;
             var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
             var jwtSecurityToken = securityToken as JwtSecurityToken;
             if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                 throw new SecurityTokenException("This is Invalid Token");
             return principal;

         }

        // [Authorize]
         [HttpGet]
         public async Task<ActionResult<User>> GetAllUsers()
         {
             return Ok(await _authContext.Users.ToListAsync());
         }

         [HttpPost("refresh")]
         public async Task<IActionResult> Refresh([FromBody] TokenApiDto tokenApiDto)
         {
             if (tokenApiDto is null)
                 return BadRequest("Invalid Client Request");
             string accessToken = tokenApiDto.AccessToken;
             string refreshToken = tokenApiDto.RefreshToken;
             var principal = GetPrincipleFromExpiredToken(accessToken);
             var username = principal.Identity.Name;
             var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Username == username);
             if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                 return BadRequest("Invalid Request");
             var newAccessToken = CreateJwt(user);
             var newRefreshToken = CreateRefreshToken();
             user.RefreshToken = newRefreshToken;
             await _authContext.SaveChangesAsync();
             return Ok(new TokenApiDto()
             {
                 AccessToken = newAccessToken,
                 RefreshToken = newRefreshToken,
             });
         }*/
        private readonly ApplicationDbContext mdbc;
        private readonly IConfiguration configuration;

        public UserController(ApplicationDbContext m, IConfiguration ic)
        {
            this.mdbc = m;
            this.configuration = ic;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = await mdbc.Users.ToListAsync();
            return Ok(user);
        }
        [HttpPost("register")]
        public async Task<ActionResult> AddUser(User r)
        {
            var users = mdbc.Users.FirstOrDefault(x => x.Username == r.Username);
            if (users == null)
            {
                mdbc.Users.Add(r);
                mdbc.SaveChanges();
                return Ok(r);
            }
            return BadRequest(new { error = "UserName Already Exists" });

        }
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(Login l)
        {
            var user = await mdbc.Users.FirstOrDefaultAsync(x => x.Username == l.Username);
            if (user != null)
            {
                if (user.Password == l.Password)
                {
                    var token = CreatToken(user);
                    return Ok(new { token, user });
                }
            }
            return NotFound("User Not Found With Provided Detail");

        }
        private string CreatToken(User ud)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ud.Username),
                new Claim(ClaimTypes.Role,ud.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSetting:Token").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }

}

