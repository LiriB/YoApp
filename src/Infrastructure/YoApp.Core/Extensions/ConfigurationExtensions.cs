﻿using Microsoft.Extensions.Configuration;

namespace YoApp.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsLocalInstance(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("RUN_LOCAL");
        }
    }
}
