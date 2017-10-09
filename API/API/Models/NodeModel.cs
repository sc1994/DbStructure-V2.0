using System;
using System.Collections.Generic;

namespace API.Models
{
    public class NodeModel
    {
        // ReSharper disable once InconsistentNaming
        public string id { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public string label { get; set; } = string.Empty;
        // ReSharper disable once InconsistentNaming
        public List<NodeModel> children { get; set; } = new List<NodeModel>();
    }
}