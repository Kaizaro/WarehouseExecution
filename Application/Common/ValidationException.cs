namespace WarehouseExecution.Application.Common;

public sealed class ValidationException(string message) : AppException(message);
