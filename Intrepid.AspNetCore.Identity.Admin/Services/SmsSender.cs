using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Options;

namespace Intrepid.AspNetCore.Identity.Admin.Services
{
    public class SmsSender : ISmsSender
    {
        public SmsSender(IOptions<SmsSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public SmsSenderOptions Options { get; } //set only via Secret Manager

        public Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(Options.AccountSid, Options.AuthToken);

            var response = MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(Options.FromNumber),
                to: new Twilio.Types.PhoneNumber(number)
            );

            return response;
        }
    }
}
