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
    
    public partial class Move
    {
        public int id_move { get; set; }
        public int id_custmer { get; set; }
        public int id_warehouse1 { get; set; }
        public int id_warehouse2 { get; set; }
        public int type { get; set; }
        public System.DateTime time { get; set; }
        public int id_user { get; set; }
        public int number { get; set; }

        public List<Inventory> Inventories { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
        public virtual Warehouse WarehouseOne { get; set; }
        public virtual Warehouse WarehouseTwo { get; set; }
    }
}
