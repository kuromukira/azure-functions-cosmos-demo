namespace Demo.Function.Models;

public record struct DomainResponse(bool Success, double? RU = null, string Message = null, dynamic Data = null);