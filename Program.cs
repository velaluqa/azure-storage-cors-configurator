namespace azurestoragecorsconfigurator
{

  ﻿using System;
  using System.Collections.Generic;
  using Microsoft.WindowsAzure.Storage;
  using Microsoft.WindowsAzure.Storage.Auth;
  using Microsoft.WindowsAzure.Storage.Blob;
  using Microsoft.WindowsAzure.Storage.Shared.Protocol;

  class Program
  {
    static List<String> ALLOWED_CORS_ORIGINS = new List<String> { "*" };
    static List<String> ALLOWED_CORS_HEADERS = new List<String> { "x-ms-meta-qqfilename", "Content-Type", "x-ms-blob-type", "x-ms-blob-content-type" };
    const CorsHttpMethods ALLOWED_CORS_METHODS = CorsHttpMethods.Delete | CorsHttpMethods.Put;
    const int ALLOWED_CORS_AGE_DAYS = 5;

    [STAThread]
    private static void Main(string[] args)
    {
      var storageAccountName = System.Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME");
      if (storageAccountName == null)
      {
        Console.WriteLine("Please set the STORAGE_ACCOUNT_NAME environment variable.");
        return;
      }

      var storageAccountKey= System.Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_KEY");
      if (storageAccountKey == null)
      {
        Console.WriteLine("Please set the STORAGE_ACCOUNT_KEY environment variable.");
        return;
      }

      var accountAndKey = new StorageCredentials(storageAccountName, storageAccountKey);
      var storageAccount = new CloudStorageAccount(accountAndKey, true);

      configureCors(storageAccount);
    }

    private static void configureCors(CloudStorageAccount storageAccount)
    {
      var blobClient = storageAccount.CreateCloudBlobClient();

      Console.WriteLine("Storage Account: " + storageAccount.BlobEndpoint);
      var newProperties = CurrentProperties(blobClient);

      newProperties.DefaultServiceVersion = "2013-08-15";
      blobClient.SetServiceProperties(newProperties);

      var addRule = true;
      if (addRule)
      {
        var ruleWideOpenWriter = new CorsRule()
        {
          AllowedHeaders = ALLOWED_CORS_HEADERS,
          AllowedOrigins = ALLOWED_CORS_ORIGINS,
          AllowedMethods = ALLOWED_CORS_METHODS,
          MaxAgeInSeconds = (int)TimeSpan.FromDays(ALLOWED_CORS_AGE_DAYS).TotalSeconds
        };
        newProperties.Cors.CorsRules.Clear();
        newProperties.Cors.CorsRules.Add(ruleWideOpenWriter);
        blobClient.SetServiceProperties(newProperties);

        Console.WriteLine("New Properties:");
        CurrentProperties(blobClient);

        Console.ReadLine();
      }
    }

    private static ServiceProperties CurrentProperties(CloudBlobClient blobClient)
    {
      var currentProperties = blobClient.GetServiceProperties();
      if (currentProperties != null)
      {
        if (currentProperties.Cors != null)
        {
          Console.WriteLine("Cors.CorsRules.Count          : " + currentProperties.Cors.CorsRules.Count);
          for (int index = 0; index < currentProperties.Cors.CorsRules.Count; index++)
          {
            var corsRule = currentProperties.Cors.CorsRules[index];
            Console.WriteLine("corsRule[index]              : " + index);
            foreach (var allowedHeader in corsRule.AllowedHeaders)
            {
              Console.WriteLine("corsRule.AllowedHeaders      : " + allowedHeader);
            }
            Console.WriteLine("corsRule.AllowedMethods      : " + corsRule.AllowedMethods);

            foreach (var allowedOrigins in corsRule.AllowedOrigins)
            {
              Console.WriteLine("corsRule.AllowedOrigins      : " + allowedOrigins);
            }
            foreach (var exposedHeaders in corsRule.ExposedHeaders)
            {
              Console.WriteLine("corsRule.ExposedHeaders      : " + exposedHeaders);
            }
            Console.WriteLine("corsRule.MaxAgeInSeconds     : " + corsRule.MaxAgeInSeconds);
          }
        }
        Console.WriteLine("DefaultServiceVersion         : " + currentProperties.DefaultServiceVersion);
        Console.WriteLine("HourMetrics.MetricsLevel      : " + currentProperties.HourMetrics.MetricsLevel);
        Console.WriteLine("HourMetrics.RetentionDays     : " + currentProperties.HourMetrics.RetentionDays);
        Console.WriteLine("HourMetrics.Version           : " + currentProperties.HourMetrics.Version);
        Console.WriteLine("Logging.LoggingOperations     : " + currentProperties.Logging.LoggingOperations);
        Console.WriteLine("Logging.RetentionDays         : " + currentProperties.Logging.RetentionDays);
        Console.WriteLine("Logging.Version               : " + currentProperties.Logging.Version);
        Console.WriteLine("MinuteMetrics.MetricsLevel    : " + currentProperties.MinuteMetrics.MetricsLevel);
        Console.WriteLine("MinuteMetrics.RetentionDays   : " + currentProperties.MinuteMetrics.RetentionDays);
        Console.WriteLine("MinuteMetrics.Version         : " + currentProperties.MinuteMetrics.Version);
      }
      return currentProperties;
    }
  }
}
