using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OTPClasses;
using OTPService.Data;
using OTPService.Models;
using System.Net;

namespace OTPService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPServicesController : ControllerBase
    {
        private readonly int _otpLen = 6;
        private readonly int _otpWindow = 5;
        private readonly OTPSessionDbContext _context;
        public OTPServicesController(OTPSessionDbContext context) => _context = context;

        // this controller action is for fulfilling the OTP request
        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> otpCreate(OTPSession otplog)
        {
            // create the OTP generation class
            // should be injected in Program.cs using IOTPGene interface
            OTPGene otptest = new OTPGene(_otpLen);
            // generate the OTP
            otplog.otp = otptest.generateToken();
            // log the time from which the OTP was created
            otplog.issued = DateTime.Now;
            await _context.OTPSessions.AddAsync(otplog);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByName), new { usr = otplog.user }, otplog);
        }

        // this controller is for fulfilling OTP validation
        [HttpPost("otp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> otpValidate(OTPSession otplog)
        {
            string valotp = otplog.otp;
            try
            {
                var thelog = _context.OTPSessions.Where<OTPSession>(s => s.otp == valotp).FirstOrDefault();
                // calculate time from when the OTP was generated to when it was used
                TimeSpan duration = DateTime.Now - (DateTime) thelog.issued;
                if (duration.Hours <= 0 && duration.Minutes < _otpWindow)
                {
                    return Ok(otplog);
                }
                else
                    return Content($"{HttpStatusCode.NotAcceptable} Password Expired");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpGet("usr")]
        [ProducesResponseType(typeof(OTPSession), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(string usr)
        {
            var otpsession = await _context.OTPSessions.FindAsync(usr);
            return otpsession == null ? NotFound() : Ok(otpsession);
        }
    }
}
