using Knowit.Umbraco.TokenReplacement.Service;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;

namespace Knowit.Umbraco.TokenReplacement.API
{

    public class TokenReplacementApiController : UmbracoApiController
    {

        private readonly ICmsDictionaryReader _cmsDictionaryReader;
        public TokenReplacementApiController(ICmsDictionaryReader cmsDictionaryReader)
        {
            _cmsDictionaryReader = cmsDictionaryReader;
        }
        //[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [HttpGet("umbraco/api/tokenreplacement/tokens")]
        public IActionResult Tokens()
        {
            return Ok(_cmsDictionaryReader.GetAll().Dictionary);
        }
    }
}
