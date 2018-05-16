﻿using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.Core.Objects
{
    class SkinElement
    {
        private FileType fileType;
        private string extension;

        public string Path { get; set; }

        public string TempPath { get
            {
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Osmo");


                foreach (string fi in Directory.EnumerateFiles(tempPath, "ImageElement.*"))
                {
                    File.Delete(fi);
                }

                return System.IO.Path.Combine(tempPath, 
                    "Osmo\\ImageElement." + extension);
            }
        }

        public string Name { get; set; }
        
        public BitmapImage Image { get; set; }

        public FileType FileType { get => fileType; }

        //TODO: Implement detection of HD elements (see https://osu.ppy.sh/help/wiki/Ranking_Criteria/Skin_Set_List/ for recommended sizes)
        public bool IsHighDefinition { get => false; }

        internal SkinElement(FileInfo fi)
        {
            Path = fi.FullName;
            Name = fi.Name;
            extension = fi.Extension;

            fileType = GetFileType(fi.Extension);

            if (fileType == FileType.Image)
            {
                Image = LoadImage(Path);
            }
        }

        private SkinElement()
        {
            Path = "";
            Name = "";
            fileType = FileType.Unknown;
        }

        internal void RefreshImage()
        {
            Image = LoadImage(Path);
        }

        private BitmapImage LoadImage(string path)
        {
            BitmapImage bmp = new BitmapImage();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = fs;
                bmp.EndInit();
            }

            return bmp;
        }

        internal static SkinElement Empty { get => new SkinElement(); }

        private FileType GetFileType(string extension)
        {
            switch (extension)
            {
                case ".ini":
                    return FileType.Configuration;
                case ".png":
                case ".jpg":
                case ".jpeg":
                    return FileType.Image;
                case ".ogg":
                case ".wav":
                case ".mp3":
                    return FileType.Audio;
                default:
                    return FileType.Unknown;
            }
        }

        #region Method and operator overrides
        public static bool operator ==(SkinElement element, string path)
        {
            if (path != null)
                return element.Path.Contains(path);
            else
                return false;
        }

        public static bool operator !=(SkinElement element, string path)
        {
            if (path != null)
                return !element.Path.Contains(path);
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj != BindingOperations.DisconnectedSource && 
                Path != null && (obj as SkinElement).Path != null)
                return Path.Contains((obj as SkinElement).Path);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}