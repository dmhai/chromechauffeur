using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ChromeChauffeur.TestApp.Areas.Angular.Models;

namespace ChromeChauffeur.TestApp.Areas.Angular.Controllers
{
    public class AngularTodoController : ApiController
    {
        private static readonly List<AngularTodoItem> Items = new List<AngularTodoItem>
        {
            new AngularTodoItem() { Id = 1, Title = "Item 1" },
            new AngularTodoItem() { Id = 2, Title = "Item 2" },
            new AngularTodoItem() { Id = 3, Title = "Item 3" },
            new AngularTodoItem() { Id = 4, Title = "Item 4" },
            new AngularTodoItem() { Id = 5, Title = "Item 5" },
        };

        public AngularTodoItem Get(int id)
        {
            return Items.Single(x => x.Id == id);
        }

        public List<AngularTodoItem> Get()
        {
            return Items;
        }

        public void Post(AngularTodoItem item)
        {
            var id = Items.Max(x => x.Id) + 1;
            item.Id = id;
            Items.Add(item);
        }
    }
}