using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
//using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TaskDialogInterop;

namespace DSList
{
    public partial class Customer
    {
        private string _Notes;

        public string Notes
        {
            get
            {
                return this._Notes;
            }
            set
            {
                this.NotesChanged = this.NotesHash != value.GetHashCode();
                if (this._Notes != value)
                {
                    this._Notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        public bool NotesChanged { get; set; }

        private int NotesHash { get; set; }

        public DateTime NotesLastWriteTime { get; set; }

        public string GetNotesFileName(bool TouchDir = false, bool TouchFile = false)
        {
            string str = System.IO.Path.Combine(NewMainWindow.DBPath, "Notes");
            string path = System.IO.Path.Combine(str, this.NumberCVZ + ".txt");
            if (TouchDir && !Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
            if (TouchFile && !System.IO.File.Exists(path))
            {
                System.IO.File.Create(path).Dispose();
            }
            return path;
        }

        public async Task SaveNotes()
        {
            this.NotesChanged = false;
            await Task.Run(delegate
            {
                if (string.IsNullOrWhiteSpace(this.Notes))
                {
                    File.Delete(this.GetNotesFileName(true, false));
                }
                else
                {
                    File.WriteAllText(this.GetNotesFileName(true, false), $"[{DateTime.UtcNow.AddHours(3.0)}] {Environment.UserName}: {Regex.Replace(this.Notes, @"^\[.*\].*:\s", string.Empty)}");
                }
            });
            await this.LoadNotes();

        }

        public async Task LoadNotes()
        {
            await Task.Run(delegate
            {

            });
            //return new Task((Action)delegate { });
        }
        

    public async Task AppendNotes()
    {
        await Task.Run(delegate
        {
            System.IO.File.AppendAllText(this.GetNotesFileName(true, false), "\n" + $"[{DateTime.UtcNow.AddHours(3.0)}] {Environment.UserName}: {Regex.Replace(this.Notes, @"^\[.*\].*:\s", string.Empty)}");
        });
        await this.LoadNotes();
    }

}

}
