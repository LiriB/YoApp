using System;
using System.Collections.Generic;

namespace YoApp.Identity.Core
{
    public interface IConfigurationService
    {
        TimeSpan VerificationDuration { get; }
        IEnumerable<int> CountriesBlackList { get; }
        ConfigurationService.TwillioSettings Twillio { get; }
    }
}