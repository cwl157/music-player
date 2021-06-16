using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Services
{
    public class FileLibraryLoader : ILibraryLoader
    {
        public FileLibraryLoader()
        {
        }

        public void Load(System.IO.DirectoryInfo root, List<Song> result, DateTime lastSyncTime)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.mp3");
                
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                //log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                if (files.Length > 0)
                {
                    foreach (System.IO.FileInfo fi in files)
                    {
                        //var x = new TagLib.Id3v2.Tag()
                        // In this example, we only access the existing FileInfo object. If we
                        // want to open, delete or modify the file, then
                        // a try-catch block is required here to handle the case
                        // where the file has been deleted since the call to TraverseTree().
                        if (fi.Extension.ToLower() == ".mp3")
                        {

                            var tfile = TagLib.File.Create(fi.FullName);
                            Song s = new Song();
                            s.Artist = tfile.Tag.FirstPerformer;
                            s.Album = tfile.Tag.Album;
                            s.Title = tfile.Tag.Title;
                            s.TrackNumber = (int)tfile.Tag.Track;
                            s.Duration = tfile.Properties.Duration;
                            s.FilePath = fi.FullName;
                            s.Lyrics = tfile.Tag.Lyrics;
                            if (tfile.Tag.Comment == null)
                            {
                                s.Comment = "";
                            }
                            else if (tfile.Tag.Comment.ToLower().Contains("various"))
                            {
                                s.Comment = "various";
                            }
                            else
                            {
                                s.Comment = "";
                            }
                            //s.Comment = tfile.Tag.Comment ?? tfile.Tag.Comment.ToLower().Contains("various") ? "various : "";
                            //if (tfile.Tag.Pictures.Length > 0)
                            //{
                            //    s.AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
                            //}
                            s.Year = tfile.Tag.Year.ToString();
                            s.DateAdded = fi.CreationTimeUtc;
                            result.Add(s);
                        }
                    }
                }
                else
                {
                    // Now find all the subdirectories under this directory.
                    subDirs = root.GetDirectories();

                    foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                    {
                        // Resursive call for each subdirectory.
                        Load(dirInfo, result, lastSyncTime);
                    }
                }
            }
        }

        //public List<Song> Load(string filepath)
        //{
        //    DirectoryInfo dir = new DirectoryInfo(filepath);
        //    List<Song> songs = new List<Song>();
        //    foreach (FileInfo f in dir.GetFiles())
        //    {
        //        if (f.Extension.ToLower() == ".mp3")
        //        {
        //            var tfile = TagLib.File.Create(f.FullName);
        //            Song s = new Song();
        //            s.Artist = tfile.Tag.FirstPerformer;
        //            s.Album = tfile.Tag.Album;
        //            s.Title = tfile.Tag.Title;
        //            s.TrackNumber = (int)tfile.Tag.Track;
        //            s.Duration = tfile.Properties.Duration;
        //            s.FilePath = f.FullName;
        //            s.Lyrics = tfile.Tag.Lyrics;
        //            //if (tfile.Tag.Pictures.Length > 0)
        //            //{
        //            //    s.AlbumArt = LoadImage(tfile.Tag.Pictures[0].Data.Data);
        //            //}
        //            s.Year = tfile.Tag.Year.ToString();
        //            songs.Add(s);
        //        }
        //    }

        //    songs = songs.OrderBy(s => s.TrackNumber).ToList();

        //    return songs;
        //}

        //private static BitmapImage LoadImage(byte[] imageData)
        //{
        //    if (imageData == null || imageData.Length == 0) return null;
        //    var image = new BitmapImage();
        //    using (var mem = new MemoryStream(imageData))
        //    {
        //        mem.Position = 0;
        //        image.BeginInit();
        //        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        //        image.CacheOption = BitmapCacheOption.OnLoad;
        //        image.UriSource = null;
        //        image.StreamSource = mem;
        //        image.EndInit();
        //    }
        //    image.Freeze();
        //    return image;
        //}
    }
}
