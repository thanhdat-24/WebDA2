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
    
    public partial class sysmergeschemaarticle
    {
        public string name { get; set; }
        public Nullable<byte> type { get; set; }
        public int objid { get; set; }
        public System.Guid artid { get; set; }
        public string description { get; set; }
        public Nullable<byte> pre_creation_command { get; set; }
        public System.Guid pubid { get; set; }
        public Nullable<byte> status { get; set; }
        public string creation_script { get; set; }
        public byte[] schema_option { get; set; }
        public string destination_object { get; set; }
        public string destination_owner { get; set; }
        public int processing_order { get; set; }
    }
}
