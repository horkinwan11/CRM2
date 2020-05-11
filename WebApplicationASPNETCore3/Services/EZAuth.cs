using CRM.Models;
using CRM.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Services
{
	public class EZAuth
	{
		readonly HttpContext _httpContext;
		readonly Cryptography _cryptography;
		readonly AuthOptions _authConfiguration;
		readonly CRMContext _context;

		public EZAuth(
			IHttpContextAccessor contextAccessor,
			Cryptography cryptography,
			IOptions<AuthOptions> authConfiguration,
			CRMContext context)
		{
			_httpContext = contextAccessor.HttpContext;
			_cryptography = cryptography;
			_authConfiguration = authConfiguration.Value;
			_context = context;
		}

		private AuthInfo _scopeAuthInfo = null;
		public AuthInfo ScopeAuthInfo
		{
			get
			{
				if (_scopeAuthInfo == null)
				{
					AuthInfo tokenAuthInfo = null;
					var cookieValue = _httpContext.Request.Cookies["ezAuthToken"];
					if (!string.IsNullOrEmpty(cookieValue))
					{
						try
						{
							tokenAuthInfo = AuthInfoFromToken(cookieValue);
							CookieOptions options = new CookieOptions();
							//if (expireTime.HasValue)
							//	options.Expires = DateTime.Now.AddMinutes(expireTime.Value);
							//else
							options.Expires = DateTimeOffset.Now.AddMinutes(5);
							_httpContext.Response.Cookies.Delete("ezAuthToken");
							_httpContext.Response.Cookies.Append("ezAuthToken", AuthInfoToToken(tokenAuthInfo), options);

						}
						catch
						{
						}
					}
					_scopeAuthInfo = tokenAuthInfo != null ? tokenAuthInfo : new AuthInfo();


				}
				return _scopeAuthInfo;
			}
		}
		public bool IsAuthenticated
			{
				get
				{ 
					return _httpContext.User.Identity.IsAuthenticated;
				} 
			}
		public String UserName
		{
			get
			{
				return _httpContext.User.Identity.Name;
			}
		}
		

		public async Task<bool> SignIn(string email, string password)
		{
			var user = _context.User.Include(u => u.UserCredential).FirstOrDefault(u => u.Email == email); 
			
			if (user == null) return false;

			var userCredential = user.UserCredential;
			var claimedPasswordHashed = _cryptography.HashSHA256(password + userCredential.PasswordSalt);

			if (claimedPasswordHashed != userCredential.HashedPassword) return false;

			var permissions = _context.UserPermission.Where(up => up.UserId == user.Id)
									.Select(up => up.Permission.Code).ToList();

			await LoginAsync(user,permissions);
			return true;

			//AuthInfo authInfo = new AuthInfo() {
			//	UserId = user.Id,
			//	FirstName = user.FirstName,
			//	LastName = user.LastName,
			//	Email = user.Email,
			//	Claims = new Dictionary<string, string>(),
			//	Permissions = permissions,
			//};
			//CookieOptions options = new CookieOptions();

			//_httpContext.Response.Cookies.Append("ezAuthToken", AuthInfoToToken(authInfo), options);
			//return true;
		}

		public async Task SignOut()
		{
			//_httpContext.Response.Cookies.Delete("ezAuthToken");
			await LogoutAsync();
		}

		private string AuthInfoToToken(AuthInfo authInfo)
		{
			var serializedAuthInfo = JsonConvert.SerializeObject(authInfo);

			// Encrypt serialized authInfo
			var key = Encoding.UTF8.GetBytes(_authConfiguration.AuthEncryptionKey);
			var iv = Aes.Create().IV;
			var ivBase64 = Convert.ToBase64String(iv);
			var encBytes = _cryptography.EncryptStringToBytes_Aes(serializedAuthInfo, key, iv);
			var result = $"{ivBase64.Length.ToString().PadLeft(3, '0')}{ivBase64}{Convert.ToBase64String(encBytes)}";
			return result;
		}

		private AuthInfo AuthInfoFromToken(string token)
		{
			// Decrypt token
			string decryptedToken;
			var ivLength = Convert.ToInt32(token.Substring(0, 3));
			var ivBase64 = token.Substring(3, ivLength);
			var iv = Convert.FromBase64String(ivBase64);
			var encBase64 = token.Substring(ivLength + 3);
			var encBytes = Convert.FromBase64String(encBase64);
			var key = Encoding.UTF8.GetBytes(_authConfiguration.AuthEncryptionKey);
			decryptedToken = _cryptography.DecryptStringFromBytes_Aes(encBytes, key, iv);

			// Deserialize decrypted token
			var result = JsonConvert.DeserializeObject<AuthInfo>(decryptedToken);
			return result;
		}



		private async Task LoginAsync(User user, List<String> permissions)
		{
			var properties = new AuthenticationProperties
			{
				AllowRefresh = true,
				//IsPersistent = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15),


			};
			
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Surname, user.LastName),
				new Claim(ClaimTypes.Name, user.Email )   //Http Identity Name
		    };
			
			foreach (String perm in permissions)
				claims.Add(new Claim(ClaimTypes.Role, perm));

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			await _httpContext.SignInAsync(principal, properties);
		}

		private async Task LogoutAsync()
		{
			
				await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			
		}
	}
}
