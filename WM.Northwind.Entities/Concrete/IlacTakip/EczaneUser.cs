using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Core.Entities;
using WM.Northwind.Entities.Concrete.Authorization;

namespace WM.Northwind.Entities.Concrete.IlacTakip
{
    public class EczaneUser : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EczaneId { get; set; }
        public virtual Eczane Eczane { get; set; }
        public virtual User User { get; set; }

    }
}