using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
     public class SmsController : ControllerBase
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioNumber;

        public SmsController(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"]
                ?? throw new Exception("Twilio AccountSid is missing");
            _authToken = configuration["Twilio:AuthToken"]
                ?? throw new Exception("Twilio AuthToken is missing");
            _twilioNumber = configuration["Twilio:TwilioNumber"]
                ?? throw new Exception("TwilioNumber is missing");

            // Initialize the Twilio client 
            TwilioClient.Init(_accountSid, _authToken);
        }

        // Model records for deserializing JSON payloads
        public record SmsRequest(string Message, string PhoneNumber);
        public record WebhookData(string PhoneNumber, string Message);
        public record WebhookRequest(string Event, WebhookData Data);

        /// <summary>
        /// REST API endpoint to manually send an SMS.
        /// POST /sms/send-sms
        /// JSON Body: { "message": "Your text here", "phoneNumber": "+1RecipientNumber" }
        /// </summary>
        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest smsRequest)
        {
            if (string.IsNullOrWhiteSpace(smsRequest.Message) ||
                string.IsNullOrWhiteSpace(smsRequest.PhoneNumber))
            {
                return BadRequest(new { error = "Missing message or phoneNumber in the request body." });
            }

            try
            {
                var message = await MessageResource.CreateAsync(
                    body: smsRequest.Message,
                    from: new PhoneNumber(_twilioNumber),
                    to: new PhoneNumber(smsRequest.PhoneNumber)
                );

                return Ok(new { status = "success", messageSid = message.Sid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }

        /// <summary>
        /// Webhook endpoint to receive events.
        /// POST /sms/webhook
        /// Expected JSON Body: 
        /// {
        ///   "event": "new_signup",
        ///   "data": { "phoneNumber": "+1RecipientNumber", "message": "Welcome!" }
        /// }
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequest webhookRequest)
        {
            Console.WriteLine($"Received webhook event: {webhookRequest.Event}");

            if (webhookRequest.Event == "new_signup" &&
                webhookRequest.Data != null &&
                !string.IsNullOrWhiteSpace(webhookRequest.Data.PhoneNumber) &&
                !string.IsNullOrWhiteSpace(webhookRequest.Data.Message))
            {
                try
                {
                    var message = await MessageResource.CreateAsync(
                        body: webhookRequest.Data.Message,
                        from: new PhoneNumber(_twilioNumber),
                        to: new PhoneNumber(webhookRequest.Data.PhoneNumber)
                    );

                    return Ok(new { status = "success", messageSid = message.Sid });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { status = "error", error = ex.Message });
                }
            }
            else
            {
                return BadRequest(new { error = "Invalid webhook payload." });
            }
        }
    }
}
