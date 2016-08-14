using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Contracts.Models
{
    public class TreeDirectory : ITreeFile, INotifyPropertyChanged
    {
        private bool _isSelected;
        public string Name { get; set; }
        public List<ITreeFile> Files { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = false;
                OnPropertyChanged();
            }
        }

        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
        public FileDiff FileDiff { get; set; }
        public Type GetTreeType { get { return this.GetType(); } }
        public bool IsExpanded { get; set; }
        public bool IsSelectable { get; set; }
        public long Added { get; set; }
        public long Removed { get; set; }

        public TreeDirectory(string name)
        {
            Name = name;
            Files = new List<ITreeFile>();
            FileDiff = null;
            IsSelectable = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}