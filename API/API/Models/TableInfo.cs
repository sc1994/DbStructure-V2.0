using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class TableInfo
    {
        // ReSharper disable once InconsistentNaming
        public string tableName { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string tableDescribe { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string newfield { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string fieldname { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string identifying { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string primarykey { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string types { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string lengths { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string ornull { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string defaults { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string describe { get; set; } = string.Empty;
    }
}