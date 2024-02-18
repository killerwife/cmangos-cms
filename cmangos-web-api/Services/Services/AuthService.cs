using cmangos_web_api.Configs;
using cmangos_web_api.Repositories;
using Data.Dto;
using Data.Dto.Srp6;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using OtpNet;
using Data.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common;
using cmangos_web_api.Auth;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IOptionsMonitor<AuthConfig> _options;
        private IAccountRepository _accountRepository;
        private IUserProvider _userProvider;
        private IEmailService _emailService;

        private static readonly RandomNumberGenerator rngCsp = RandomNumberGenerator.Create();

        // algorithm constants
        private static readonly BigInteger g = 7;
        // trailing 0 to force unsigned
        private static readonly BigInteger N = BigInteger.Parse("0894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", NumberStyles.AllowHexSpecifier);

        public AuthService(IOptionsMonitor<AuthConfig> options, IAccountRepository accountRepository, IUserProvider userProvider)
        {
            _options = options;
            _accountRepository = accountRepository;
            _userProvider = userProvider;
        }

        public async Task<ChallengeResponseDto> GenerateChallenge(string userName)
        {
            var account = await _accountRepository.FindByUsername(userName);
            return new ChallengeResponseDto
            {

            };
        }

        public async Task<AuthResDto> ValidateProof(byte[] answer, byte[] m1, byte[] crcHash, byte[] pin)
        {
            return new AuthResDto
            {

            };
        }

        public async Task<AuthResDto> ValidatePlainLogin(string name, string password, string? pin)
        {
            var account = await _accountRepository.FindByUsername(name);

            var data = GetSRP6RegistrationData(name, password);

            // When logging in
            // verify login
            bool validLoginInfo = VerifySRP6Login(name, password, data.Salt, data.Verifier);
            if (account == null)
                return new AuthResDto
                {
                    Errors = new List<string>() { "Invalid credentials" }
                };

            var salt = BigInteger.Parse(account.s, NumberStyles.AllowHexSpecifier);
            var verifier = BigInteger.Parse(account.v, NumberStyles.AllowHexSpecifier);
            var saltHex = salt.ToString("X");
            var verifierHex = verifier.ToString("X");
            if (!VerifySRP6Login(name, password, salt.ToByteArray(), verifier.ToByteArray()))
                return new AuthResDto
                {
                    Errors = new List<string>() { "Invalid credentials" }
                };

            if (!string.IsNullOrEmpty(account.token))
            {
                if (pin == null)
                    return new AuthResDto
                    {
                        Errors = new List<string>() { "Invalid credentials" }
                    };
                var result = validateToken(account.token, pin);
                if (result == false)
                    return new AuthResDto
                    {
                        Errors = new List<string>() { "Invalid credentials" }
                    };
            }

            var claims = GetClaims(account);
            var newRefreshToken = generateRefreshToken("");
            await _accountRepository.RevokeAndAddToken(newRefreshToken);
            return new AuthResDto
            {
                JwtToken = CreateToken(claims),
                RefreshToken = newRefreshToken.Token
            };
        }

        public (byte[] Salt, byte[] Verifier) GetSRP6RegistrationData(string username, string password)
        {
            // generate a random salt
            byte[] salt = new byte[32];
            rngCsp.GetBytes(salt);

            // calculate verifier using this salt
            byte[] verifier = CalculateSRP6Verifier(username, password, salt);

            // done - this is what you put in the account table!
            return (salt, verifier);
        }

        public byte[] CalculateSRP6Verifier(string username, string password, byte[] salt_bytes)
        {
            SHA1 sha1 = SHA1.Create();
            sha1.Initialize();
            var userNameBytes = Encoding.ASCII.GetBytes(username);
            sha1.TransformBlock(userNameBytes, 0, userNameBytes.Length, null, 0);
            var delimiterBytes = Encoding.ASCII.GetBytes(":");
            sha1.TransformBlock(delimiterBytes, 0, delimiterBytes.Length, null, 0);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            sha1.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
            var loginBytes = sha1.Hash;

            // calculate first hash
            byte[] login_bytes2 = Encoding.UTF8.GetBytes((username + ':' + password).ToUpper());
            sha1.Initialize();
            sha1.TransformBlock(salt_bytes, 0, salt_bytes.Length, null, 0);
            sha1.TransformFinalBlock(loginBytes!, 0, loginBytes!.Length);
            var finalDigest = sha1.Hash;

            // convert to integer (little-endian)
            BigInteger h2 = new BigInteger(finalDigest, true);

            // g^h2 mod N
            BigInteger verifier = BigInteger.ModPow(g, h2, N);

            // convert back to a byte array (little-endian)
            byte[] verifier_bytes = verifier.ToByteArray();

            // pad to 32 bytes, remember that zeros go on the end in little-endian!
            byte[] verifier_bytes_padded = new byte[Math.Max(32, verifier_bytes.Length)];
            Buffer.BlockCopy(verifier_bytes, 0, verifier_bytes_padded, 0, verifier_bytes.Length);

            // done!
            return verifier_bytes_padded;
        }

        public bool VerifySRP6Login(string username, string password, byte[] salt, byte[] verifier)
        {
            // re-calculate the verifier using the provided username + password and the stored salt
            byte[] checkVerifier = CalculateSRP6Verifier(username.ToUpper(), password.ToUpper(), salt);

            // compare it against the stored verifier
            return verifier.SequenceEqual(checkVerifier);
        }

        private uint generateToken(string b32Key)
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            long timestamp = dto.ToUnixTimeSeconds() / 30;
            byte[] challenge = new byte[8];

            for (int i = 8; i > 0; timestamp >>= 8)
            {
                i--;
                challenge[i] = (byte)timestamp;
            }

            var key = Base32Encoding.ToBytes(b32Key);
            var hmac = new HMACSHA1(key!);
            var hmac_result = hmac.ComputeHash(challenge);
            int offset = hmac_result[19] & 0xF;
            uint truncHash = (uint)(hmac_result[offset] << 24) | (uint)(hmac_result[offset + 1] << 16) | (uint)(hmac_result[offset + 2] << 8) | (uint)(hmac_result[offset + 3]);
            truncHash &= 0x7FFFFFFF;

            return truncHash % 1000000u;
        }

        public string CreateToken(List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            RSAParameters rsaParams;
            using (var rsaProvider = new RSACryptoServiceProvider(Constants.RsaKeyLength))
            {
                rsaProvider.ImportFromPem(_options.CurrentValue.JwtPrivate.ToCharArray());
                rsaParams = rsaProvider.ExportParameters(true);
                // use the RSAParameters here
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(_options.CurrentValue.JwtExpirationSeconds),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsaParams), SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IEnumerable<Claim> DecodeToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            //RSAParameters rsaParams;
            //using (var rsaProvider = new RSACryptoServiceProvider(Constants.RsaKeyLength))
            //{
            //    rsaProvider.ImportFromPem(_options.CurrentValue.JwtPublic.ToCharArray());
            //    rsaParams = rsaProvider.ExportParameters(false);
            //    // use the RSAParameters here
            //}
            //tokenHandler.ValidateToken(token, new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new RsaSecurityKey(rsaParams),
            //    ValidateIssuer = false,
            //    ValidateAudience = false,
            //    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            //    ClockSkew = TimeSpan.Zero
            //}, out SecurityToken validatedToken);

            return token.Claims;
        }

        public async Task<AuthResDto?> RefreshToken(string reqRefreshToken, string ipAddress)
        {
            Account user = await _accountRepository.FindByToken(reqRefreshToken);
            if (user == null) return null;

            var refreshToken = (await _accountRepository.GetTokens(user.id)).Single(x => x.Token == reqRefreshToken);
            if (!refreshToken.IsActive) return null;

            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            newRefreshToken.UserId = user.id;
            await _accountRepository.UpdateToken(refreshToken);
            await _accountRepository.RevokeAndAddToken(newRefreshToken);

            var claims = GetClaims(user);
            var jwtToken = CreateToken(claims);

            return new AuthResDto()
            {
                Id = user.id,
                JwtToken = jwtToken,
                ExpiresIn = _options.CurrentValue.RefreshExpirationSeconds,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<bool> RevokeToken(string reqRefreshToken, string ipAddress)
        {
            var user = await _accountRepository.FindByToken(reqRefreshToken);
            if (user == null) return false;

            var refreshToken = (await _accountRepository.GetTokens(user.id)).Single(x => x.Token == reqRefreshToken);
            if (!refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _accountRepository.UpdateToken(refreshToken);
            await _accountRepository.RevokeTokens(user.id);

            return true;
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            string refreshToken = "";
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
            }
            return new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddSeconds(_options.CurrentValue.RefreshExpirationSeconds),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private List<Claim> GetClaims(Account user)
        {
            var claims = new List<Claim>() { new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()), new Claim("name", user.username) };
            var roles = user.GetRoles();
            if (roles != null)
            {
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private bool validateToken(string token, string pin)
        {
            var totp = new Totp(Base32Encoding.ToBytes(token), 15);
            var result = totp.VerifyTotp(pin, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
            var serverToken = generateToken(token);
            var clientToken = uint.Parse(pin);
            return serverToken == clientToken;
        }

        public async Task<string?> AddPendingAuthenticator()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32String = Base32Encoding.ToString(key);
            var result = await _accountRepository.AddPendingAuthenticator(_userProvider.CurrentUser!.Id, base32String);
            if (result == false)
                return null;
            return base32String;
        }

        public async Task<bool> AddAuthenticator(string pin)
        {
            var ext = await _accountRepository.GetExt(_userProvider.CurrentUser!.Id);
            if (ext == null)
                return false;
            if (ext.PendingToken == null)
                return false;
            bool result = validateToken(ext.PendingToken, pin);
            if (result == false)
                return false;
            return await _accountRepository.QualifyPendingToken(ext);
        }

        public async Task<bool> VerifyEmail(string token)
        {
            bool result = await _accountRepository.VerifyPendingEmail(token);
            return result;
        }

        public async Task<IActionResult?> CreateAccount(string username, string password, string email, string url)
        {
            byte[] salt = new byte[32];
            rngCsp.GetBytes(salt);
            BigInteger saltInteger = new BigInteger(salt);
            byte[] verifier = CalculateSRP6Verifier(username.ToUpper(), password.ToUpper(), salt);
            BigInteger verifierInteger = new BigInteger(verifier);
            Guid g;
            var accountDraft = new Account
            {
                email = null,
                username = username,
                gmlevel = 0,
                s = saltInteger.ToString("X"),
                v = verifierInteger.ToString("X")
            };
            Account? newAccount;
            bool result = false;
            do
            {
                g = Guid.NewGuid();
                newAccount = await _accountRepository.Create(accountDraft, email, g.ToString());
            }
            while (newAccount == null);

            var emailResult = await _emailService.SendToken(username, email, g.ToString(), url, "en-GB", Operation.SendConfirmationEmail);
            return emailResult;
        }

        public async Task<IActionResult?> ResendValidationEmail(string url)
        {
            var data = await _accountRepository.GetEmailValidationData(_userProvider.CurrentUser!.Id);
            if (data == null || data.Value.validationToken == null) { return new BadRequestObjectResult("No pending email change."); }
            if (data.Value.lastSentTime < DateTime.UtcNow.AddMinutes(-2)) { return new BadRequestObjectResult("New request too soon"); }

            var emailResult = await _emailService.SendToken(_userProvider.CurrentUser.Name, data.Value.email!, g.ToString(), url, "en-GB", Operation.SendConfirmationEmail);
            return emailResult;
        }
    }
}
