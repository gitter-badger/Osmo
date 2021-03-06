﻿using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo.ViewModel
{
    class OsmoViewModel : ViewModelBase
    {
        private SkinManager mManager;
        
        private SidebarEntry[] mSidebarItems;
        
        private int mSelectedSkinIndex = -1;
        private int mSelectedSidebarIndex = 0;
        
        private string mBackupDirectory = "";
        private double mBackupDirectorySize = 0;

        public SkinManager SkinManager
        {
            get => mManager;
            set
            {
                if (mManager != null)
                {
                    mManager.SkinChanged -= MManager_SkinChanged;
                    mManager.SkinRenamed -= MManager_SkinRenamed;
                }

                mManager = value;
                if (mManager != null)
                {
                    mManager.SkinChanged += MManager_SkinChanged;
                    mManager.SkinRenamed += MManager_SkinRenamed;
                }
            }
        }

        public VeryObservableCollection<Skin> Skins { get => SkinManager?.Skins; }

        public SidebarEntry[] Items { get => mSidebarItems; }

        public int SelectedSkinIndex
        {
            get => mSelectedSkinIndex;
            set
            {
                mSelectedSkinIndex = value;
                InvokePropertyChanged("SelectedSkinIndex");
            }
        }

        public int SelectedSidebarIndex
        {
            get => mSelectedSidebarIndex;
            set
            {
                mSelectedSidebarIndex = value;
                InvokePropertyChanged("SelectedSidebarIndex");
            }
        }

        public string OsuDirectory
        {
            get => SkinManager != null ? SkinManager.Directory : "";
            set
            {
                if (SkinManager == null)
                    SkinManager = SkinManager.MakeInstance(value);
                else
                    SkinManager.Directory = value;
                InvokePropertyChanged("OsuDirectory");
            }
        }

        public string BackupDirectory
        {
            get => mBackupDirectory;
            set
            {
                mBackupDirectory = value;
                InvokePropertyChanged("BackupDirectory");
            }
        }

        public double BackupDirectorySize
        {
            get => mBackupDirectorySize;
            set
            {
                mBackupDirectorySize = value;
                InvokePropertyChanged("BackupDirectorySize");
            }
        }

        public bool IsEditorEnabled
        {
            set
            {
                mSidebarItems[FixedValues.EDITOR_INDEX].IsEnabled = value;
                InvokePropertyChanged("Items");
            }
        }

        public bool IsMixerEnabled
        {
            set
            {
                mSidebarItems[FixedValues.MIXER_INDEX].IsEnabled = value;
                InvokePropertyChanged("Items");
            }
        }

        public OsmoViewModel()
        {
            FixedValues.InitializeReader();
            string osuDir = AppConfiguration.GetInstance().OsuDirectory;

            if (!string.IsNullOrWhiteSpace(osuDir))
                SkinManager = SkinManager.MakeInstance(osuDir);

            mSidebarItems = new SidebarEntry[]
            {
                new SidebarEntry(Helper.FindString("sidebar_home"), MaterialDesignThemes.Wpf.PackIconKind.Home, SkinSelect.Instance),
                new SidebarEntry(Helper.FindString("sidebar_wizard"), MaterialDesignThemes.Wpf.PackIconKind.AutoFix, SkinCreationWizard.Instance),
                new SidebarEntry(Helper.FindString("sidebar_editor"), MaterialDesignThemes.Wpf.PackIconKind.Pencil, SkinEditor.Instance, false),
                new SidebarEntry(Helper.FindString("sidebar_mixer"), MaterialDesignThemes.Wpf.PackIconKind.PotMix, SkinMixer.Instance, false),
                new SidebarEntry(Helper.FindString("sidebar_templateManager"), MaterialDesignThemes.Wpf.PackIconKind.Archive, TemplateManager.Instance),
                new SidebarEntry(Helper.FindString("sidebar_settings"), MaterialDesignThemes.Wpf.PackIconKind.Settings, Settings.Instance),
                new SidebarEntry(Helper.FindString("sidebar_about"), MaterialDesignThemes.Wpf.PackIconKind.Information, About.Instance),
                new SidebarEntry(Helper.FindString("sidebar_templateEditor"), MaterialDesignThemes.Wpf.PackIconKind.Pencil, TemplateEditor.Instance, Visibility.Hidden)
            };
        }
        
        private void MManager_SkinChanged(object sender, SkinChangedEventArgs e)
        {
            if (Skins != null)
            {
                //TODO: isSkin may be removed in case the menu background isn't needed
                bool isSkin = System.IO.File.GetAttributes(e.Path) == System.IO.FileAttributes.Directory;

                //switch (e.ChangeType)
                //{
                //    //case System.IO.WatcherChangeTypes.Changed:
                //    //    if (!isSkin)
                //    //    {
                //    //        Skin changed = mSkins.FirstOrDefault(x => x == System.IO.Path.GetDirectoryName(e.Path));
                //    //        if (changed != null)
                //    //        {
                //    //        }
                //    //    }
                //    //    break;
                //    case System.IO.WatcherChangeTypes.Created:
                //        if (isSkin)
                //            Skins.Add(new Skin(e.Path));
                //        break;
                //    case System.IO.WatcherChangeTypes.Deleted:
                //        if (isSkin)
                //        {
                //            Skin removed = Skins.FirstOrDefault(x => x == e.Path);
                //            if (removed != null)
                //                Skins.Remove(removed);
                //        }
                //        break;
                //}
            }
        }

        private void MManager_SkinRenamed(object sender, SkinRenamedEventArgs e)
        {
            if (Skins != null && System.IO.File.GetAttributes(e.Path) == System.IO.FileAttributes.Directory)
            {
                Skin renamed = Skins.First(x => x == e.Path);
                if (renamed != null)
                    Skins[Skins.IndexOf(renamed)].Path = e.Path;
            }
        }
    }
}
