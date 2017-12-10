using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CouchExplorer.Infrastructure;

namespace CouchExplorer.Features.Explorer
{
    public class ExplorerViewModel : ViewModelBase
    {
        private string _currentPath;
        private ExplorerItemViewModel _selectedItem;

        public ExplorerViewModel(string path) => CurrentPath = path;

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value; 
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(Items));
            }
        }

        public IEnumerable<ExplorerItemViewModel> Items
        {
            get
            {
                var items = new List<ExplorerItemViewModel>();
                items.AddRange(GetVisibleDirectories());
                items.AddRange(GetVisibleFiles());
                return items;
            }
        }

        public ExplorerItemViewModel SelectedItem
        {
            get => _selectedItem ?? Items.FirstOrDefault();
            set
            {
                _selectedItem = value; 
                
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand SelectItemCommand => new RelayCommand(SelectItem);

        private void SelectItem()
        {
            if (SelectedItem.IsFile)
            {
                Process.Start(SelectedItem.FilePath);
                return;
            }

            CurrentPath = SelectedItem.FilePath;
            SelectedItem = Items.FirstOrDefault();
        }

        public ICommand GoBackCommand => new RelayCommand(GoBack);

        private void GoBack()
        {
            if (SelectedItem.DirectoryName == Path.GetPathRoot(SelectedItem.DirectoryName))
                return;

            CurrentPath = Path.GetDirectoryName(SelectedItem.DirectoryName);
            SelectedItem = Items.FirstOrDefault();
        }

        #endregion

        #region Private Helpers

        private IEnumerable<ExplorerItemViewModel> GetVisibleDirectories()
        {
            return new DirectoryInfo(CurrentPath).GetDirectories()
                .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(f => new ExplorerItemViewModel(f.FullName));
        }

        private IEnumerable<ExplorerItemViewModel> GetVisibleFiles()
        {
            return new DirectoryInfo(CurrentPath).GetFiles()
                .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(f => new ExplorerItemViewModel(f.FullName));
        }

        #endregion
    }
}
