using Microsoft.Extensions.Configuration;
using RPSLS.Game.Api.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Extensions
{
    public static class FolderConfigurationExtensions
    {
        public static IConfigurationBuilder AddFolder(this IConfigurationBuilder builder, string folderName, string prefix = null) =>
            builder.Add(new FolderConfigurationSource(folderName, prefix));
    }
}
