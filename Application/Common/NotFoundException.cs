namespace WarehouseExecution.Application.Common;

public sealed class NotFoundException(string message) : AppException(message);
