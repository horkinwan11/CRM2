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
	public class EZSession
	{
		readonly HttpContext _httpContext;

		public EZSession(
			IHttpContextAccessor contextAccessor
		)
		{
			_httpContext = contextAccessor.HttpContext;
		}

		public string wkCampaignId
		{
			get
			{
				return GetSessionValue("wKCampaignId");
			}
			set
			{
				SetSessionValue("wKCampaignId", value);
			}
		}


		public string wKCampaignName
		{
			get
			{
				return GetSessionValue("wKCampaignName");
			}
			set
			{
				SetSessionValue("wKCampaignName", value);
			}
		}

		private string GetSessionValue(string key)
		{
			string valueInString = "";
			var valueInByte = default(byte[]);
			if (_httpContext.Session.TryGetValue(key, out valueInByte))
				valueInString = Encoding.UTF8.GetString(valueInByte);

			return valueInString;

		}
		private void SetSessionValue(string key, string value)
		{
			_httpContext.Session.Set(key, Encoding.UTF8.GetBytes(value));
		}

	}
		
}
