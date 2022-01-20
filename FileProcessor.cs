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
                IsVaildWorkingDir();
                CreateOutputDir();
                AddQueries();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void IsVaildWorkingDir()
        {
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
                    Console.WriteLine(dir);
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
                Console.WriteLine("Start Moving");
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
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp3", ".m4a", ".flac", ".wav", ""};
        }

        public static void ChangeMetaData(string filePath, Crawler album)
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

        // Wav meta data https://www.robotplanet.dk/audio/wav_meta_data/
        public static void WavTest(string filePath)
        {
            try
            {
                var tfile = TagLib.File.Create(filePath);
                Console.WriteLine("Create Success");
                tfile.Tag.Clear();
                /*
                Console.WriteLine(tfile.TagTypes.ToString());
                tfile.Tag.Album = "tsds";
                string[] names = {"a", "b"};
                tfile.Tag.Performers = names;
                */
                TagLib.Riff.InfoTag test = (TagLib.Riff.InfoTag)tfile.GetTag(TagLib.TagTypes.RiffInfo, false);
                if(test != null)
                {
                    test.SetValue("IART", "aaa;bbb");
                    test.SetValue("IGNR", "ASMR;TEST;");
                    test.SetValue("TITL", "TITL");
                    test.SetValue("IPRD", "album title");
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