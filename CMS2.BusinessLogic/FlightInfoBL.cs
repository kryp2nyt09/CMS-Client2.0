using System;
using System.Linq.Expressions;
using CMS2.DataAccess.Interfaces;
using CMS2.Entities;

namespace CMS2.BusinessLogic
{
    public class FlightInfoBL:BaseAPCargoBL<FlightInfo>
    {
        private ICmsUoW _unitOfWork;

        public FlightInfoBL()
        {
            _unitOfWork = GetUnitOfWork();
        }

        public FlightInfoBL(ICmsUoW unitOfWork)
            : base(unitOfWork)
        {
        }

        public override Expression<Func<FlightInfo, object>>[] Includes()
        {
            return new Expression<Func<FlightInfo, object>>[]
                {
                    x => x.OriginCity,
                    x=>x.DestinationCity
                };
        }
    }
}
