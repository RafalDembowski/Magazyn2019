//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Magazyn2019.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Inventory
    {
        public int id_inventory { get; set; }
        public int id_product { get; set; }
        public int id_warehouse { get; set; }
        public int amount { get; set; }
        public int id_move { get; set; }
    
        public virtual Product Product { get; set; }
    }
}