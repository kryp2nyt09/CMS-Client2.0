using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2.Client.SyncHelper
{
    public class SyncTables
    {
        public SyncTables()
        {
            Status = TableStatus.Good;
            isSelected = false;     
            
        }
        [DisplayName("Tables")]
        public string TableName { get; set; }
        [DisplayName("Status")]
        public TableStatus Status { get; set; }

        [DisplayName("Select")]
        public bool isSelected { get; set; }


       
                
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
