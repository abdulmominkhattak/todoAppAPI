using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using toDoApp.Configuration;
using toDoApp.Models.DTOs.Requests;
using toDoApp.Models.DTOs.Responses;

namespace toDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthManagmentController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagmentController(UserManager<IdentityUser> userManager, 
            IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.email);
                if(existingUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {

                        Errors = new List<string>
                        {
                            "Bad Request"
                        },
                        Success = false 
                    }) ;
                  }

                var newUser = new IdentityUser()
                {
                    Email = user.email,
                    UserName = user.email
                }; 
                var isCreated = await _userManager.CreateAsync(newUser, user.password);
                if(isCreated.Succeeded)
                {
                    var jwtToken = GenerateJwtToken(newUser);

                    return Ok(new RegistrationResponse()
                    {
                        Success = true,
                        Token = jwtToken
                    }) ;


                }
                return BadRequest();
                
            }
            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>()
                {
                    "Invalid"
                },
                Success = false
            }); 

             
        }
        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.email);
                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>
                        {
                            "Invalid logins"
                        },
                        Success = false
                    });
                }

                    var isCorrect = await _userManager.CheckPasswordAsync(existingUser,user.password);
                    if(!isCorrect)
                    {
                        return BadRequest(new RegistrationResponse()
                        {
                            Errors = new List<string>
                        {
                            "Invalid logins"
                        },
                            Success = false
                        });

                    }
                    var jwtToken = GenerateJwtToken(existingUser);
                    return Ok(new RegistrationResponse()
                    {
                        Success = true,
                        Token = jwtToken
                    });


                }        
            
            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>
                {
                    "Invalid Payload "
                }
            });
        }

        private JwtConfig Get_jwtConfig()
        {
            return _jwtConfig;
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)



            };
            // Create Token bases on the Descriptor

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);


            return jwtToken;

        }

    }
}
