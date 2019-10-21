﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Dinah.Core;

namespace FileManager
{
    public class Configuration
    {
        // settings will be persisted when all are true
        // - property (not field)
        // - string
        // - public getter
        // - public setter

        #region // properties to test reflection
        /*
        // field should NOT be populated
        public string TestField;
        // int should NOT be populated
        public int TestInt { get; set; }
        // read-only should NOT be populated
        public string TestGet { get; } // get only: should NOT get auto-populated
        // set-only should NOT be populated
        public string TestSet { private get; set; }

        // get and set: SHOULD be auto-populated
        public string TestGetSet { get; set; }
        */
        #endregion

        private const string configFilename = "LibationSettings.json";

        private PersistentDictionary persistentDictionary { get; }

        [Description("Location of the configuration file where these settings are saved. Please do not edit this file directly while Libation is running.")]
        public string Filepath { get; }

        [Description("Your user-specific key used to decrypt your audible files (*.aax) into audio files you can use anywhere (*.m4b)")]
        public string DecryptKey
        {
            get => persistentDictionary[nameof(DecryptKey)];
            set => persistentDictionary[nameof(DecryptKey)] = value;
        }

        [Description("Location for book storage. Includes destination of newly liberated books")]
        public string Books
        {
            get => persistentDictionary[nameof(Books)];
            set => persistentDictionary[nameof(Books)] = value;
        }

        public string WinTemp { get; } = Path.Combine(Path.GetTempPath(), "Libation");

        [Description("Location for storage of program-created files")]
        public string LibationFiles
        {
            get => persistentDictionary[nameof(LibationFiles)];
            set => persistentDictionary[nameof(LibationFiles)] = value;
        }

        // default setting and directory creation occur in class responsible for files.
        // config class is only responsible for path. not responsible for setting defaults, dir validation, or dir creation

        // temp/working dir(s) should be outside of dropbox
        [Description("Temporary location of files while they're in process of being downloaded.\r\nWhen download is complete, the final file will be in [LibationFiles]\\DownloadsFinal")]
        public string DownloadsInProgressEnum
        {
            get => persistentDictionary[nameof(DownloadsInProgressEnum)];
            set => persistentDictionary[nameof(DownloadsInProgressEnum)] = value;
        }

        // temp/working dir(s) should be outside of dropbox
        [Description("Temporary location of files while they're in process of being decrypted.\r\nWhen decryption is complete, the final file will be in Books location")]
        public string DecryptInProgressEnum
        {
            get => persistentDictionary[nameof(DecryptInProgressEnum)];
            set => persistentDictionary[nameof(DecryptInProgressEnum)] = value;
        }

		public string LocaleCountryCode
		{
			get => persistentDictionary[nameof(LocaleCountryCode)];
			set => persistentDictionary[nameof(LocaleCountryCode)] = value;
		}

        // singleton stuff
        public static Configuration Instance { get; } = new Configuration();
        private Configuration()
        {
            Filepath = getPath();

            // load json values into memory
            persistentDictionary = new PersistentDictionary(Filepath);
            ensureDictionaryEntries();

            // setUserFilesDirectoryDefault
            // don't create dir. dir creation is the responsibility of places that use the dir
            if (string.IsNullOrWhiteSpace(LibationFiles))
                LibationFiles = Path.Combine(Path.GetDirectoryName(Exe.FileLocationOnDisk), "Libation");
        }

        public static string GetDescription(string propertyName)
        {
			var attribute = typeof(Configuration)
				.GetProperty(propertyName)
				?.GetCustomAttributes(typeof(DescriptionAttribute), true)
				.SingleOrDefault()
				as DescriptionAttribute;

			return attribute?.Description;
		}

        private static string getPath()
        {
            // search folders for config file. accept the first match
            var defaultdir = Path.GetDirectoryName(Exe.FileLocationOnDisk);

            var baseDirs = new HashSet<string>
            {
                defaultdir,
                getNonDevelopmentDir(defaultdir),
                Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            };

            var subDirs = baseDirs.Select(dir => Path.Combine(dir, "Libation"));
            var dirs = baseDirs.Concat(subDirs).ToList();

            foreach (var dir in dirs)
            {
                var f = Path.Combine(dir, configFilename);
                if (File.Exists(f))
                    return f;
            }

            return Path.Combine(defaultdir, configFilename);
        }

        private static string getNonDevelopmentDir(string path)
        {
            // examples:
            // \Libation\Core2_0\bin\Debug\netcoreapp3.0
            // \Libation\StndLib\bin\Debug\netstandard2.1
            // \Libation\MyWnfrm\bin\Debug
            // \Libation\Core2_0\bin\Release\netcoreapp3.0
            // \Libation\StndLib\bin\Release\netstandard2.1
            // \Libation\MyWnfrm\bin\Release

            var curr = new DirectoryInfo(path);

            if (!curr.Name.EqualsInsensitive("debug") && !curr.Name.EqualsInsensitive("release") && !curr.Name.StartsWithInsensitive("netcoreapp") && !curr.Name.StartsWithInsensitive("netstandard"))
                return path;

            // get out of netcore/standard dir => debug
            if (curr.Name.StartsWithInsensitive("netcoreapp") || curr.Name.StartsWithInsensitive("netstandard"))
                curr = curr.Parent;

            if (!curr.Name.EqualsInsensitive("debug") && !curr.Name.EqualsInsensitive("release"))
                return path;

            // get out of debug => bin
            curr = curr.Parent;
            if (!curr.Name.EqualsInsensitive("bin"))
                return path;

            // get out of bin
            curr = curr.Parent;
            // get out of csproj-level dir
            curr = curr.Parent;

            // curr should now be sln-level dir
            return curr.FullName;
        }

        private void ensureDictionaryEntries()
        {
            var stringProperties = getDictionaryProperties().Select(p => p.Name).ToList();
            var missingKeys = stringProperties.Except(persistentDictionary.Keys).ToArray();
            persistentDictionary.AddKeys(missingKeys);
        }

        private IEnumerable<System.Reflection.PropertyInfo> dicPropertiesCache;
        private IEnumerable<System.Reflection.PropertyInfo> getDictionaryProperties()
        {
            if (dicPropertiesCache == null)
                dicPropertiesCache = PersistentDictionary.GetPropertiesToPersist(this.GetType());
            return dicPropertiesCache;
        }
    }
}
