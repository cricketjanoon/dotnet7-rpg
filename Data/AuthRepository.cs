using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using dotnet_rpg.Models;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data {
    public class AuthRepository : IAuthRepository {

        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthRepository(DataContext context, IConfiguration configuration){
            _context = context;
            _config = configuration;
        }
            
        public async Task<ServiceResponse<string>> Login(string username, string password) {
            
            var serviceResponse = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower()==username.ToLower());

            if(user is null){
                serviceResponse.Success = false;
                serviceResponse.Message = "User not found.";
            }
            else if (!VeriftyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                serviceResponse.Success = false;
                serviceResponse.Message = "Wrong password.";
            }
            else{
                serviceResponse.Data = CreateToken(user);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password) {

            var serviceResponse = new ServiceResponse<int>();

            if(await UserExisits(user.Username)){
                serviceResponse.Success=false;
                serviceResponse.Message="User already exists";
                return serviceResponse;
            }

            byte[] passwordhash;
            byte[] passwordSalt;
            CreatePasswordHash(password, out passwordhash, out passwordSalt);

            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordSalt;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            serviceResponse.Data = user.Id;
            return serviceResponse;
        }

        public async Task<bool> UserExisits(string username) {
            
            if(await _context.Users.AnyAsync(u => u.Username.ToLower()==username.ToLower())){
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt){
            using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VeriftyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt){
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user){
            
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingToken = _config.GetSection("AppSettings:Token").Value;
            if(appSettingToken is null)
                throw new Exception("AppSettings Token is null.");

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingToken));
            SigningCredentials creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
 
            return tokenHandler.WriteToken(token);
        }
    }
}