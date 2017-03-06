using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Client.SyncHelper
{
    public class SyncTables : INotifyPropertyChanged
    {

        private TableStatus _status;
        private bool _isSelected;
        public event PropertyChangedEventHandler PropertyChanged;
        public SyncTables()
        {
            Status = TableStatus.Good;
            isSelected = false;

        }
        [DisplayName("Tables")]
        public string TableName { get; set; }

        [DisplayName("Status")]
        public TableStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        [DisplayName("Select")]
        public bool isSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    OnPropertyChanged("isSelected");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public enum TableStatus
    {
        Fresh, // currently provision
        Good, //sync working
        Bad, //sync not working
        Provisioned,
        Deprovisioned,
        ErrorProvision,
        ErrorDeprovision
    }

}
