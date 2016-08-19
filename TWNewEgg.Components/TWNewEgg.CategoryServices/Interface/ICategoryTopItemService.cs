using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CategoryServices.Interface
{
    public interface ICategoryTopItemService
    {
        List<CategoryTopItemDM> GetCategoryTopItem(CategoryTopItemDM SearchCondition);

        ResponseMessage<List<CategoryTopItemDM>> SaveCategoryTopItem(TopItemDM NewData);
    }
}
