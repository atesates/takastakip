﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM.Northwind.Entities.ComplexTypes.IlacTakip;
using WM.Northwind.Entities.Concrete.IlacTakip;
//using WM.Northwind.Entities.Concrete.Optimization.IlacTakip;

namespace WM.Northwind.Business.Abstract.IlacTakip
{
    public interface IYayinlamaTurService
    {
        YayinlamaTur GetById(int yayinlamaTurId);
        List<YayinlamaTur> GetList();
        //List<YayinlamaTur> GetByCategory(int categoryId);
        void Insert(YayinlamaTur yayinlamaTur);
        void Update(YayinlamaTur yayinlamaTur);
        void Delete(int yayinlamaTurId);
                        
    }
} 