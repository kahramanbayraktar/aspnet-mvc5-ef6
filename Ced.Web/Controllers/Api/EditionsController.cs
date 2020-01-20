using Ced.BusinessServices;
using Ced.Web.Filters;
using System.Web.Http;

namespace Ced.Web.Controllers.Api
{
    [ApiAuthentication]
    public class EditionsController : ApiController
    {
        private readonly IEditionServices _editionServices;

        public EditionsController(IEditionServices editionServices)
        {
            _editionServices = editionServices;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var edition = _editionServices.GetEditionById(id);
            if (edition == null)
                return NotFound();
            return Ok(edition);
        }

        [HttpGet]
        public IHttpActionResult GetByAxId(int axId)
        {
            var edition = _editionServices.GetEditionByAxId(axId);
            if (edition == null)
                return NotFound();
            return Ok(edition);
        }

        [HttpGet]
        [ActionName("wb365")]
        public IHttpActionResult GetForWb365()
        {
            var editions = _editionServices.GetEditionsForWb365();
            if (editions == null)
                return NotFound();
            return Ok(editions);
        }

        [HttpGet]
        [ActionName("wisent")]
        public IHttpActionResult GetForWisent(string eventName = null, short? financialYearStart = null, int? count = null)
        {
            var editions = _editionServices.GetEditionsForWisent(eventName, financialYearStart, count);
            if (editions == null)
                return NotFound();
            return Ok(editions);
        }

        [HttpGet]
        [ActionName("startdatediff")]
        public IHttpActionResult GetStartDateDiff(int axId)
        {
            var diff = _editionServices.GetEditionStartDateDiff(axId);
            if (diff == null)
                return NotFound();
            return Ok(diff);
        }
    }
}
