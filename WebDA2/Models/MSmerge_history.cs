//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebDA2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MSmerge_history
    {
        public Nullable<int> session_id { get; set; }
        public int agent_id { get; set; }
        public string comments { get; set; }
        public int error_id { get; set; }
        public byte[] timestamp { get; set; }
        public bool updateable_row { get; set; }
        public System.DateTime time { get; set; }
    }
}