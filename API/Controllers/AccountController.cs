using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Services;
using Domain;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly SignInManager<AppUser> _signinManager;
    private readonly EmailSender _emailSender;
    public AccountController(UserManager<AppUser> userManager,
      SignInManager<AppUser> signinManager,
      TokenService tokenService,
      EmailSender emailSender)
    {
      _signinManager = signinManager;
      _tokenService = tokenService;
      _userManager = userManager;
      _emailSender = emailSender;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == loginDto.Email);
      if (user == null) return Unauthorized("Invalid email");

      if (!user.EmailConfirmed) return Unauthorized("Email not confirmed");

      var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

      if (result.Succeeded)
      {
        await SetRefreshToken(user);
        return CreateUserObject(user);
      }
      return Unauthorized("Invalid password");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
      {
        ModelState.AddModelError("email", "Email taken");
        return ValidationProblem();
      }
      if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
      {
        ModelState.AddModelError("username", "Username taken");
        return ValidationProblem();
      }

      var user = new AppUser
      {
        DisplayName = registerDto.DisplayName,
        Email = registerDto.Email,
        UserName = registerDto.Username,
      };
      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) return BadRequest("Problem registering user");

      var origin = Request.Headers["origin"];
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

      var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
      var message = $"<p>Please click the link below to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify</a></p>]";

      await _emailSender.SendEmailAsync(user.Email, "Please verify your email", message);

      return Ok("Registration successful. Please check your email");
    }

    [AllowAnonymous]
    [HttpPost("verifyEmail")]
    public async Task<IActionResult> VerifyEmail(string token, string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) return Unauthorized();
      var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
      var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

      var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
      if (!result.Succeeded) return BadRequest("Couuld not verify email address");

      return Ok("Email confirmed, you can now login");
    }

    [AllowAnonymous]
    [HttpGet("resendEmailConfirmationLink")]
    public async Task<IActionResult> ResendEmailConfirmationLink(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) return Unauthorized();

      var origin = Request.Headers["origin"];
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

      var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
      var message = $"<p>Please click the link below to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify</a></p>]";

      await _emailSender.SendEmailAsync(user.Email, "Please verify your email", message);

      return Ok("Email verification message sent.");
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
      var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
      await SetRefreshToken(user);
      return CreateUserObject(user);
    }

    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
      var refreshToken = Request.Cookies["refreshToken"];
      var user = await _userManager.Users.Include(r => r.RefreshTokens).Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));
      if (user == null) return Unauthorized();

      var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

      if (oldToken != null && !oldToken.IsActive) return Unauthorized();
      if (oldToken != null) oldToken.Revoked = DateTime.UtcNow;

      return CreateUserObject(user);
    }
    private async Task SetRefreshToken(AppUser user)
    {
      var refreshToken = _tokenService.GenerateRefreshToken();
      user.RefreshTokens.Add(refreshToken);
      await _userManager.UpdateAsync(user);

      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTime.UtcNow.AddDays(7),

      };
      Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

    private UserDto CreateUserObject(AppUser user)
    {
      return new UserDto
      {
        DisplayName = user.DisplayName,
        Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
        Token = _tokenService.CreateToken(user),
        Username = user.UserName,
      };
    }
  }
}