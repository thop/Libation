﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dinah.Core;
using Dinah.Core.Collections.Generic;

namespace FileManager
{
    // could add images here, but for now images are stored in a well-known location
    public enum FileType { Unknown, Audio, AAX, PDF }

	/// <summary>
	/// Files are large. File contents are never read by app.
	/// Paths are varied.
	/// Files are written during download/decrypt/backup/liberate.
	/// Paths are read at app launch and during download/decrypt/backup/liberate.
	/// Many files are often looked up at once
	/// </summary>
	public abstract class AudibleFileStorage : Enumeration<AudibleFileStorage>
	{
		public abstract string[] Extensions { get; }
		public abstract string StorageDirectory { get; }

		#region static
		public static AudioFileStorage Audio { get; } = new AudioFileStorage();
		public static AudibleFileStorage AAX { get; } = new AaxFileStorage();
		public static AudibleFileStorage PDF { get; } = new PdfFileStorage();

		public static string DownloadsInProgress
        {
            get
            {
                if (!Configuration.Instance.DownloadsInProgressEnum.In("WinTemp", "LibationFiles"))
                    Configuration.Instance.DownloadsInProgressEnum = "WinTemp";
                var AaxRootDir
                    = Configuration.Instance.DownloadsInProgressEnum == "WinTemp"
                    ? Configuration.WinTemp
                    : Configuration.Instance.LibationFiles;

                return Directory.CreateDirectory(Path.Combine(AaxRootDir, "DownloadsInProgress")).FullName;
            }
        }

        public static string DecryptInProgress
        {
            get
            {
                if (!Configuration.Instance.DecryptInProgressEnum.In("WinTemp", "LibationFiles"))
                    Configuration.Instance.DecryptInProgressEnum = "WinTemp";

                var M4bRootDir
                    = Configuration.Instance.DecryptInProgressEnum == "WinTemp"
                    ? Configuration.WinTemp
                    : Configuration.Instance.LibationFiles;

                return Directory.CreateDirectory(Path.Combine(M4bRootDir, "DecryptInProgress")).FullName;
            }
        }

        // not customizable. don't move to config
        public static string DownloadsFinal => new DirectoryInfo(Configuration.Instance.LibationFiles).CreateSubdirectory("DownloadsFinal").FullName;

        public static string BooksDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Configuration.Instance.Books))
                    Configuration.Instance.Books = Path.Combine(Configuration.Instance.LibationFiles, "Books");
                return Directory.CreateDirectory(Configuration.Instance.Books).FullName;
            }
        }
        #endregion

        #region instance
        public FileType FileType => (FileType)Value;

		private IEnumerable<string> extensions_noDots { get; }
		private string extAggr { get; }

        protected AudibleFileStorage(FileType fileType) : base((int)fileType, fileType.ToString())
		{
			extensions_noDots = Extensions.Select(ext => ext.Trim('.')).ToList();
			extAggr = extensions_noDots.Aggregate((a, b) => $"{a}|{b}");
		}

        /// <summary>
        /// Example for full books:
        /// Search recursively in _books directory. Full book exists if either are true
        /// - a directory name has the product id and an audio file is immediately inside
        /// - any audio filename contains the product id
        /// </summary>
        public bool Exists(string productId) => GetPath(productId) != null;

		public string GetPath(string productId)
        {
            var cachedFile = FilePathCache.GetPath(productId, FileType);
            if (cachedFile != null)
                return cachedFile;

            var firstOrNull =
				Directory
				.EnumerateFiles(StorageDirectory, "*.*", SearchOption.AllDirectories)
				.FirstOrDefault(s => Regex.IsMatch(s, $@"{productId}.*?\.({extAggr})$", RegexOptions.IgnoreCase));

			if (firstOrNull is null)
				return null;

			FilePathCache.Upsert(productId, FileType, firstOrNull);
			return firstOrNull;
        }

        public string GetDestDir(string title, string asin)
        {
            // to prevent the paths from getting too long, we don't need after the 1st ":" for the folder
            var underscoreIndex = title.IndexOf(':');
            var titleDir
                = underscoreIndex < 4
                ? title
                : title.Substring(0, underscoreIndex);
            var finalDir = FileUtility.GetValidFilename(StorageDirectory, titleDir, null, asin);
            return finalDir;
        }

        public bool IsFileTypeMatch(FileInfo fileInfo)
            => extensions_noDots.ContainsInsensative(fileInfo.Extension.Trim('.'));
        #endregion
    }

    public class AudioFileStorage : AudibleFileStorage
    {
        public const string SKIP_FILE_EXT = "libhack";

		public override string[] Extensions { get; } = new[] { "m4b", "mp3", "aac", "mp4", "m4a", "ogg", "flac", SKIP_FILE_EXT };

        // we always want to use the latest config value, therefore
        // - DO use 'get' arrow "=>"
        // - do NOT use assign "="
        public override string StorageDirectory => BooksDirectory;

        public AudioFileStorage() : base(FileType.Audio) { }

        public string CreateSkipFile(string title, string asin, string contents = null)
        {
            var destinationDir = GetDestDir(title, asin);
            Directory.CreateDirectory(destinationDir);

            var path = FileUtility.GetValidFilename(destinationDir, title, SKIP_FILE_EXT, asin);
            File.WriteAllText(path, contents ?? string.Empty);

            return path;
        }
    }

    public class AaxFileStorage : AudibleFileStorage
    {
        public override string[] Extensions { get; } = new[] { "aax" };

        // we always want to use the latest config value, therefore
        // - DO use 'get' arrow "=>"
        // - do NOT use assign "="
        public override string StorageDirectory => DownloadsFinal;

        public AaxFileStorage() : base(FileType.AAX) { }
    }

    public class PdfFileStorage : AudibleFileStorage
    {
		public override string[] Extensions { get; } = new[] { "pdf", "zip" };

        // we always want to use the latest config value, therefore
        // - DO use 'get' arrow "=>"
        // - do NOT use assign "="
        public override string StorageDirectory => BooksDirectory;

        public PdfFileStorage() : base(FileType.PDF) { }
    }
}
