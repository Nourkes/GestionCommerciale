namespace GestionCommerciale.Application.DTOs.Auth;

public record LoginResponseDto(string Token, DateTime ExpiresAt, string Username);