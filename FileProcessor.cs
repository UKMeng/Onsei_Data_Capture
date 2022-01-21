using Serilog;
using TagLib;

namespace ODC
{
    class FileProcessor
    {
        public static List<string> Queries {get; set;} = new List<string>();
        public static void InitializeFileProcessor()
        {
            try
            {
                Log.Logger = Settings.Logger;
                IsVaildWorkingDir();
                CreateOutputDir();
                AddQueries();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void IsVaildWorkingDir()
        {
            Log.Information("SourceDir: " + Settings.WorkingDir);
            if(!Directory.Exists(Settings.WorkingDir))
            {                
                throw new DirectoryNotFoundException("Error: Can't find the Source Folder, Please check your configuration.");
            }
        }
        private static void AddQueries()
        {
            try
            {
                var dirs = from dir in
                        Directory.EnumerateDirectories(Settings.WorkingDir, "RJ*")
                        select dir;
                foreach(var dir in dirs)
                {
                    //Console.WriteLine(dir);
                    Queries.Add(dir);
                    //Console.WriteLine(Path.GetFileName(dir));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void CreateOutputDir()
        {
            try
            {
                if(!Directory.Exists(Settings.OutputDir))
                {
                    Directory.CreateDirectory(Settings.OutputDir);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void MoveAllFilesFromDirectory(string sourceDir, string destDir)
        {
            try
            {
                Log.Debug("Start Moving");
                if(!Directory.Exists(destDir))
                {
                    Directory.Move(sourceDir, destDir);
                }
                else
                {
                    var files = Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)
                        .GroupBy(s=> Path.GetDirectoryName(s));
                    foreach (var folder in files)
                    {
                        var targetFolder = folder.Key.Replace(sourceDir, destDir);
                        Directory.CreateDirectory(targetFolder);
                        foreach (var file in folder)
                        {
                            var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                            if (System.IO.File.Exists(targetFile)) System.IO.File.Delete(targetFile);
                            System.IO.File.Move(file, targetFile);
                        }
                    }
                    Directory.Delete(sourceDir, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void EditTags(string albumPath, Crawler album)
        {
            try
            {
                var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp3", ".m4a", ".flac", ".wav"};
                var audioFiles = from file in Directory.EnumerateFiles(albumPath, "*")
                                where extensions.Contains(Path.GetExtension(file))
                                select file;
                foreach(var file in audioFiles)
                {
                    Log.Debug(file);
                    if(string.Equals(Path.GetExtension(file), ".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        WavFileEditor(file, album);
                    } else
                    {
                        AudioFileEditor(file, album);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void AudioFileEditor(string filePath, Crawler album)
        {
            try
            {
                var tfile = TagLib.File.Create(filePath);
                tfile.Tag.Clear();
                tfile.Tag.Album = album.title;
                tfile.Tag.Performers = album.actorNames.ToArray();
                tfile.Tag.AlbumArtists = album.actorNames.ToArray();
                tfile.Tag.Genres = album.tags.ToArray();
                tfile.Tag.Year = UInt32.Parse(album.releaseYear);
                tfile.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
        /// <summary>
        /// Edit metadata of .wav audio file
        /// .wav audio file's metadata use RIFF INFO TAG
        /// https://picard-docs.musicbrainz.org/en/appendices/tag_mapping.html
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="album"></param>
        public static void WavFileEditor(string filePath, Crawler album)
        {
            try
            {
                var tfile = TagLib.File.Create(filePath);
                tfile.Tag.Clear();
                TagLib.Riff.InfoTag tags = (TagLib.Riff.InfoTag)tfile.GetTag(TagLib.TagTypes.RiffInfo, false);
                if(tags != null)
                {
                    tags.SetValue("IART", album.actorStr);      // Aritist
                    tags.SetValue("IGNR", album.tagStr);   // Genre
                    tags.SetValue("IPRD", album.title);  // Album Title
                }
                else
                {
                    throw new InvalidOperationException("This file do not have RiffInfo tag");
                }
                tfile.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}