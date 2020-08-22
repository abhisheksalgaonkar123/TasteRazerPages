using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;

namespace Taste.DataAccess.Data.Repository
{
    public class FoodTypeRepository : Repository<FoodType>, IFoodTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public FoodTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetFoodTypeListForDropDown()
        {
            return _db.FoodType.Select(f => 
                new SelectListItem {
                    Text = f.Name,
                    Value = f.Id.ToString()

                });
        }

        public void Update(FoodType foodtype)
        {
            var objFromDb = _db.FoodType.FirstOrDefault(s => s.Id == foodtype.Id);
            objFromDb.Name = foodtype.Name;     
            _db.SaveChanges();
        }
    }
}
