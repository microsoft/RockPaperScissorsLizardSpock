using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Config
{
    public class FolderConfigurationProvider : ConfigurationProvider
    {
        private readonly FolderConfigurationSource _source;
        public FolderConfigurationProvider(FolderConfigurationSource source)
        {
            _source = source;
        }
        public override void Load()
        {
            if (Directory.Exists(_source.Folder))
            {
                var entries = Directory.EnumerateFiles(_source.Folder);
                foreach (var entry in entries)
                {
                    var name = Path.GetFileName(entry);
                    if (!string.IsNullOrEmpty(_source.Prefix) && name.StartsWith(_source.Prefix))
                    {
                        name = name.Substring(_source.Prefix.Length);
                    }

                    Data.Add(name, File.ReadAllText(entry));
                }
            }
        }

    }
}

