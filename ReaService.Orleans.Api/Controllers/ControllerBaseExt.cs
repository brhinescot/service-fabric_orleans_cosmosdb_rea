#region Using Directives

using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace ReaService.Orleans.Api.Controllers
{
    /// <summary>
    ///     Provides additional application specific helpers for returning status codes.
    /// </summary>
    public abstract class ControllerBaseExt : ControllerBase
    {
        protected BadRequestObjectResult BadRequest(string message)
        {
            return BadRequest(new ErrorResponse(message));
        }

        protected BadRequestObjectResult ParameterMissing(string parameterName)
        {
            return BadRequest(new ErrorResponse($"The parameter '{parameterName}' is required."));
        }

        protected BadRequestObjectResult ContentMissing(string typeName)
        {
            return BadRequest(new ErrorResponse($"Expected content of type '{typeName}' in the request."));
        }

        protected BadRequestObjectResult DoNotSetId()
        {
            return BadRequest(new ErrorResponse("The id property should not be set."));
        }

        protected BadRequestObjectResult PutParametersMismatch(string entityName, string propertyName, string parameterName)
        {
            return BadRequest($"The '{propertyName}' property of the '{entityName}' parameter should match the '{parameterName}' parameter");
        }

        protected NotFoundObjectResult ApplicationNotFound(string appId)
        {
            return NotFound(new ErrorResponse($"An app with id '{appId}' was not found in the organization."));
        }

        protected NotFoundObjectResult PackageNotFound(string packageId)
        {
            return NotFound(new ErrorResponse($"A package with id '{packageId}' was not found in the organization."));
        }

        protected NotFoundObjectResult UserNotFound(string userId)
        {
            return NotFound(new ErrorResponse($"A user with id '{userId}' was not found in the organization."));
        }

        protected NotFoundObjectResult UserNotFound(IPrincipal principal)
        {
            return NotFound(new ErrorResponse($"The user '{principal?.Identity?.Name}' was not found in the organization."));
        }
    }
}