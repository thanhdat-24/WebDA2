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
    
    public partial class MSmerge_replinfo
    {
        public System.Guid repid { get; set; }
        public bool use_interactive_resolver { get; set; }
        public int validation_level { get; set; }
        public long resync_gen { get; set; }
        public string login_name { get; set; }
        public string hostname { get; set; }
        public byte[] merge_jobid { get; set; }
        public int sync_info { get; set; }
    }
}
