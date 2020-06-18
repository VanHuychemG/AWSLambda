using Microsoft.AspNetCore.Authorization;

namespace CalendarWebApi
{
    public class CognitoGroupAuthorizationRequirement : IAuthorizationRequirement
    {
        public string CognitoGroup { get; }

        public CognitoGroupAuthorizationRequirement(string cognitoGroup)
        {
            CognitoGroup = cognitoGroup;
        }
    }
}
