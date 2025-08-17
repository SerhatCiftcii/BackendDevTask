using MyProject.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Core.Interfaces
{
    public interface IProductRepository
    {
        //tüm productlarımızı getirir
        Task<IEnumerable<Product>> GetAllAsync();
        //id ye göre getri
        Task<Product> GetByIdAsync(int id);
        //ekleme
        Task AddAsync(Product product);
        //güncelleme
        Task UpdateAsync(Product product);
        //silme işlemimiz
        Task DeleteAsync(int id);
    }
}
