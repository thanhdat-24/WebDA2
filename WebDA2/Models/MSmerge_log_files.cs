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
    
    public partial class MSmerge_log_files
    {
        public int id { get; set; }
        public Nullable<System.Guid> pubid { get; set; }
        public Nullable<System.Guid> subid { get; set; }
        public string web_server { get; set; }
        public string file_name { get; set; }
        public System.DateTime upload_time { get; set; }
        public int log_file_type { get; set; }
        public byte[] log_file { get; set; }
    }
}
