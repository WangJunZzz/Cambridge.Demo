﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Cambridge.Demo.Client.Terminal
{
	/// <summary>
	/// Use the Client Credential Flow to get the access_token
	/// </summary>
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Press a key To start");
			Console.ReadKey();
			//Use The Discovery Endpoint to get a list of AuthServer Endpoints
			DiscoveryCache Cache = new DiscoveryCache("https://localhost:4001");

			var disco = await Cache.GetAsync();
			if (disco.IsError) throw new Exception(disco.Error);

			var tokenClient = new HttpClient();
			//This is an IdentityModel Extension for HttpClient
			var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = "CambridgeDemoClientTwo",
				ClientSecret = "secret",
				Scope = "cambridgedemoapi"
			});

			if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);


			var apiClient = new HttpClient();
			apiClient.BaseAddress = new Uri("https://localhost:61849/");
			//Use IdentityModel Extension to set the AccessToken in the header of the HTTP request
			apiClient.SetBearerToken(tokenResponse.AccessToken);
			
			Console.WriteLine(await apiClient.GetStringAsync("Document/Client"));
		}
    }
}
