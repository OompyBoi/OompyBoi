﻿using Microsoft.Extensions.Logging;
using Server.Base.Core.Helpers;
using Web.AssetBundles.Models;

namespace Web.AssetBundles.Extensions;

public static class GetInfoFile
{
    public static string GetWebPlayerInfoFile(this AssetBundleRwConfig rwConfig, AssetBundleRConfig rConfig,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        rwConfig.WebPlayerInfoFile = TryGetInfoFile($"Web Player '{rConfig.DefaultWebPlayerCacheLocation}'",
            rwConfig.WebPlayerInfoFile, logger);

        if (rwConfig.WebPlayerInfoFile == rwConfig.CacheInfoFile)
        {
            logger.LogError("Web player cache and saved directory should not be the same! Skipping...");
            rwConfig.WebPlayerInfoFile = string.Empty;
        }

        if (!rwConfig.WebPlayerInfoFile.Contains("appdata", StringComparison.CurrentCultureIgnoreCase))
        {
            logger.LogError("Web player cache has to be in the AppData/LocalLow folder! Skipping...");
            rwConfig.WebPlayerInfoFile = string.Empty;
        }

        return rwConfig.WebPlayerInfoFile;
    }

    public static string TryGetInfoFile(string cacheName, string defaultFile,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        var lowerName = cacheName.ToLower();

        logger.LogDebug("Getting the {Type} cache directory...", lowerName);

        try
        {
            defaultFile = SetFileValue.SetIfNotNull(defaultFile, $"Get the {cacheName} '__info' Cache File",
                $"{cacheName} Info File (__info)\0__info\0");
        }
        catch
        {
            // ignored
        }

        while (true)
        {
            if (string.IsNullOrEmpty(defaultFile) || !defaultFile.EndsWith("__info"))
            {
                logger.LogError("Please enter the absolute file path for the {Type} '__info' cache file.", lowerName);
                defaultFile = Console.ReadLine() ?? string.Empty;
                continue;
            }

            break;
        }

        logger.LogInformation("{Type} Cache Directory: {Directory}", cacheName, Path.GetDirectoryName(defaultFile));

        return defaultFile;
    }
}
