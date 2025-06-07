using Microsoft.AspNetCore.Authorization;
namespace AUEUMS.Custom
{
    public class CustomUserRequireClaim: IAuthorizationRequirement
    {
        public string ClaimType { get; }
        public CustomUserRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }
    }
}
