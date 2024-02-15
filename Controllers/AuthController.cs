global using dotnet_rpg.DTOs.User;
using dotnet_rpg.Data;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {

        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo){
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request){
            
            var response = await _authRepo.Register(new User {Username=request.Username}, request.Password);

            if(!response.Success)
                return BadRequest(response);
            else
                return Ok(response);

        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto loginRequest){
            var response = await _authRepo.Login(loginRequest.Username, loginRequest.Password);
            if(!response.Success){
                return BadRequest(response);
            }
            else{
                return Ok(response);
            }
        }
    }
}