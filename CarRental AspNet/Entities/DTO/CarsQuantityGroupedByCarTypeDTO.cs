using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Entities.DTO
{
    public class CarsQuantityGroupedByCarTypeDTO
    {
		public int Id{get;set;}
        public string CarType{get;set;}
		public int CarsQuantity{get;set;}
    }
}