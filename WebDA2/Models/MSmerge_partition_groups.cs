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
    
    public partial class MSmerge_partition_groups
    {
        public int partition_id { get; set; }
        public short publication_number { get; set; }
        public Nullable<long> maxgen_whenadded { get; set; }
        public Nullable<bool> using_partition_groups { get; set; }
        public bool is_partition_active { get; set; }
    
        public virtual MSmerge_dynamic_snapshots MSmerge_dynamic_snapshots { get; set; }
    }
}
