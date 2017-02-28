using CMS2.DataAccess.Interfaces;
using CMS2.Entities;

namespace CMS2.BusinessLogic
{
    public class ApplicableRateBL : BaseAPCargoBL<ApplicableRate>
    {
        private ICmsUoW _unitOfWork;

        public ApplicableRateBL()
        {
            _unitOfWork = GetUnitOfWork();
        }

        public ApplicableRateBL(ICmsUoW unitOfWork)
                : base(unitOfWork)
        {

        }
    }
}
