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
    
    public partial class KhoHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhoHang()
        {
            this.ChiTietKhoHangs = new HashSet<ChiTietKhoHang>();
        }
    
        public int ID_Kho { get; set; }
        public string Ten_Kho { get; set; }
        public Nullable<int> id_chinhanh { get; set; }
        public System.Guid rowguid { get; set; }
    
        public virtual ChiNhanh ChiNhanh { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietKhoHang> ChiTietKhoHangs { get; set; }
    }
}
