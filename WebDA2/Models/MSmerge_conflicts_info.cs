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
    
    public partial class MSmerge_conflicts_info
    {
        public int tablenick { get; set; }
        public System.Guid rowguid { get; set; }
        public string origin_datasource { get; set; }
        public Nullable<int> conflict_type { get; set; }
        public Nullable<int> reason_code { get; set; }
        public string reason_text { get; set; }
        public Nullable<System.Guid> pubid { get; set; }
        public System.DateTime MSrepl_create_time { get; set; }
        public Nullable<System.Guid> origin_datasource_id { get; set; }
    }
}
