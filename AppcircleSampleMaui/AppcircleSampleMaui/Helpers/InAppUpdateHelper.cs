using System.Text;
using System.Text.Json;
using static AppcircleSampleMaui.Models.InAppUpdateModel;

namespace AppcircleSampleMaui.Helpers;

public static class InAppUpdateHelper
{
    private static async Task<string> GetACToken(string profileId)
	{
		try
		{
			var httpClient = new HttpClient();
			var endpointUrl = $"{Environment.GetEnvironmentVariable("STORE_URL")}/api/auth/token";
			var secret = DeviceInfo.Platform == DevicePlatform.iOS
				? Environment.GetEnvironmentVariable("IOS_STORE_SECRET")
				: Environment.GetEnvironmentVariable("ANDROID_STORE_SECRET");

			var requestBody = new
			{
				ProfileId = profileId,
				Secret = secret
			};

			var json = JsonSerializer.Serialize(requestBody);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await httpClient.PostAsync(endpointUrl, content);
			var responseData = await response.Content.ReadAsStringAsync();
			var responseObject = JsonSerializer.Deserialize<TokenResponse>(responseData);

			return responseObject.access_token;
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to get access token", ex);
		}
		
	}
	
	private static async Task<List<AppVersion>> GetAppVersions(string accessToken)
	{
		var url = $"{Environment.GetEnvironmentVariable("STORE_URL") }/api/app-versions";
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
		};
		using (var httpClient = new HttpClient())
		{
			httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

			var response = await httpClient.GetAsync(url);
			var jsonResponse = await response.Content.ReadAsStringAsync();
			var responseData = JsonSerializer.Deserialize<AppVersionsResponse>(jsonResponse,options);
			return responseData.data;
		}
	}

	/*
    You can implement your custom update check mechanism within this function.
    Currently, we convert the version to an integer and compare it with the 'CFBundleShortVersionString'.
	*/
	public static AppVersion GetLatestVersion(string currentVersion, List<AppVersion> appVersions)
	{
		AppVersion latestAppVersion = null;
		
		// Helper function to convert version string into an array of integers
		int[] VersionComponents(string version)
		{
			return version
				.Split('.')
				.Select(part => int.TryParse(part, out int num) ? num : (int?)null)
				.Where(num => num.HasValue)
				.Select(num => num.Value)
				.ToArray();
		}
		var currentComponents = VersionComponents(currentVersion);
		
		foreach (var app in appVersions)
		{
			// Convert versions to arrays of integers
			var latestComponents = VersionComponents(app.Version);

			// Compare versions component by component
			for (int i = 0; i < Math.Min(currentComponents.Length, latestComponents.Length); i++)
			{
				var current = currentComponents[i];
				var latest = latestComponents[i];

				// You can control to update None, Beta or Live publish types you have selected on Appcircle Enterprise Store
				if (latest > current && app.PublishType == (int)PublishType.Live)
				{
					latestAppVersion = app;
					break; // Assuming once we find a valid version, we don't need to check further.
				}
			}
		}
		
		return latestAppVersion;
	}

	public static string CreateDownloadUrl(string availableVersionId, string accessToken, string email)
	{
		var baseUrl = $"{Environment.GetEnvironmentVariable("STORE_URL")}/api/app-versions/{availableVersionId}/download-version/{accessToken}/user/{email}";
		var downloadUrl = $"itms-services://?action=download-manifest&url={Uri.EscapeDataString(baseUrl)}";
		try
		{
			// Assuming you have a way to determine the platform
			var isIos = DeviceInfo.Platform == DevicePlatform.iOS;

			return isIos ? downloadUrl : baseUrl;
		}
		catch (Exception)
		{
			Console.WriteLine("Latest Version URL could not be created");
			return null;
		}
	}
	public static async Task<UpdateResult> CheckForUpdate(string currentVersion, string userEmail)
	{
		var profileId = DeviceInfo.Platform == DevicePlatform.iOS ?
			Environment.GetEnvironmentVariable("IOS_PROFILE_ID") : 
			Environment.GetEnvironmentVariable("ANDROID_PROFILE_ID");
		if (profileId != null)
		{
			var accessToken = await GetACToken(profileId);
			var appVersions = await GetAppVersions(accessToken);
			var latestVersion = GetLatestVersion(currentVersion, appVersions);
			if (latestVersion != null)
			{
				var downloadUrl = CreateDownloadUrl(latestVersion.Id,accessToken,userEmail);
				if (downloadUrl == null)
				{
					return null;
				}

				return new UpdateResult
				{
					DownloadUrl = downloadUrl,
					Version = latestVersion.Version
				};
			}
		}

		return null;
	}
}