using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;

namespace Taste.DataAccess.Data.Repository
{
    class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;

        public MenuItemRepository( ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(MenuItem menuItem)
        {
            var menuItemToUpdate = _db.MenuItem.FirstOrDefault(a => a.Id == menuItem.Id);
            if(menuItemToUpdate != null)
            {
                menuItemToUpdate.Name = menuItem.Name;
                menuItemToUpdate.Description = menuItem.Description;
                menuItemToUpdate.Price = menuItem.Price;
                menuItemToUpdate.CategoryId = menuItem.CategoryId;
                if(menuItem.Image != null)
                {
                    menuItemToUpdate.Image = menuItem.Image;
                }
                _db.SaveChanges();
            }
        }
    }
}
